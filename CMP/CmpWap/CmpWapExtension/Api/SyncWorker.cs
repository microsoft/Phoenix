//*****************************************************************************
// File: SyncWorker.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that are used for syncing with 
//          an external service (CMP) and Azure.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient;
using AzureAdminClientLib;
using CmpServiceLib;
using System.Web.Configuration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{

    /// <remarks>
    /// This class contains methods that are used for syncing with 
    /// an external service (CMP) and Azure in different threads.
    /// </remarks>
    public class SyncWorker
    {
        private static EventLog _eventLog;
        private static SyncWorker _syncWorker;
        private Thread _workerThread;
        private Thread _azureThread;
        private const int SleepyTime = 60000;
        private const int AzureSleepTime = 24 * 60 * 60 * 1000;

        //*********************************************************************
        ///
        /// <summary>
        /// This method starts the thread
        /// </summary>
        /// <param name="eventLog"></param>
        /// 
        //*********************************************************************

        public static void StartAsync(EventLog eventLog)
        {
            _eventLog = eventLog;

            if(null == _syncWorker)
                _syncWorker = new SyncWorker();

            _syncWorker._workerThread = new Thread(_syncWorker.Worker);
            _syncWorker._azureThread = new Thread(_syncWorker.AzureWorker);

            _syncWorker._workerThread.Start();
            _syncWorker._azureThread.Start();
        }

        #region --- CMP Sync Worker ---

        //*********************************************************************
        ///
        /// <summary>
        /// Thread worker
        /// </summary>
        /// 
        //*********************************************************************

        //*** NOTE * Refresh

        private void Worker()
        {
            while (true)
            {
                try
                {
                    SynchWithCmp();
                    Thread.Sleep(SleepyTime);
                }
                catch (Exception ex)
                {
                    LogThis(ex, EventLogEntryType.Error, 
                        "Exception in SyncWorker.SynchWithCmp()", 0, 0);
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Add a CMP VM to the CmpWap DB
        /// </summary>
        /// <param name="cmpVm"></param>
        /// <param name="cwdb"></param>
        /// 
        //*********************************************************************

        private void ImportVm(CmpClient.CmpService.VmDeploymentRequest cmpVm, CmpWapDb cwdb)
        {
            try
            {
                var cmpReq = new CmpRequest()
                {
                    AccessGroupId = 0,
                    Active = true,
                    AddressFromVm = null,
                    CmpRequestID = cmpVm.ID,
                    Config = cmpVm.Config,
                    Domain = null,
                    ExceptionMessage = cmpVm.ExceptionMessage,
                    Id = 0,
                    FeatureSpec = null,
                    LastStatusUpdate = cmpVm.LastStatusUpdate,
                    ParentAppName = cmpVm.ParentAppName,
                    RequestType = cmpVm.RequestType,
                    SourceImageName = null,
                    SourceServerName = cmpVm.SourceServerName,
                    StatusCode = cmpVm.Status,
                    StatusMessage = cmpVm.StatusMessage,
                    StorageSpec = null,
                    TagData = cmpVm.TagData,
                    TagID = cmpVm.TagID,
                    TargetLocation = cmpVm.TargetLocation,
                    TargetVmName = cmpVm.TargetVmName,
                    UserSpec = null,
                    VmSize = cmpVm.VmSize,
                    WapSubscriptionID = null,
                    Warnings = null,
                    WhenRequested = cmpVm.WhenRequested,
                    WhoRequested = cmpVm.WhoRequested
                };

                cwdb.InsertVmDepRequest(cmpReq);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in SyncWorker.ImportVm()", 0, 0);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resGroup"></param>
        /// <param name="cwdb"></param>
        /// 
        //*********************************************************************

        private void ImportApp(CmpServiceLib.Models.Container resGroup, CmpWapDb cwdb)
        {
            try
            {
                var app = new Application()
                {
                    ApplicationId = 0,
                    Code = resGroup.Code,
                    Name = resGroup.Name,
                    HasService = (bool)resGroup.HasService,
                    IsActive = resGroup.IsActive,
                    SubscriptionId = resGroup.SubscriptionId,
                    CreatedOn = resGroup.CreatedOn,
                    CreatedBy = resGroup.CreatedBy,
                    LastUpdatedOn = resGroup.LastUpdatedOn,
                    LastUpdatedBy = resGroup.LastUpdatedBy,
                    CIOwner = resGroup.CIOwner,
                    Region = resGroup.Region
                };

                cwdb.InsertApp(app);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in SyncWorker.ImportApp()", 0, 0);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Sync the CMP VM list with the CmpWap VM list
        /// </summary>
        /// 
        //*********************************************************************

        public void SynchWithCmp()
        {
            var cmp = new CmpApiClient(_eventLog);
            var cwdb = new CmpWapDb();

            //*** Fetch the CMP resource group list ***
            var cmpResGroupList = cmp.FetchAzureResourceGroups();

            //*** Fetch the CmpWap app list ***
            var cmpWapAppList = cwdb.FetchAppList();

            //*** Fold ***
            foreach (var cmpResGroup in cmpResGroupList.Where(cmpResGroup =>
                !cmpWapAppList.Any(cmpWapApp => cmpResGroup.Name.Equals(
                    cmpWapApp.Name, StringComparison.InvariantCultureIgnoreCase))))
                ImportApp(cmpResGroup, cwdb);

            //*** Fetch the CMP VM list ***
            var cmpVmList = cmp.FetchCmpRequests();

            //*** Fetch the CmpWap VM list ***
            var cmpWapVmList = cwdb.FetchVmDepRequests(null, true);

            //*** Fold ***
            foreach (var cmpVm in cmpVmList.Where(cmpVm =>
                !cmpWapVmList.Any(cmpWapVm => cmpVm.TargetVmName.Equals(
                    cmpWapVm.TargetVmName, StringComparison.InvariantCultureIgnoreCase))))
                ImportVm(cmpVm, cwdb);
        }

        #endregion

        #region --- Azure Sync Worker --

        private void AzureWorker()
        {
            var retryCount = 1;

            while (true)
            {
                try
                {
                    SyncWithAzure();
                    Thread.Sleep(AzureSleepTime);
                }
                catch (Exception ex)
                {
                    var error = Utilities.UnwindExceptionMessages(ex);

                    if (error.Contains("503") & (5 > retryCount++))
                        Thread.Sleep((int)(1000 * Math.Pow(4.17, retryCount)));
                    else if (3 > retryCount++)
                        Thread.Sleep(5000 * retryCount);
                    else
                    {
                        LogThis(ex, EventLogEntryType.Error,
                            "Exception in SyncWorker.SyncWithAzure()", 0, 0);
                        Thread.Sleep(1000 * 60 * 5);
                    }
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Sync the CmpWapDb AzureRegion and VmSize tables with Azure domain data
        /// </summary>
        /// 
        //*********************************************************************
        public static void SyncWithAzure()
        {
            var ars = new AzureRefreshService(_eventLog, WebConfigurationManager.ConnectionStrings["CMPContext"].ConnectionString);
            IEnumerable<AzureCatalogue> azureCatalogueResult = ars.FetchAzureInformationWithArm().ToList();

            UpdateAzureRegions(azureCatalogueResult);
            UpdateVmSizes(azureCatalogueResult);
            UpdateRegionVmSizeMappings(azureCatalogueResult);
            UpdateVmOs(azureCatalogueResult);
            UpdateRegionVmOsMappings(azureCatalogueResult);
        }

        private static void UpdateAzureRegions(IEnumerable<AzureCatalogue> azureCatalogueResult)
        {
            try
            {
                var cmpWapDb = new CmpWapDb();
                var cmpRegions = cmpWapDb.FetchAzureRegionList(onlyActiveOnes: false);
                var azureRegions = azureCatalogueResult.Select(ac => new AzureRegion
                {
                    Name = ac.RegionName,
                    Description = ac.RegionDisplayName,
                    OsImageContainer = null,
                    IsActive = true,
                    CreatedBy = "CMP WAP Extension",
                    LastUpdatedBy = "CMP WAP Extension",
                    CreatedOn = DateTime.UtcNow,
                    LastUpdatedOn = DateTime.UtcNow
                });

                var newRegions = azureRegions.Where(azureItem => !cmpRegions.Any(cmpItem => string.Equals(azureItem.Name.Replace(" ", string.Empty), cmpItem.Name.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase))).ToList();

                if (newRegions.Any())
                {
                    cmpWapDb.InsertAzureRegionByBatch(newRegions);
                }          
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in UpdateAzureRegions: " + ex.ToString());
            }
        }

        private static void UpdateVmSizes(IEnumerable<AzureCatalogue> azureCatalogueResult)
        {
            try
            {
                var cmpWapDb = new CmpWapDb();
                var cmpWapVmSizes = cmpWapDb.FetchVmSizeInfoList(onlyActiveOnes: false).ToList();
                var newVmSizes = new List<VmSize>();
                var azureCatalogueVmSizes = new List<AzureVmSizeArmData>();

                //Eliminate dupes, we only need them to establish the mappings in another method.
                foreach (var regionInCatalogue in azureCatalogueResult)
                {
                    azureCatalogueVmSizes.AddRange(regionInCatalogue.VmSizes); 
                }
                azureCatalogueVmSizes = azureCatalogueVmSizes.Distinct(new AzureVmSizeArmData.AzureVmSizeComparer()).ToList();

                //Translate and convert each AzureVmSizeArmData object into a VmSize object.
                foreach (var vmSize in azureCatalogueVmSizes)
                {
                    if (!cmpWapVmSizes.Any(x => string.Equals(vmSize.name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var newVmSize = new VmSize
                        {
                            Name = vmSize.name,
                            Cores = vmSize.numberOfCores,
                            Memory = vmSize.memoryInMB,
                            MaxDataDiskCount = vmSize.maxDataDiskCount,
                            IsActive = true,
                            CreatedBy = "CMP WAP Extension",
                            LastUpdatedBy = "CMP WAP Extension",
                            CreatedOn = DateTime.UtcNow,
                            LastUpdatedOn = DateTime.UtcNow
                        };

                        if (vmSize.memoryInMB >= 1000)
                        {
                            newVmSize.Description = vmSize.name + " - " + vmSize.numberOfCores + " Cores, " + vmSize.memoryInMB / 1000 + " GB, " + vmSize.maxDataDiskCount + " Disk";
                        }
                        else
                        {
                            newVmSize.Description = vmSize.name + " - " + vmSize.numberOfCores + " Cores, " + vmSize.memoryInMB + " MB, " + vmSize.maxDataDiskCount + " Disk";
                        }

                        newVmSizes.Add(newVmSize);
                    }
                }
                            

                if (newVmSizes.Any())
                {
                    cmpWapDb.InsertVmSizeByBatch(newVmSizes);
                }       
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in UpdateVmSizes: " + ex.ToString());
            }
        }

        private static void UpdateVmOs(IEnumerable<AzureCatalogue> azureCatalogueResult)
        {
            try
            {
                var cmpWapDb = new CmpWapDb();
                var cmpVmOses = cmpWapDb.FetchOsInfoList(onlyActiveOnes: false).ToList();
                var newVmOses = new List<VmOs>();
                var azureCatalogueVmOses = new List<AzureVmOsArmData>();

                //Eliminate dupes, we only need them to establish the mappings in another method.
                foreach (var regionInCatalogue in azureCatalogueResult)
                {
                    azureCatalogueVmOses.AddRange(regionInCatalogue.VmOses);               
                }
                azureCatalogueVmOses = azureCatalogueVmOses.Distinct(new AzureVmOsArmData.AzureVmOsArmDataComparer()).ToList();

                //Translate and convert each AzureVmOsArmData object into a VmOs object.
                var azureVmOses = azureCatalogueVmOses.Select(data => new VmOs
                {
                    Name = (data.Publisher + ", " + data.Offer + ", " + data.SKU).Length >= 100 ? (data.Publisher + ", " + data.Offer + ", " + data.SKU).Substring(0, 100) : (data.Publisher + ", " + data.Offer + ", " + data.SKU),//Adjusting for DB limitations in length, so that the logic comparison does not fail
                    Description = string.Empty,
                    OsFamily = string.Empty,
                    AzureImageName = string.Empty,
                    IsCustomImage = false,
                    IsActive = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = @"CMP WAP Extension",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = @"CMP WAP Extension",
                    AzureImagePublisher = data.Publisher,
                    AzureImageOffer = data.Offer,
                    AzureWindowsOSVersion = data.SKU
                });

                newVmOses.AddRange(azureVmOses.Where(azureItem => !cmpVmOses.Any(cmpItem => string.Equals(azureItem.Name, cmpItem.Name, StringComparison.InvariantCultureIgnoreCase))).ToList());

                if (newVmOses.Any())
                {
                    cmpWapDb.InsertOsInfoByBatch(newVmOses);
                }          
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in UpdateVmOs: " + ex.ToString());
            }
        }

        private static void UpdateRegionVmSizeMappings(IEnumerable<AzureCatalogue> azureCatalogueResult)
        {
            var cmpWapDb = new CmpWapDb();
            var cmpWapRegions = cmpWapDb.FetchAzureRegionList(onlyActiveOnes: false).ToList();
            var cmpWapVmSizes = cmpWapDb.FetchVmSizeInfoList(onlyActiveOnes: false).ToList();
            var newMappings = new List<AzureRegionVmSizeMapping>();

            try
            {
                foreach (var regionInCatalogue in azureCatalogueResult)
                {
                    foreach (var vmSize in regionInCatalogue.VmSizes)
                    {
                        //We need to get the IDs to make the mapping object. Query the VmSize and Region tables to get them.
                        var vmSizeToMap = cmpWapVmSizes.FirstOrDefault(x => string.Equals(vmSize.name, x.Name, StringComparison.InvariantCultureIgnoreCase));
                        var regionToMap = cmpWapRegions.FirstOrDefault(y => string.Equals(regionInCatalogue.RegionName.Replace(" ", string.Empty), y.Name.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase));
                        if (vmSizeToMap == null || regionToMap == null) 
                            continue; //something's wrong here if we reach this.

                        var newMapping = new AzureRegionVmSizeMapping
                        {
                            AzureRegionId = regionToMap.AzureRegionId,
                            VmSizeId = vmSizeToMap.VmSizeId,
                            IsActive = true
                        };

                        newMappings.Add(newMapping);
                    }
                }             

                if (newMappings.Any())
                {
                    cmpWapDb.SetRegionVmSizeMappingsByBatch(newMappings);
                } 
            }
            catch (Exception ex)
            {
               throw new Exception("Exception caught in UpdateRegionVmSizeMappings: " + ex.ToString());
            }            
        }

        private static void UpdateRegionVmOsMappings(IEnumerable<AzureCatalogue> azureCatalogueResult)
        {
            var cmpWapDb = new CmpWapDb();
            var cmpWapRegions = cmpWapDb.FetchAzureRegionList(onlyActiveOnes: false).ToList();
            var cmpWapVmOses = cmpWapDb.FetchOsInfoList(onlyActiveOnes: false).ToList();
            var newMappings = new List<AzureRegionVmOsMapping>();

            try
            {
                foreach (var regionInCatalogue in azureCatalogueResult)
                {
                    foreach (var vmOs in regionInCatalogue.VmOses)
                    {
                        //Since we format the VmOs name when inserting it into the DB table, to look it up, we will need to do the same format operation as before
                        var vmOsName = (vmOs.Publisher + ", " + vmOs.Offer + ", " + vmOs.SKU).Length >= 100
                            ? (vmOs.Publisher + ", " + vmOs.Offer + ", " + vmOs.SKU).Substring(0, 100)
                            : (vmOs.Publisher + ", " + vmOs.Offer + ", " + vmOs.SKU); //Adjusting for DB limitations in length, so that the logic comparison does not fail

                        //We need to get the IDs to make the mapping object. Query the VmOs and Region tables to get them.
                        var vmOsToMap = cmpWapVmOses.FirstOrDefault(x => string.Equals(vmOsName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                        var regionToMap = cmpWapRegions.FirstOrDefault(y => string.Equals(regionInCatalogue.RegionName.Replace(" ", string.Empty), y.Name.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase));
                        if (vmOsToMap == null || regionToMap == null)
                            continue; //something's wrong here if we reach this.

                        var newMapping = new AzureRegionVmOsMapping
                        {
                            AzureRegionId = regionToMap.AzureRegionId,
                            VmOsId = vmOsToMap.VmOsId,
                            IsActive = true
                        };

                        newMappings.Add(newMapping);
                    }
                }

                if (newMappings.Any())
                {
                    cmpWapDb.SetRegionVmOsMappingsByBatch(newMappings);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in UpdateRegionVmOsMappings: " + ex.ToString());
            }
        }

        #endregion

        //*********************************************************************
        ///
        /// <summary>
        /// Send an event to the eventlog
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(Exception ex, EventLogEntryType type, string prefix, int id, short category)
        {
            if (null != _eventLog)
                _eventLog.WriteEntry(prefix + " : " + CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
        }
    }
}

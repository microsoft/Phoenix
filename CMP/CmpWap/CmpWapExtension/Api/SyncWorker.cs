//*****************************************************************************
// File: SyncWorker.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that are used for syncing with 
//          an external service (CMP).
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
    /// an external service (CMP).
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
            //_syncWorker._azureThread.Start();
        }

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
                        Thread.Sleep(5000*retryCount);
                    else
                    {
                        LogThis(ex, EventLogEntryType.Error,
                            "Exception in SyncWorker.SyncWithAzure()", 0, 0);
                        Thread.Sleep(1000*60*5);
                    }
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

        //*********************************************************************
        ///
        /// <summary>
        /// Sync the CmpWapDb AzureRegion and VmSize tables with Azure domain data
        /// </summary>
        /// 
        //*********************************************************************
        private void SyncWithAzure()
        {
            IEnumerable<AzureLocationArmData> locationResult;
            IEnumerable<AzureVmSizeArmData> sizeResult;
            IEnumerable<AzureVmOsArmData> dataResult;

            AzureRefreshService ars = new AzureRefreshService(_eventLog, WebConfigurationManager.ConnectionStrings["CMPContext"].ConnectionString);
            ars.FetchAzureInformationWithArm(out locationResult, out sizeResult, out dataResult);

            UpdateAzureRegions(locationResult.ToList());
            UpdateVmSizes(sizeResult.ToList());
            UpdateVmOs(dataResult);
        }

        private static void UpdateAzureRegions(List<AzureLocationArmData> azureLocations)
        {
            try
            {
                CmpWapDb cmpWapDb = new CmpWapDb();
                List<AzureRegion> cmpRegions = cmpWapDb.FetchAzureRegionList(onlyActiveOnes: false);
                var azureRegions = azureLocations.Select(loc => new AzureRegion
                {
                    Name = loc.Name,
                    Description = loc.DisplayName,
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

        private void UpdateVmSizes(List<AzureVmSizeArmData> azureVmSizes)
        {
            try
            {
                CmpWapDb cmpWapDb = new CmpWapDb();
                List<VmSize> cmpWapVmSizes = cmpWapDb.FetchVmSizeInfoList(onlyActiveOnes: false).ToList();
                List<VmSize> newVmSizes = new List<VmSize>();

                foreach (AzureVmSizeArmData vmSize in azureVmSizes)
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

        private void UpdateVmOs(IEnumerable<AzureVmOsArmData> dataList)
        {
            try
            {
                CmpWapDb cmpWapDb = new CmpWapDb();
                List<VmOs> cmpVmOses = cmpWapDb.FetchOsInfoList(onlyActiveOnes: false).ToList();

                var azureVmOses = dataList.Select(data => new VmOs
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

                var newVmOses = azureVmOses.Where(azureItem => !cmpVmOses.Any(cmpItem => string.Equals(azureItem.Name, cmpItem.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();

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

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            try
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { var x = ex2.Message; }
        }
    }
}

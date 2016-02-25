using AzureAdminClientLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure;

namespace CmpServiceLib
{
    public class AzureRefreshService
    {
        private static string _cmpDbConnectionString;
        protected static EventLog EventLog;

        #region Class-specific variables

        private static string _targetPublisherString;
        private static string _targetRegionString;
        private static int _targetNumberOfPublishers;

        //Default values in case config settings are not present
        private readonly string _defaultPublisherString = "OpenLogic";
        private readonly string _defaultRegionString = "Europe";
        private readonly int _defaultNumberOfPublishers = 50;

        #endregion

        #region --- Setup Methods --------------------------------------------

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Constructor
        ///  </summary>
        ///  <param name="eLog"></param>
        ///  
        //*********************************************************************
        public AzureRefreshService(EventLog eLog)
        {
            InitVals(eLog, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        public AzureRefreshService(EventLog eLog, string cmpDbConnectionString)
        {
            InitVals(eLog, cmpDbConnectionString);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        private void InitVals(EventLog eLog, string cmpDbConnectionString)
        {
            EventLog = eLog;
            _cmpDbConnectionString = cmpDbConnectionString;
            _targetPublisherString = CloudConfigurationManager.GetSetting("AzureCatalogueSyncTargetPublisher");
            _targetRegionString = CloudConfigurationManager.GetSetting("AzureCatalogueSyncTargetRegion");

            if (!int.TryParse(CloudConfigurationManager.GetSetting("AzureCatalogueSyncTargetNumberOfPublishers"), out _targetNumberOfPublishers))
                _targetNumberOfPublishers = _defaultNumberOfPublishers;
            if (string.IsNullOrEmpty(_targetPublisherString))
                _targetPublisherString = _defaultPublisherString;
            if (string.IsNullOrEmpty(_targetRegionString))
                _targetRegionString = _defaultRegionString;
        }

        #endregion

        #region Class Methods

        //*********************************************************************
        ///
        /// <summary>
        /// Queries the CMP DB for subscritpions and their AAD creds. 
        /// Then it individually queries the API for each subscription
        /// to get AzureLocationArmData, AzureVmSizeArmData, and AzureVmOsData 
        /// objects
        /// </summary>
        /// <returns>An IEnumerables of AzureLocationArmData, AzureVmSizeArmData 
        /// and AzureVmOsArmData objects. </returns>
        /// 
        //*********************************************************************
        public IEnumerable<AzureCatalogue> FetchAzureInformationWithArm()
        {
            //Data Contracts
            var locationResult = Enumerable.Empty<AzureLocationArmData>(); //AzureLocationArmData is an Azure region, in accordance to Azure API
            var publisherList = Enumerable.Empty<AzurePublisher>();
            var offerList = Enumerable.Empty<AzureOffer>();

            //Result payload of all Azure calls to be sent back for processing
            var azureResultsList = new List<AzureCatalogue>();

            /*
             * Hierarchy of the API calls through ARM:
             * 
             * Azure subscription -> Collection of Locations (AKA Azure Regions)
             * Locations -> Collection of VmSizes and Publishers
             * Publisher -> Collection of Offers
             * Offer -> Collection of VM OSes
             * 
             */
            
            try
            {
                //Retrieving Service provider accounts and initializing a Connection object with each one, so we get appropriate AAD creds
                var serviceAccountList = ServProvAccount.GetAzureServiceAccountList(_cmpDbConnectionString).Where(sa => sa.AccountID != null);
                var subscriptionsList = serviceAccountList.Select(
                    sa => new AzureCatalogueOps(new Connection(
                        sa.AccountID, sa.CertificateThumbprint, sa.AzureADTenantId, sa.AzureADClientId, sa.AzureADClientKey)));

                //Block that starts getting the info for each subscription
                foreach (var subscription in subscriptionsList)
                {         
                    IList<AzureLocationArmData> azureLocationsList = subscription.GetAzureLocationsList().ToList();
                    locationResult = locationResult.Union(azureLocationsList, new LocationComparer());

                    //Section to limit amount of regions (to avoid pulling an obscene amount of data)
                    azureLocationsList = azureLocationsList.Where(loc => loc.Name.ToUpperInvariant().Contains(_targetRegionString.ToUpperInvariant()) && loc.DisplayName.ToUpperInvariant().Contains(_targetRegionString.ToUpperInvariant())).ToList();
                    //End section to limit amount of regions

                    //This loop starts the actual querying of Azure and getting the entire catalogue of offerings
                    foreach (var loc in azureLocationsList)
                    {
                        var azureCatalogueItem = new AzureCatalogue(loc.Id, loc.Name, loc.DisplayName, loc.Longitude, loc.Latitude);
                        var locationString = loc.Name.Replace(" ", string.Empty);
                        IList<AzureVmSizeArmData> azureVmSizeList = subscription.GetVmSizeList(locationString).ToList();
                        IList<AzurePublisher> azurePublishers = subscription.GetPublisherList(locationString).ToList();

                        azureCatalogueItem.VmSizes = azureCatalogueItem.VmSizes.Union(azureVmSizeList, new AzureVmSizeArmData.AzureVmSizeComparer());
                        publisherList = publisherList.Union(azurePublishers, new AzurePublisher.AzurePublisherComparer());

                        //Section to limit amount of publishers (to avoid pulling an obscene amount of data)
                        var pubCount = 0;
                        azurePublishers = azurePublishers.Where(ap => ap.name.Contains(_targetPublisherString)).ToList();
                        //End section to limit amount of publishers

                        foreach (var ap in azurePublishers)
                        {
                            var azurePublisherName = ap.name;
                            IList<AzureOffer> subOfferList = subscription.GetOfferList(locationString, ap.name).ToList();
                            offerList = offerList.Union(subOfferList, new AzureOffer.AzureOfferComparer());

                            foreach (var ao in subOfferList)
                            {
                                var azureOfferName = ao.name;
                                IList<AzureSku> azureSkusList = subscription.GetSkuList(locationString, ap.name, ao.name).ToList();

                                azureCatalogueItem.VmOses = azureSkusList.Select(sku => new AzureVmOsArmData
                                                {
                                                    Publisher = azurePublisherName,
                                                    Offer = azureOfferName,
                                                    SKU = sku.name
                                                }).Union(azureCatalogueItem.VmOses, new AzureVmOsArmData.AzureVmOsArmDataComparer());
                            }

                            //Limiting publishers to a constant to avoid major delays in the syncing process. Configurable.
                            if (pubCount++ >= _targetNumberOfPublishers) break;
                        }
                        
                        azureResultsList.Add(azureCatalogueItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                   "Exception in AzureRefreshService:FetchAzureInformationWithArm()", 1, 1);
                throw;
            }
            return azureResultsList;
        }

        #endregion

        #region Class Utilites

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            if (null == EventLog)
                return;

            if (null == ex)
                EventLog.WriteEntry(prefix, type, id, category);
            else
                EventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex),
                    type, id, category);
        }

        #endregion
    }
}

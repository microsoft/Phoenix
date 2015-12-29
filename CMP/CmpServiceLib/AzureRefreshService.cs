using AzureAdminClientLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CmpServiceLib
{
    public class AzureRefreshService
    {
        private static string _cmpDbConnectionString;
        protected static EventLog EventLog;

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
        public void FetchAzureInformationWithArm(out IEnumerable<AzureLocationArmData> locationResult, out IEnumerable<AzureVmSizeArmData> vmSizeResult, out IEnumerable<AzureVmOsArmData> vmOsResult)
        {
            locationResult = Enumerable.Empty<AzureLocationArmData>(); //AzureLocationArmData is an Azure region, in accordance to Azure API
            vmSizeResult = Enumerable.Empty<AzureVmSizeArmData>(); //AzureVmSizeArmData is a VM Size
            vmOsResult = new List<AzureVmOsArmData>(); //AzureVmOsArmData is a VM OS

            IEnumerable<AzurePublisher> publisherList = Enumerable.Empty<AzurePublisher>();
            IEnumerable<AzureOffer> offerList = Enumerable.Empty<AzureOffer>();

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
                    sa => new AzureRegionOps(new Connection(
                        sa.AccountID, sa.CertificateThumbprint, sa.AzureADTenantId, sa.AzureADClientId, sa.AzureADClientKey)));

                foreach (var subscription in subscriptionsList)
                {         
                    IList<AzureLocationArmData> azureLocationsList = subscription.GetAzureLocationsList().ToList();
                    locationResult = locationResult.Union(azureLocationsList, new LocationComparer());

                    //Debugging and test section
                    azureLocationsList = azureLocationsList.Where(loc => loc.Name.ToUpper().Contains("US")).ToList();
                    //End debugging and test section

                    foreach (var loc in azureLocationsList)
                    {
                        var locationString = loc.Name.Replace(" ", string.Empty);
                        IList<AzureVmSizeArmData> azureVmSizeList = subscription.GetVmSizeList(locationString).ToList();
                        IList<AzurePublisher> azurePublishers = subscription.GetPublisherList(locationString).ToList();

                        vmSizeResult = vmSizeResult.Union(azureVmSizeList, new AzureVmSizeArmData.AzureVmSizeComparer());
                        publisherList = publisherList.Union(azurePublishers, new AzurePublisher.AzurePublisherComparer());

                        //Debugging and test section
                        var pubCount = 0;
                        azurePublishers = azurePublishers.Where(ap => ap.name.Contains("MicrosoftWindowsServer")).ToList();
                        //End debugging and test section

                        foreach (var ap in azurePublishers)
                        {
                            var azurePublisherName = ap.name;
                            IList<AzureOffer> subOfferList = subscription.GetOfferList(locationString, ap.name).ToList();
                            offerList = offerList.Union(subOfferList, new AzureOffer.AzureOfferComparer());

                            foreach (var ao in subOfferList)
                            {
                                var azureOfferName = ao.name;
                                IList<AzureSku> azureSkusList = subscription.GetSKUList(locationString, ap.name, ao.name).ToList();

                                vmOsResult = azureSkusList.Select(sku => new AzureVmOsArmData
                                                {
                                                    Publisher = azurePublisherName,
                                                    Offer = azureOfferName,
                                                    SKU = sku.name
                                                }).Union(vmOsResult, new AzureVmOsArmData.AzureVmOsArmDataComparer());
                            }

                            //Testing for 50 publishers for now
                            pubCount++;
                            if (pubCount >= 50)
                            {
                                break;
                            }
                        }
                        //break; //Testing only 1 region for now
                    }
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                   "Exception in AzureRefreshService:FetchAzureInformationWithArm()", 1, 1);
                throw;
            }
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

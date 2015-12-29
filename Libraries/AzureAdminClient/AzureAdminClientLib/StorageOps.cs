//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CmpInterfaceModel;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using BlobListingDetails = Microsoft.WindowsAzure.StorageClient.BlobListingDetails;
using BlobRequestOptions = Microsoft.WindowsAzure.StorageClient.BlobRequestOptions;
using CloudBlobClient = Microsoft.WindowsAzure.StorageClient.CloudBlobClient;
using CloudBlobContainer = Microsoft.WindowsAzure.StorageClient.CloudBlobContainer;

namespace AzureAdminClientLib
{
    public class BlobAddressInfo
    {
        readonly string _containerName;
        readonly string _accountName;
        readonly string _blobName;

        public string ContainerName
        { get { return _containerName; } }
        public string AccountName
        { get { return _accountName; } }
        public string BlobName
        { get { return _blobName; } }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobUrl"></param>
        /// 
        //*********************************************************************

        public BlobAddressInfo(string blobUrl)
        {
            try
            {
                var index = blobUrl.IndexOf("//", StringComparison.Ordinal);
                if (-1 == index) throw new Exception("Malformed URL: Missing '//'");
                _containerName = blobUrl.Substring(index + 2);

                index = _containerName.IndexOf(".", StringComparison.Ordinal);
                if (-1 == index) throw new Exception("Malformed URL: Missing '.'");
                _accountName = _containerName.Substring(0, index);

                index = ContainerName.IndexOf("/", StringComparison.Ordinal);
                if (-1 == index) throw new Exception("Malformed URL: Missing first '/'");
                _containerName = _containerName.Substring(index + 1);

                index = _containerName.IndexOf("/", StringComparison.Ordinal);

                if (-1 < index)
                {
                    _blobName = _containerName.Substring(index + 1);
                    _containerName = _containerName.Substring(0, index);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BlobAddressInfo.BlobAddressInfo() : " + ex.Message);
            }
        }
    }

    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class StorageInfo
    {
        public string ServiceName;
        public string Description;
        public string LabelB64;
        public string Location;
        public string ReplicationEnabled;
        public string CreatedBy;
        public HostedServiceInfo ServiceInfo = null;

        public bool CreateHostedService = false;

        public string LabelText
        {
            set { LabelB64 = Util.ToB64(value); }
            get { return Util.FromB64(LabelB64); }
        }
    }

    public sealed class CopyState
    {
        public long? BytesCopied { get; internal set; }
        public DateTimeOffset? CompletionTime { get; internal set; }
        public string CopyId { get; internal set; }
        public Uri Source { get; internal set; }
        public string StatusDescription { get; internal set; }
        public long? TotalBytes { get; internal set; }
    }

    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class BlobTransferInfo
    {
        public Uri Url { get; set; }
        public string Key { get; set; }
        public FileInfo LocalFile { get; set; }
        
        [DefaultValue(0)]
        public int TransferThreadCount { get; set; }
        [DefaultValue(false)]
        public bool OverwriteDestination { get; set; }
        [DefaultValue(false)]
        public bool DeleteSource { get; set; }

        /// <summary></summary>
        public string AccountUrl
        {
            get
            {
                return Url.GetLeftPart(UriPartial.Authority);
            }
        }

        /// <summary></summary>
        public string Account
        {
            get
            {
                var accountUrl = AccountUrl;

                accountUrl = accountUrl.Substring(Url.GetLeftPart(UriPartial.Scheme).Length);
                accountUrl = accountUrl.Substring(0, accountUrl.IndexOf('.'));

                return accountUrl;
            }
        }

        /// <summary></summary>
        public string Container
        {
            get
            {
                var container = Url.PathAndQuery;
                container = container.Substring(1);
                container = container.Substring(0, container.IndexOf('/'));
                return container;
            }
        }

        /// <summary></summary>
        public string Blob
        {
            get
            {
                var blob = Url.PathAndQuery;
                blob = blob.Substring(1);
                blob = blob.Substring(blob.IndexOf('/') + 1);

                var queryOffset = blob.IndexOf('?');
                if (queryOffset != -1)
                {
                    blob = blob.Substring(0, queryOffset);
                }
                return blob;
            }
        }
    }

    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class StorageOps
    {
        const string URLTEMPLATE_GETSTORAGELIST = "https://management.core.windows.net/{0}/services/storageservices";
        const string URLTEMPLATE_CREATESTORE = "https://management.core.windows.net/{0}/services/storageservices";

        const string BODYTEMPLATE_CREATESTORE =
          @"<?xml version=""1.0"" encoding=""utf-8""?>
            <CreateStorageServiceInput xmlns=""http://schemas.microsoft.com/windowsazure"">
              <ServiceName>{ServiceName}</ServiceName>
              <Description>{Description}</Description>
              <Label>{LabelB64}</Label>
              <Location>{Location}</Location>
              <GeoReplicationEnabled>{ReplicationEnabled}</GeoReplicationEnabled>
              <ExtendedProperties>
                <ExtendedProperty>
                  <Name>AccountCreatedBy</Name>
                  <Value>{CreatedBy}</Value>
                </ExtendedProperty>
              </ExtendedProperties>
            </CreateStorageServiceInput>";

        private const int PageBlobPageSize = 512;
        private const int OneMegabyteAsBytes = 1024 * 1024;
        private const long FourMegabytesAsBytes = 4 * OneMegabyteAsBytes;
        private readonly Connection _connection;

        public Connection Connection 
        {
            get { return _connection; }
        }

        private XNamespace AzureNameSpace = "http://schemas.microsoft.com/windowsazure";
        public Exception _ContainerSpaceAzureSynchException = null;
        private static string _containerSpaceListAddLock = "x";
        private static ContainerSpaceInfo _containerSpaceInfo;
        public Dictionary<string, List<AzureVirtualNetwork>> VNetList = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public StorageOps(Connection connection)
        {
            _connection = connection;

            if(null == _containerSpaceInfo)
                _containerSpaceInfo = new ContainerSpaceInfo();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse GetStorageList()
        {
            var url = string.Format(URLTEMPLATE_GETSTORAGELIST, _connection.SubcriptionID);
            var hi = new HttpInterface(_connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="affinityGroupName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<AzureVirtualNetwork> GetVNetsInAffinityGroup(string affinityGroupName)
        {
            var vNetListOut = new List<AzureVirtualNetwork>();

            var currentSubscription = new AzureSubscription();

            var vNetList = currentSubscription.GetVirtualNetworks(
                _connection.SubcriptionID, _connection.CertThumbprint);

            foreach (var vNet in vNetList)
                if (null != vNet.AffinityGroup)
                    if (vNet.AffinityGroup.Equals(affinityGroupName, StringComparison.InvariantCultureIgnoreCase))
                        vNetListOut.Add(vNet);

            return vNetListOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<AzureVirtualNetwork> GetVNetsInLocation(string locationName)
        {
            var currentSubscription = new AzureSubscription();

            var vNetList = currentSubscription.GetVirtualNetworks(
                _connection.SubcriptionID, _connection.CertThumbprint);

            var vmList = currentSubscription.GetAllVirtualMachines(
                _connection.SubcriptionID, _connection.CertThumbprint);


            var vNetListOut = vNetList.Where(vNet => null != vNet.Location).Where(
                vNet => vNet.Location.Equals(locationName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            //**************

            foreach (var vNet in from vNet in vNetListOut from vM in vmList.Where(vM => string.Compare(vNet.Name, 
                vM.VirtualNetworkName, StringComparison.InvariantCultureIgnoreCase) == 0) select vNet)
                vNet.VirtualMachineCount++;

            //*************

            return vNetListOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageContainerUrl"></param>
        /// <param name="blobCount"></param>
        /// <param name="tagId"></param>
        /// <param name="tagObject"></param>
        /// 
        //*********************************************************************

        public void ReserveContainerSpace(string storageContainerUrl,
            int blobCount, int tagId, object tagObject)
        {
            ContainerSpaceInfo.Reserve(storageContainerUrl,
                blobCount, tagId, tagObject);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageContainerUrl"></param>
        /// <param name="blobCount"></param>
        /// <param name="tagId"></param>
        /// <param name="tagObject"></param>
        /// 
        //*********************************************************************

        public void FreeContainerSpace(string storageContainerUrl,
            int blobCount, int tagId, object tagObject)
        {
            ContainerSpaceInfo.Free(storageContainerUrl,
                blobCount, tagId, tagObject);
        }

        public enum SynchType { Reset, Update }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="containerName"></param>
        /// <param name="synchType"></param>
        /// <param name="requireAffinityGroup"></param>
        ///  
        //*********************************************************************

        public IEnumerable<ContainerSpaceInfo> SynchContainerSpace(
            string containerName, SynchType synchType, bool requireAffinityGroup)
        {
            switch (synchType)
            {
                case SynchType.Reset:
                    return ContainerSpaceInfo.SychWithAzure(
                        this, containerName, ContainerSpaceInfo.SynchType.Reset, requireAffinityGroup);
                    break;
                case SynchType.Update:
                    return ContainerSpaceInfo.SychWithAzure(
                        this, containerName, ContainerSpaceInfo.SynchType.Update, requireAffinityGroup);
                    break;
            }
          
            return null;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="solist"></param>
        /// <param name="containerName"></param>
        /// <param name="synchType"></param>
        /// <param name="requireAffinityGroup"></param>
        /// <param name="exceptionMessageList"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public static IEnumerable<ContainerSpaceInfo> SynchContainerSpace(
            List<StorageOps> solist, string containerName, 
            SynchType synchType, bool requireAffinityGroup, out string exceptionMessageList)
        {
            exceptionMessageList = null;

            switch (synchType)
            {
                case SynchType.Reset:
                    return ContainerSpaceInfo.SychWithAzure(
                        solist, ContainerSpaceInfo.SynchType.Reset, containerName, requireAffinityGroup, out exceptionMessageList);
                    break;
                case SynchType.Update:
                    return ContainerSpaceInfo.SychWithAzure(
                        solist, ContainerSpaceInfo.SynchType.Update, containerName, requireAffinityGroup, out exceptionMessageList);
                    break;
            }
            
            return null;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vNetList"></param>
        /// <param name="vmLimit"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private bool IsAnyVnetUnderVmLimit(List<AzureVirtualNetwork> vNetList, int vmLimit)
        {
            return 0 == vmLimit || vNetList.Any(vNet => vNet.VirtualMachineCount < vmLimit);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="containerName"></param>
        /// <param name="affinityGroupName"></param>
        /// <param name="location"></param>
        /// <param name="blobCountUpperLimit"></param>
        /// <param name="vmsPerVnetLimit"></param>
        /// <param name="blobCountNeeded"></param>
        /// <param name="enforceAppAgAffinity"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        //*** NOTE * Compute

        public AzureStorageAccountContainer GetLeastUsedContainer(
            string containerName, string affinityGroupName, string location,
            int blobCountUpperLimit, int vmsPerVnetLimit, 
            int blobCountNeeded, bool enforceAppAgAffinity)
        {
            try
            {
                var containerSpaceList = ContainerSpaceInfo.GetContainerSpaces(this);
                var agVnetLists = new Dictionary<string, List<AzureVirtualNetwork>>();
                var locationVnetLists = new Dictionary<string, List<AzureVirtualNetwork>>();
                List<AzureVirtualNetwork> vNetList;

                ContainerSpaceInfo leastUsedContainerSpace = null;

                foreach (var containerSpace in containerSpaceList)
                {
                    //*** Hopefully there are very few of these. We need to count and report at population time
                    if (null == containerSpace.AzureStorageAccount)
                        continue;

                    //*** Hopefully there are very few of these. We need to count and report at population time
                    if (!containerSpace.AzureStorageAccountContainer.Resolved)
                        continue;

                    if (null != containerName)
                        if (!containerName.Equals(containerSpace.AzureStorageAccountContainer.Name,
                            StringComparison.InvariantCultureIgnoreCase))
                            continue;

                    //** TODO * Remove after affinity groups are removed from subscriptions
                    //if (null == containerSpace.AzureStorageAccount.AffinityGroup)
                    //    continue;

                    if (!enforceAppAgAffinity)
                    {
                        if (null == containerSpace.AzureStorageAccount.Location)
                            continue;

                        //*** If location is specified, then skip this storage account if location not matched
                        if (null != location)
                            if (0 < location.Length)
                                if (!containerSpace.AzureStorageAccount.Location.Equals(location,StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                        if (locationVnetLists.TryGetValue(containerSpace.AzureStorageAccount.Location, out vNetList))
                            containerSpace.AzureStorageAccount.VirtualNetworksAvailable = vNetList;
                        else
                        {
                            containerSpace.AzureStorageAccount.VirtualNetworksAvailable = 
                                GetVNetsInLocation(containerSpace.AzureStorageAccount.Location);

                            locationVnetLists.Add(containerSpace.AzureStorageAccount.Location,
                                containerSpace.AzureStorageAccount.VirtualNetworksAvailable);
                        }
                    }
                    else
                    {
                        if (null == containerSpace.AzureStorageAccount.AffinityGroup)
                            continue;

                        if (null != affinityGroupName)
                            if (!affinityGroupName.Equals(containerSpace.AzureStorageAccount.AffinityGroup,
                                StringComparison.InvariantCultureIgnoreCase))
                                continue;

                        if (agVnetLists.TryGetValue(containerSpace.AzureStorageAccount.AffinityGroup, out vNetList))
                            containerSpace.AzureStorageAccount.VirtualNetworksAvailable = vNetList;
                        else
                        {
                            containerSpace.AzureStorageAccount.VirtualNetworksAvailable =
                                GetVNetsInAffinityGroup(containerSpace.AzureStorageAccount.AffinityGroup);

                            agVnetLists.Add(containerSpace.AzureStorageAccount.AffinityGroup,
                                containerSpace.AzureStorageAccount.VirtualNetworksAvailable);
                        }
                    }

                    if (null == containerSpace.AzureStorageAccount.VirtualNetworksAvailable)
                        continue;

                    if (0 == containerSpace.AzureStorageAccount.VirtualNetworksAvailable.Count)
                        continue;

                    if ( !IsAnyVnetUnderVmLimit(containerSpace.AzureStorageAccount.VirtualNetworksAvailable,
                            vmsPerVnetLimit))
                        continue;

                    containerSpace.AzureStorageAccountContainer.StorageAccount = containerSpace.AzureStorageAccount;

                    if (0 == containerSpace.BlobCount)
                    {
                        return containerSpace.AzureStorageAccountContainer;
                    }

                    if (null == leastUsedContainerSpace)
                        leastUsedContainerSpace = containerSpace;
                    else
                    {
                        if (containerSpace.BlobCount < leastUsedContainerSpace.BlobCount)
                            leastUsedContainerSpace = containerSpace;
                    }
                }

                if (null == leastUsedContainerSpace)
                    return null;

                return leastUsedContainerSpace.AzureStorageAccountContainer;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetLeastUsedContainer() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="containerUrl"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        //*** NOTE * Compute

        public AzureStorageAccountContainer GetContainer(string containerUrl )
        {
            //*** Search local cache first **************

            var containerSpaceList = ContainerSpaceInfo.GetContainerSpaces(this);

            if (null != containerSpaceList)
            foreach (var containerSpace in containerSpaceList.Where(containerSpace => containerUrl.Equals(containerSpace.AzureStorageAccountContainer.Url,
                StringComparison.InvariantCultureIgnoreCase)))
            {
                return containerSpace.AzureStorageAccountContainer;
            }

            //*** Now search Azure **************

            var storageAccountList = GetStorageDetails();

            foreach (var sa in storageAccountList)
                if (null != sa.Containers)
                    foreach (var container in sa.Containers)
                        if (containerUrl.Equals(container.Url, StringComparison.InvariantCultureIgnoreCase))
                        {
                            container.StorageAccount = sa;

                            container.StorageAccount.VirtualNetworksAvailable =
                                GetVNetsInAffinityGroup(sa.AffinityGroup);
                            
                            return container;
                        }

            return null;
        }

        public List<AzureStorageAccount> StorageAccountList
        { get { return _StorageAccountList;  } }

        private List<AzureStorageAccount> _StorageAccountList = null;

        //*********************************************************************
        ////
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //*** NOTE * Compute

        public List<AzureStorageAccount> GetStorageDetails()
        {
            try
            {
                var currentSubscription = new AzureSubscription();

                currentSubscription.LoadStorageAccounts(
                    _connection.SubcriptionID, _connection.CertThumbprint);

                _StorageAccountList = currentSubscription.StorageAccounts;

                if (null == _StorageAccountList)
                    throw new Exception("No accessible storage accounts found");

                if (!_StorageAccountList.Any())
                    throw new Exception("No accessible storage accounts found");

                var bld = Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails.All;

                foreach (var storageAccount in _StorageAccountList)
                {
                    if (null == storageAccount.Containers)
                        continue;

                    var blobClient = GetBlobClient2(storageAccount.Name,
                        storageAccount.PrimaryAccessKey);

                    foreach (var container in storageAccount.Containers)
                    {
                        for (var x = 0; x < 3; x++)
                        {
                            try
                            {
                                var containerRef = blobClient.GetContainerReference(container.Name);

                                if (null != containerRef)
                                {
                                    var blobList = containerRef.ListBlobs(null, true, bld, null);

                                    if (null != blobList)
                                        if (blobList.Any())
                                            container.ObjectCount = blobList.Count();

                                    container.StorageAccount.VirtualNetworksAvailable =
                                        GetVNetsInAffinityGroup(storageAccount.AffinityGroup);

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.Message;
                            }
                        }
                    }
                }

                return _StorageAccountList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetStorageDetails() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static CloudBlobClient GetBlobClient(string storageName, string privateKey)
        {
            var connectionString = String.Format(
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageName, privateKey);

            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();

            blobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);

            return blobClient;
        }

        private static Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient GetBlobClient2(string storageName, string privateKey)
        {
            var connectionString = String.Format(
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageName, privateKey);

            var account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();

            blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.Zero, 4);

            return blobClient;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public CloudBlob CreateSnapshot(string accountName, string containerName, 
            string blobName, string privateKey)
        {
            try
            {
                var blobClient = GetBlobClient(accountName, privateKey);
                var container = blobClient.GetContainerReference(containerName);
                var blob = container.GetBlobReference(blobName);

                blob.Metadata.Add("first", "1");
                blob.SetMetadata();

                var snapshot = blob.CreateSnapshot();

                return snapshot;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return null;
                }

                throw new Exception("Exception in CreateSnapshot() : " + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// When you create a snapshot, the Blob service returns a DateTime value 
        /// that uniquely identifies the snapshot relative to its base blob. You 
        /// can use this value to perform further operations on the snapshot. Note 
        /// that you should treat this DateTime value as opaque. 
        ///
        /// The DateTime value identifies the snapshot on the URI. For example, a 
        /// base blob and its snapshots have URIs similar to the following:
        /// Base blob: http://myaccount.blob.core.windows.net/mycontainer/myblob 
        ///
        /// Snapshot: http://myaccount.blob.core.windows.net/mycontainer/myblob?snapshot=<DateTime>
        /// SnapshotTime	{10/18/2013 4:23:15 PM}	System.DateTime
        /// </summary>
        /// <param name="blobUrl"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse CreateSnapshot(string blobUrl, 
            string privateKey)
        {
            var resp = new HttpResponse();
            var bai = new BlobAddressInfo(blobUrl);

            var snap = CreateSnapshot(bai.AccountName, bai.ContainerName, 
                bai.BlobName, privateKey);

            if (null != snap)
            {
                resp.StatusCheckUrl = Microsoft.WindowsAzure.StorageClient.Protocol.BlobRequest.Get
                    (snap.Uri, 0, snap.SnapshotTime.Value, null).Address.AbsoluteUri;

                resp.HadError = false;
                resp.Body = "Snapshot Created";
            }
            else
            {
                resp.HadError = true;
                resp.Body = "Snapshot Not Created";
            }

            return resp;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="blobUrl"></param>
        /// <param name="privateKey"></param>
        /// <param name="blobList"></param>
        ///  
        //*********************************************************************

        public static AzureAdminClientLib.HttpResponse ListBlobsAndSnapshots(
            string blobUrl, string privateKey, out IEnumerable<IListBlobItem> blobList )
        {
            try
            {
                if (null == blobUrl)
                    throw new Exception("blobUrl = null");

                if (null == privateKey)
                    throw new Exception("privateKey = null");

                var resp = new HttpResponse();
                var bai = new BlobAddressInfo(blobUrl);

                var blobClient = GetBlobClient(bai.AccountName, privateKey);

                // Get a reference to the container.
                var container = blobClient.GetContainerReference(bai.ContainerName);

                var options = new BlobRequestOptions
                {
                    UseFlatBlobListing = true,
                    BlobListingDetails = BlobListingDetails.All
                };

                blobList = container.ListBlobs(options);

                resp.Body = "Found " + blobList.Count().ToString() + " snapshots";

                string uri;

                foreach (CloudBlob blob in blobList)
                {
                    try
                    {
                        if (null == blob.SnapshotTime)
                            uri = blob.Uri.AbsoluteUri;
                        else
                            uri = Microsoft.WindowsAzure.StorageClient.Protocol.BlobRequest.Get
                                (blob.Uri, 0, blob.SnapshotTime.Value, null).Address.AbsoluteUri;

                        resp.Body += "\r\n" + uri.ToString();
                    }
                    catch (Exception ex)
                    {
                        resp.Body += "\r\n" + ex.Message;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ListBlobsAndSnapshots() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //public bool DoesBlobExist(BlobTransferInfo config, string blobUrl, string privateKey)
        public static bool DoesBlobExist(string blobUrl, string privateKey)
        {
            //*** just return true for now ***
            return true;
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageInfo"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse CreateStorage(StorageInfo storageInfo)
        {
            //***

            if (storageInfo.CreateHostedService)
            {
                var hso = new HostedServiceOps(_connection);
                var resp = hso.CreateHostedService(storageInfo.ServiceInfo);
            }

            //***

            var url = string.Format(URLTEMPLATE_CREATESTORE, _connection.SubcriptionID);
            var body = string.Copy(BODYTEMPLATE_CREATESTORE);

            body = body.Replace("{ServiceName}", storageInfo.ServiceName);
            body = body.Replace("{Description}", storageInfo.Description);
            body = body.Replace("{LabelB64}", storageInfo.LabelB64);
            body = body.Replace("{Location}", storageInfo.Location);
            body = body.Replace("{ReplicationEnabled}", storageInfo.ReplicationEnabled);
            body = body.Replace("{CreatedBy}", storageInfo.CreatedBy);

            var hi = new HttpInterface(_connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }

        private CloudBlobContainer GetContainer(BlobTransferInfo config)
        {
            var creds = new StorageCredentialsAccountAndKey(config.Account, config.Key);
            var blobStorage = new CloudBlobClient(new Uri(config.AccountUrl), creds);
            var container = blobStorage.GetContainerReference(config.Container);

            return container;
        }

        public string GetBlobSasUri(CloudBlobContainer container, string blobName)
        {
            //Get a reference to a blob within the container.
            var blob = container.GetBlockBlobReference(blobName);

            //Set the expiry time and permissions for the blob.
            //In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            var sasConstraints = new SharedAccessPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.List
            };

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

        public string GetBlobSasUri(string blobUrl, string accessKey, double timoutHours)
        {
            var bti = new BlobTransferInfo { Url = new Uri(blobUrl), Key = accessKey };
            var container = GetContainer(bti);
            
            //Get a reference to a blob within the container.
            var blob = container.GetBlockBlobReference(bti.Blob);

            //Set the expiry time and permissions for the blob.
            //In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            var sasConstraints = new SharedAccessPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(timoutHours),
                Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.List
            };

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rangeOffset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool IsAllZero(byte[] range, long rangeOffset, long size)
        {
            for (long offset = 0; offset < size; offset++)
                if (range[rangeOffset + offset] != 0)
                    return false;

            return true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// 
        //*********************************************************************

        //*** NOTE * Network

        public AzureAdminClientLib.HttpResponse UploadFileToCloud(BlobTransferInfo config)
        {
            var resp = new HttpResponse();

            var creds = new
                   StorageCredentialsAccountAndKey(config.Account, config.Key);

            var blobStorage = new CloudBlobClient(config.AccountUrl, creds);
            var container = blobStorage.GetContainerReference(config.Container);

            try
            {
                container.CreateIfNotExist();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("Storage account exception: '{0}'. Please verify that Account: '{1}' and Key: '{2}' are valid",
                                    ex.Message, config.Account, config.Key ));
            }

            var pageBlob = container.GetPageBlobReference(config.Blob);
            resp.SourceSizeMBytes = config.LocalFile.Length / OneMegabyteAsBytes;

            var blobSize = RoundUpToPageBlobSize(config.LocalFile.Length);
            pageBlob.Create(blobSize);

            var stream = new FileStream(config.LocalFile.FullName, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(stream);

            long totalUploaded = 0;
            long vhdOffset = 0;
            var offsetToTransfer = -1;

            var startTime = DateTime.UtcNow;

            while (vhdOffset < config.LocalFile.Length)
            {
                var range = reader.ReadBytes((int)FourMegabytesAsBytes);

                var offsetInRange = 0;

                //*** Align page size ***
                if ((range.Length % PageBlobPageSize) > 0)
                {
                    var grow = (int)(PageBlobPageSize - (range.Length % PageBlobPageSize));
                    Array.Resize(ref range, range.Length + grow);
                }

                //*** Upload groups of contiguous non-zero page blob pages.  
                while (offsetInRange <= range.Length)
                {
                    if ((offsetInRange == range.Length) ||
                        IsAllZero(range, offsetInRange, PageBlobPageSize))
                    {
                        if (offsetToTransfer != -1)
                        {
                            //*** Transfer up to this point
                            var sizeToTransfer = offsetInRange - offsetToTransfer;
                            var memoryStream = new MemoryStream(range,
                                         offsetToTransfer, sizeToTransfer, false, false);
                            pageBlob.WritePages(memoryStream, vhdOffset + offsetToTransfer);
                            Console.WriteLine("Range ~" + Megabytes(offsetToTransfer + vhdOffset)
                                    + " + " + PrintSize(sizeToTransfer));
                            totalUploaded += sizeToTransfer;
                            offsetToTransfer = -1;
                        }
                    }
                    else
                    {
                        if (offsetToTransfer == -1)
                            offsetToTransfer = offsetInRange;
                    }
                    offsetInRange += PageBlobPageSize;
                }
                vhdOffset += range.Length;
            }

            var elapsedTime = DateTime.UtcNow.Subtract(startTime);
            resp.Body = "Uploaded " + Megabytes(totalUploaded) + " of " + Megabytes(blobSize) 
                + " in " + elapsedTime.TotalSeconds.ToString() + " seconds.";
            resp.MBytesTransferred = totalUploaded / OneMegabyteAsBytes;
            resp.ElapsedTimeMinutes = (int)elapsedTime.TotalMinutes;
            resp.Operation = "Success";
            resp.HTTP = "---";

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //*** NOTE * Network

        public AzureAdminClientLib.HttpResponse DownloadFileFromCloud(BlobTransferInfo config)
        {
            var resp = new HttpResponse();

            var creds =
                   new StorageCredentialsAccountAndKey(config.Account, config.Key);

            var blobStorage = new CloudBlobClient(config.AccountUrl, creds) {ReadAheadInBytes = 0};

            var container = blobStorage.GetContainerReference(config.Container);
            var pageBlob = container.GetPageBlobReference(config.Blob);

            //*** Get the length of the blob
            pageBlob.FetchAttributes();
            var sourceLength = pageBlob.Properties.Length;
            long totalDownloaded = 0;
            resp.SourceSizeMBytes = sourceLength / OneMegabyteAsBytes;

            //*** Create a new local file to write into
            var fileStream = new FileStream(config.LocalFile.FullName, FileMode.Create, FileAccess.Write);
            fileStream.SetLength(sourceLength);

            //*** Download the valid ranges of the blob, and write them to the file
            var pageRanges = pageBlob.GetPageRanges();
            var blobStream = pageBlob.OpenRead();

            foreach (var range in pageRanges)
            {
                //*** EndOffset is inclusive... so need to add 1
                var rangeSize = (long)(range.EndOffset + 1 - range.StartOffset);

                //*** Chop range into 4MB chucks, if needed
                for (long subOffset = 0; subOffset < rangeSize; subOffset += FourMegabytesAsBytes)
                {
                    var subRangeSize = Math.Min(rangeSize - subOffset, FourMegabytesAsBytes);
                    blobStream.Seek(range.StartOffset + subOffset, SeekOrigin.Begin);
                    fileStream.Seek(range.StartOffset + subOffset, SeekOrigin.Begin);

                    //Console.WriteLine("Range: ~" + Megabytes(range.StartOffset + subOffset)
                    //                  + " + " + PrintSize(subRangeSize));
                    var buffer = new byte[subRangeSize];

                    blobStream.Read(buffer, 0, (int)subRangeSize);
                    fileStream.Write(buffer, 0, (int)subRangeSize);
                    totalDownloaded += subRangeSize;
                }
            }

            resp.Body = "Downloaded " + Megabytes(totalDownloaded) + " of " + Megabytes(sourceLength);
            resp.Operation = "Success";
            resp.HTTP = "---";

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string PrintSize(long bytes)
        {
            if (bytes >= 1024*1024) return (bytes / 1024 / 1024).ToString() + " MB";
            if (bytes >= 1024) return (bytes / 1024).ToString() + " kb";
            return (bytes).ToString() + " bytes";
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string Megabytes(long bytes)
        {
            return (bytes / OneMegabyteAsBytes).ToString() + " MB";
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static long RoundUpToPageBlobSize(long size)
        {
            return (size + PageBlobPageSize - 1) & ~(PageBlobPageSize - 1);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="btiSource"></param>
        /// <param name="btiDestination"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.Response CloudCloudTransfer(
            BlobTransferInfo btiSource, BlobTransferInfo btiDestination)
        {
            AzureAdminClientLib.Response Resp;

            var so2 = new StorageOps2();
            Resp = so2.CloudCloudTransfer(btiSource, btiDestination, StorageOps2.TransferUnitEnum.Page);

            return Resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************
        public class ContainerSpaceInfo : IDisposable
        {
            private class ReservationInfo
            {
                public int TagId = -1;
                public object TagObject = null;
                public int BlobCount = 0;
                public ContainerSpaceInfo containerSpaceInfo = null;
            }

            private string _url = null;
            private AzureStorageAccount _azureStorageAccount = null;
            private AzureStorageAccountContainer _azureStorageAccountContainer = null;
            private int _blobCount = 0;
            public string _subscriptionId = null;
            private readonly List<ReservationInfo> _reservationInfoList = new List<ReservationInfo>();
            private static readonly Dictionary<int,object> _reservationInfoListStatic = new Dictionary<int, object>(256);
            private static List<ContainerSpaceInfo> _containerSpaceList;
            private bool _disposed = false;
            private bool _deconstructed = false;

            public int BlobCount 
            { get { return _blobCount; } }
            public AzureStorageAccount AzureStorageAccount 
            { get { return _azureStorageAccount; } }
            public AzureStorageAccountContainer AzureStorageAccountContainer 
            { get { return _azureStorageAccountContainer; } }


            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// 
            //*********************************************************************

            public void Dispose()
            {
                _disposed = true;
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// 
            //*********************************************************************

            ~ContainerSpaceInfo()
            {
                _deconstructed = true;
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="blobCount"></param>
            /// <param name="tagId"></param>
            /// <param name="tagObject"></param>
            /// 
            //*********************************************************************

            private void Reserve(int blobCount, int tagId, object tagObject)
            {
                try
                {
                    object foundObj = null;
                    if (_reservationInfoListStatic.TryGetValue(tagId, out foundObj))
                    {
                        var foundReservation = foundObj as ReservationInfo;

                        if (null != foundReservation)
                        {
                            if (foundReservation.TagId == tagId)
                                return;

                            foundReservation.containerSpaceInfo.Free(
                                foundReservation.BlobCount, foundReservation.TagId, null);
                        }
                    }

                    _blobCount += blobCount;

                    var ri = new ReservationInfo
                    {
                        BlobCount = blobCount,
                        TagId = tagId,
                        TagObject = tagObject,
                        containerSpaceInfo = this
                    };

                    _reservationInfoList.Add(ri);
                    _reservationInfoListStatic.Add(tagId, ri);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.Reserve() : " + 
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="blobCount"></param>
            /// <param name="tagId"></param>
            /// <param name="tagObject"></param>
            /// 
            //*********************************************************************

            private void Free(int blobCount, int tagId, object tagObject)
            {
                try
                {
                    _blobCount -= blobCount;
                    _reservationInfoListStatic.Remove(tagId);

                    if (-1 < tagId)
                        for (var index = 0; index < _reservationInfoList.Count; index++)
                            if (_reservationInfoList[index].TagId == tagId)
                                _reservationInfoList.Remove(_reservationInfoList[index]);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.Free() : " +
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="containerUrl"></param>
            /// <returns></returns>
            /// 
            //*********************************************************************
            private static ContainerSpaceInfo GetContainerSpace(string containerUrl)
            {
                if (null == _containerSpaceList)
                    return null;

                if (containerUrl == null)
                    throw new ArgumentNullException("containerUrl");

                for(var index = 0; index < _containerSpaceList.Count; index++)
                    if (_containerSpaceList[index]._url.Equals(
                        containerUrl, StringComparison.InvariantCultureIgnoreCase))
                        return _containerSpaceList[index];

                return null;
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="so"></param>
            /// <returns></returns>
            /// 
            //*********************************************************************

            //*** NOTE * Compute
            //*** NOTE * Network

            public static IEnumerable<ContainerSpaceInfo> GetContainerSpaces(StorageOps so)
            {
                try
                {
                    if (null == _containerSpaceList)
                        throw new Exception("_containerSpaceList has not been synchronized with Azure");

                    if(null == so)
                        return _containerSpaceList;

                    var csiOut = new List<ContainerSpaceInfo>(200);

                    lock (_containerSpaceListAddLock)
                    {
                        csiOut.AddRange(_containerSpaceList.Where(
                            csl => csl._subscriptionId.Equals(so._connection.SubcriptionID)));
                    }

                    return csiOut;

                }
                catch (Exception ex)
            {
                    if (null == so)
                        throw new Exception("Exception in ContainerSpaceInfo.GetContainerSpaces() : " + 
                            Utilities.UnwindExceptionMessages(ex));
                    
                    throw new Exception("Exception in ContainerSpaceInfo.GetContainerSpaces(" +
                        so._connection.SubcriptionID + ") : " + Utilities.UnwindExceptionMessages(ex));
                }
            }

            public Dictionary<string, List<AzureVirtualNetwork>> VNetList = null;

            //*****************************************************************
            /// 
            ///  <summary>
            ///  
            ///  </summary>
            ///  <param name="so"></param>
            /// <param name="containerName"></param>
            /// <param name="requireAffinityGroup"></param>
            /// <returns></returns>
            ///  
            //*****************************************************************

            //*** NOTE * Compute
            //*** NOTE * Network
            //*** NOTE * Refresh

            public static void SynchContainerSpaces(StorageOps so, string containerName, bool requireAffinityGroup)
            {
                try
                {
                    var storageAccountList = so.GetStorageDetails();

                    //*****************************

                    foreach(var sa in storageAccountList)
                    {
                        sa.Containers = AzureSubscription.GetAzureStorageAccountContainers(
                            sa.Name, sa.PrimaryAccessKey, containerName);

                        foreach (var cont in sa.Containers)
                            cont.StorageAccount = sa;
                    }

                    //*****************************

                    var agVnetLists = new Dictionary<string, List<AzureVirtualNetwork>>();
                    var locationVnetLists = new Dictionary<string, List<AzureVirtualNetwork>>();

                    List<AzureVirtualNetwork> vNetList = null;

                    foreach (var sa in storageAccountList)
                    {
                        if (requireAffinityGroup)
                            if (null == sa.AffinityGroup)
                                continue;

                        var retryCount = 1;

                        while (true)
                        {
                            try
                            {
                                if (null == sa.AffinityGroup)
                                {
                                    if (locationVnetLists.TryGetValue(sa.Location, out vNetList))
                                    {
                                        sa.VirtualNetworksAvailable = vNetList;
                                    }
                                    else
                                    {
                                        sa.VirtualNetworksAvailable =
                                            so.GetVNetsInLocation(sa.Location);

                                        locationVnetLists.Add(sa.Location,
                                            sa.VirtualNetworksAvailable);

                                        //**************

                                        if (null == so.VNetList)
                                            so.VNetList = new Dictionary<string, List<AzureVirtualNetwork>>();

                                        so.VNetList.Add(sa.Location, sa.VirtualNetworksAvailable);

                                        //***************
                                    }
                                }
                                else
                                {
                                    if (agVnetLists.TryGetValue(sa.AffinityGroup, out vNetList))
                                    {
                                        sa.VirtualNetworksAvailable = vNetList;

                                        if (null == so.VNetList)
                                            so.VNetList = new Dictionary<string, List<AzureVirtualNetwork>>();
                                        //so.VNetList.Add(sa.AffinityGroup, vNetList);
                                    }
                                    else
                                    {
                                        sa.VirtualNetworksAvailable =
                                            so.GetVNetsInAffinityGroup(sa.AffinityGroup);

                                        agVnetLists.Add(sa.AffinityGroup,
                                            sa.VirtualNetworksAvailable);
                                    }
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("503") & (5 > retryCount++))
                                    Thread.Sleep((int)(1000 * Math.Pow(4.17, retryCount)));
                                else if (3 > retryCount++)
                                    Thread.Sleep(5000 * retryCount);
                                else
                                    break;
                            }
                        }
                    }

                    //*****************************

                    var bld = Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails.All;

                    foreach (var sa in storageAccountList)
                    {
                        //*** No AG = no VNet, for now
                        if (requireAffinityGroup)
                            if (null == sa.AffinityGroup)
                                continue;

                        var blobClient = GetBlobClient2(sa.Name,
                            sa.PrimaryAccessKey);

                        foreach (var container in sa.Containers)
                        {
                            var retryCount = 1;

                            while (true)
                            {
                                try
                                {
                                    var containerRef = blobClient.GetContainerReference(container.Name);

                                    if (null != containerRef)
                                    {
                                        var blobList = containerRef.ListBlobs(null, true, bld, null);

                                        if (null != blobList)
                                            if (blobList.Any())
                                                container.ObjectCount = blobList.Count();

                                        container.Resolved = true;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message.Contains("503") & (5 > retryCount++))
                                        Thread.Sleep((int)(1000 * Math.Pow(4.17, retryCount)));
                                    else if (3 > retryCount++)
                                        Thread.Sleep(5000*retryCount);
                                    else
                                    {
                                        container.Exception = new Exception(string.Format(
                                            "Unable to resolve({0}) : {1}", container.Url, 
                                            Utilities.UnwindExceptionMessages(ex)),ex);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //*****************************

                    if (null == _containerSpaceList)
                        _containerSpaceList = new List<ContainerSpaceInfo>(storageAccountList.Count);

                    foreach (var storageAccount in storageAccountList)
                    {
                        if (requireAffinityGroup & null == storageAccount.AffinityGroup)
                            continue;

                        if (null == storageAccount.Containers)
                            continue;

                        foreach (var container in storageAccount.Containers)
                           if (null == GetContainerSpace(container.Url))
                               //lock (_containerSpaceListAddLock)
                               //{
                               for (var index = 0; index < 4; index++)
                               {
                                   try
                                   {
                                       _containerSpaceList.Add(new ContainerSpaceInfo
                                       {
                                           _azureStorageAccount = storageAccount,
                                           _azureStorageAccountContainer = container,
                                           _blobCount = container.ObjectCount,
                                           _url = container.Url,
                                           _subscriptionId = so._connection.SubcriptionID
                                       });
                                            
                                       break;
                                   }
                                   catch (Exception ex)
                                   {
                                       if (ex.Message.Contains("503") & (4 > index++))
                                           Thread.Sleep((int) (1000*Math.Pow(4.17, index + 1)));
                                       else if (index > 2)
                                           throw;
                                   }
                               }
                        //}
                    }
                    //return _containerSpaceList;
                }
                catch (Exception ex)
                {
                    if(null == so)
                        throw new Exception("Exception in ContainerSpaceInfo.SynchContainerSpaces() : " +
                            Utilities.UnwindExceptionMessages(ex));

                    throw new Exception("Exception in ContainerSpaceInfo.SynchContainerSpaces(" + 
                        so._connection.SubcriptionID + ") : " + Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*****************************************************************
            /// 
            ///  <summary>
            ///  
            ///  </summary>
            ///  <param name="so"></param>
            /// <param name="containerName"></param>
            /// <param name="requireAffinityGroup"></param>
            ///  
            //*****************************************************************

            private static void SynchContainerSpacesTask(StorageOps so,
                string containerName, bool requireAffinityGroup)
            {
                try
                {
                    if (null == so)
                    {
                        //* TODO * Signal a failure to resolve as StorageOps
                        return;
                    }

                    SynchContainerSpaces(so, containerName, requireAffinityGroup);
                }
                catch (Exception ex)
                {
                    if (null != so)
                        so._ContainerSpaceAzureSynchException = ex;
                }
            }

            //*****************************************************************
            /// 
            ///  <summary>
            ///  
            ///  </summary>
            ///  <param name="soList"></param>
            /// <param name="containerName"></param>
            /// <param name="requireAffinityGroup"></param>
            /// <param name="exceptionMessageList"></param>
            /// <returns></returns>
            ///  
            //*****************************************************************

            //*** NOTE * Compute
            //*** NOTE * Network
            //*** NOTE * Refresh

            private static List<ContainerSpaceInfo> SynchContainerSpaces(
                IEnumerable<StorageOps> soList, string containerName, 
                bool requireAffinityGroup, out string exceptionMessageList)
            {
                //string exceptionMessageList = null;
                exceptionMessageList = null;

                try
                {
                    var taskList = new Task[soList.Count()];
                    var soArray = soList.ToArray();

                    for(var index = 0; index < soList.Count(); index++  )
                    {
                        var index1 = index;
                        taskList[index1] = Task.Factory.StartNew(() =>
                            SynchContainerSpacesTask(soArray[index1], containerName, requireAffinityGroup));
                    }

                    Task.WaitAll(taskList);

                    foreach (var so in soList)
                    {
                        if (null != so._ContainerSpaceAzureSynchException)
                        {
                            if (null == exceptionMessageList)
                                exceptionMessageList = so._ContainerSpaceAzureSynchException.Message;
                            else
                                exceptionMessageList += " --- " +
                                    so._ContainerSpaceAzureSynchException.Message;
                        }
                    }

                    if (null != _containerSpaceList)
                        foreach (var cs in _containerSpaceList)
                        {
                            if (null != cs._azureStorageAccountContainer)
                                if(null != cs.AzureStorageAccountContainer.Exception)
                                {
                                    if (null == exceptionMessageList)
                                        exceptionMessageList = cs.AzureStorageAccountContainer.Exception.Message;
                                    else
                                        exceptionMessageList += " --- " +
                                            cs.AzureStorageAccountContainer.Exception.Message;
                                }
                        }

                    //if (null != exceptionMessageList)
                    //    throw new Exception(exceptionMessageList);

                    return _containerSpaceList;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in SynchContainerSpaces() : " + 
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            public enum SynchType
            {
                Update,
                Reset
            };

            //*********************************************************************
            /// 
            ///  <summary>
            ///  
            ///  </summary>
            ///  <param name="so"></param>
            /// <param name="containerName"></param>
            /// <param name="synchType"></param>
            /// <param name="requireAffinityGroup"></param>
            ///  
            //*********************************************************************

            public static IEnumerable<ContainerSpaceInfo> SychWithAzure(
                StorageOps so, string containerName, SynchType synchType, bool requireAffinityGroup)
            {
                try
                {
                    if (synchType == SynchType.Reset)
                        _containerSpaceList.Clear();

                    SynchContainerSpaces(so, containerName, requireAffinityGroup);

                    return _containerSpaceList;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.SychWithAzure() : " +
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*********************************************************************
            /// 
            ///  <summary>
            ///  
            ///  </summary>
            ///  <param name="soList"></param>
            ///  <param name="synchType"></param>
            /// <param name="containerName"></param>
            /// <param name="requireAffinityGroup"></param>
            /// <param name="exceptionMessageList"></param>
            ///  
            //*********************************************************************

            public static IEnumerable<ContainerSpaceInfo> SychWithAzure(
                IEnumerable<StorageOps> soList, SynchType synchType, 
                string containerName, bool requireAffinityGroup, out string exceptionMessageList)
            {
                exceptionMessageList = null;

                try
                {
                    if (synchType == SynchType.Reset)
                        if (null != _containerSpaceList)
                            _containerSpaceList.Clear();

                    var spaceList = SynchContainerSpaces(soList, 
                        containerName, requireAffinityGroup, out exceptionMessageList);

                    return _containerSpaceList;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.SychWithAzure() : " +
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="containerUrl"></param>
            /// <param name="blobCount"></param>
            /// <param name="tagId"></param>
            /// <param name="tagObject"></param>
            /// 
            //*********************************************************************

            public static void Reserve(string containerUrl, int blobCount, 
                int tagId, object tagObject)
            {
                try
                {
                    var cSpace = GetContainerSpace(containerUrl);

                    if( null == cSpace)
                        throw new Exception(string.Format(
                            "Container '{0}' not found in subscription", containerUrl));

                    cSpace.Reserve(blobCount, tagId, tagObject);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.Reserve() : " + 
                        Utilities.UnwindExceptionMessages(ex));
                }
            }

            //*********************************************************************
            ///
            /// <summary>
            /// 
            /// </summary>
            /// <param name="containerUrl"></param>
            /// <param name="blobCount"></param>
            /// <param name="tagId"></param>
            /// <param name="tagObject"></param>
            /// 
            //*********************************************************************

            public static void Free(string containerUrl, int blobCount, 
                int tagId, object tagObject)
            {
                try
                {
                    var cSpace = GetContainerSpace(containerUrl);

                    if (null == cSpace)
                        return;

                    cSpace.Free(blobCount, tagId, tagObject);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in ContainerSpaceInfo.Free() : " + 
                        Utilities.UnwindExceptionMessages(ex));
                }
            }
        }
    }
}

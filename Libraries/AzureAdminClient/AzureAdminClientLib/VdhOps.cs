namespace AzureAdminClientLib
{
    public class VhdInfo
    {
        public string ID;
        public string HostCaching;
        public string DiskLabel;            
        public string DiskName;
        public int Lun;
        public int LogicalDiskSizeInGB = 0;
        public string MediaLink;
        public string SourceMediaLink;
    }

    // http://www.windowsazure.com/en-us/manage/windows/how-to-guides/attach-a-disk/ 

    public class VdhOps
    {
        const string URLTEMPLATE_ATTACHDISK = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments/{2}/roles/{3}/DataDisks";

        const string BODYTEMPLATE_ATTACHBLOB =
          @"<?xml version=""1.0"" encoding=""utf-8""?><DataVirtualHardDisk xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
          <HostCaching>ReadOnly</HostCaching>
          <DiskLabel>{DiskLabel}</DiskLabel>
          <Lun>{Lun}</Lun>
          <SourceMediaLink>{SourceMediaLink}</SourceMediaLink>
          </DataVirtualHardDisk>";

        const string BODYTEMPLATE_ATTACHDISK =
          @"<?xml version=""1.0"" encoding=""utf-8""?><DataVirtualHardDisk xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
          <HostCaching>ReadOnly</HostCaching>
          <DiskLabel>{DiskLabel}</DiskLabel>
          <Lun>{Lun}</Lun>
          <LogicalDiskSizeInGB>{LogicalDiskSizeInGB}</LogicalDiskSizeInGB>
          <DiskName>{DiskName}</DiskName>
          <MediaLink>{MediaLink}</MediaLink>
          </DataVirtualHardDisk>";

        const string BODYTEMPLATE_ATTACHDISKFULL =
          @"<?xml version=""1.0"" encoding=""utf-8""?><DataVirtualHardDisk xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
          <HostCaching>ReadOnly</HostCaching>
          <DiskLabel>{DiskLabel}</DiskLabel>
          <DiskName>{DiskName}</DiskName> 
          <Lun>{Lun}</Lun>
          <LogicalDiskSizeInGB>{LogicalDiskSizeInGB}</LogicalDiskSizeInGB>
          <MediaLink>{MediaLink}</MediaLink>
          <SourceMediaLink>{SourceMediaLink}</SourceMediaLink>
          </DataVirtualHardDisk>";

        private Connection _Connection;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public VdhOps(Connection connection)
        {
            _Connection = connection;
        }

        public HttpResponse AttachDiskToVM(VhdInfo vhdInfo, string serviceName, string deploymentName, string roleName)
        {
            var url = string.Format(URLTEMPLATE_ATTACHDISK, _Connection.SubcriptionID, serviceName, deploymentName, roleName);
            string body = null;


            if (null != vhdInfo.SourceMediaLink)
            {
                body = string.Copy(BODYTEMPLATE_ATTACHBLOB);
                body = body.Replace("{DiskLabel}", vhdInfo.DiskLabel);
                body = body.Replace("{Lun}", vhdInfo.Lun.ToString());
                body = body.Replace("{SourceMediaLink}", vhdInfo.SourceMediaLink);
            }

            if (null != vhdInfo.DiskName)
            {
                body = string.Copy(BODYTEMPLATE_ATTACHDISK);
                body = body.Replace("{DiskLabel}", vhdInfo.DiskLabel);
                body = body.Replace("{Lun}", vhdInfo.Lun.ToString());
                body = body.Replace("{DiskName}", vhdInfo.DiskName);
                body = body.Replace("{MediaLink}", vhdInfo.MediaLink);
                body = body.Replace("{LogicalDiskSizeInGB}", "1");
            }

            var hi = new HttpInterface(_Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }
    }
}

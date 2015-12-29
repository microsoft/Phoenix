namespace AzureAdminClientLib
{
    public class CertOps
    {
        const string URLTEMPLATE_UPLOADSERVICECERT = "https://management.core.windows.net/{0}/services/hostedservices/{1}/certificates";
        const string BODYTEMPLATE_UPLOADSERVICECERT = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <CertificateFile xmlns=""http://schemas.microsoft.com/windowsazure"">
              <Data>{0}</Data>
              <CertificateFormat>pfx</CertificateFormat>
              <Password>{1}</Password>
            </CertificateFile>";

        private readonly Connection _connection;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public CertOps(Connection connection)
        {
            _connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="certRawX509"></param>
        /// <param name="certPasword"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse UploadServiceCert(string serviceName, 
            string certRawX509, string certPasword)
        {
            var url = string.Format(URLTEMPLATE_UPLOADSERVICECERT, 
                _connection.SubcriptionID, serviceName);
            var body = string.Format(BODYTEMPLATE_UPLOADSERVICECERT, 
                certRawX509, certPasword);

            var hi = new HttpInterface(_connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }
    }
}

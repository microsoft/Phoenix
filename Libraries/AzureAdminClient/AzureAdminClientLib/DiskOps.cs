using System;

namespace AzureAdminClientLib
{
    public class DiskInfo
    {
        public enum OsEnum { Windows, Linux, None };
        public OsEnum Os;
        public string Label;
        public string MediaLink;
        public string Name;
    }

    public class DiskOps
    {
        const string URLTEMPLATE_ADDDISK = "https://management.core.windows.net/{0}/services/disks";
        const string BODYTEMPLATE_ADDDISK =
            @"<Disk xmlns=""http://schemas.microsoft.com/windowsazure"">
            <Label>{label}</Label>
            <MediaLink>{MediaLink}</MediaLink>
            <Name>{DiskName}</Name>
            <OS>{OS}</OS>
            </Disk>";

        private readonly Connection _Connection = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public DiskOps(Connection connection)
        {
            _Connection = connection;
        }

        const string MAKEOSDISK_TEMPLATE = "<Disk xmlns=\"http://schemas.microsoft.com/windowsazure\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><OS>{0}</OS><Label>{1}</Label><MediaLink>{2}</MediaLink><Name>{3}</Name></Disk>";
        const string MAKEDATADISK_TEMPLATE = "<Disk xmlns=\"http://schemas.microsoft.com/windowsazure\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><Label>{0}</Label><MediaLink>{1}</MediaLink><Name>{2}</Name></Disk>";

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diskInfo"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse AddDiskToSubscription(DiskInfo diskInfo)
        {
            var resp = new HttpResponse();

            try
            {
                var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format("https://management.core.windows.net/{0}/services/disks", _Connection.SubcriptionID));
                req.Headers["x-ms-version"] = "2014-05-01";
                req.ClientCertificates.Add(_Connection.Certificate);

                System.Xml.Linq.XNamespace xmlns = "http://schemas.microsoft.com/windowsazure";

                string xml = null;

          //diskInfo.Name += "yyy";

                switch (diskInfo.Os)
                {
                    case DiskInfo.OsEnum.None:
                        xml = string.Format(MAKEDATADISK_TEMPLATE, diskInfo.Label, diskInfo.MediaLink, diskInfo.Name);

                        //****************************
                        //return new HttpResponse {HadError = false};
                        //****************************

                        break;

                    default:

                        //*******************
                        //diskInfo.Label = "aaatest1";
                        //diskInfo.Name = diskInfo.Label;
                        //**************************

                        xml = string.Format(MAKEOSDISK_TEMPLATE, diskInfo.Os.ToString(), diskInfo.Label, diskInfo.MediaLink, diskInfo.Name);
                        break;
                }

                var encoding = new System.Text.UTF8Encoding();
                var arr = encoding.GetBytes(xml);
                req.ContentLength = arr.Length;
                req.Method = "POST";
                req.ContentType = "application/xml";
                var dataStream = req.GetRequestStream();
                dataStream.Write(arr, 0, arr.Length);
                dataStream.Close();
                var response = (System.Net.HttpWebResponse)req.GetResponse();
                var respStream = response.GetResponseStream();

                if(null == respStream)
                    throw new Exception("Unable to create Stream from HttpWebResponse");

                var reader = new System.IO.StreamReader(respStream);
                var sResponse = reader.ReadToEnd();

                resp.HadError = false;
                resp.Body = sResponse;
            }
            catch (Exception ex)
            {
                resp.Body = ex.Message;

                if (ex.Message.Contains("409"))
                    resp.HadError = false;
                else
                    resp.HadError = true;
            }

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diskName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse DeleteDiskFromSubscription(string diskName)
        {
            var resp = new AzureAdminClientLib.HttpResponse();

            try
            {
                var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(
                    string.Format("https://management.core.windows.net/{0}/services/disks/{1}",
                    _Connection.SubcriptionID, diskName));
                req.Headers["x-ms-version"] = "2014-05-01";
                req.ClientCertificates.Add(_Connection.Certificate);

                System.Xml.Linq.XNamespace xmlns = "http://schemas.microsoft.com/windowsazure";

                req.Method = "DELETE";
                var response = (System.Net.HttpWebResponse)req.GetResponse();
                var respStream = response.GetResponseStream();

                if (null == respStream)
                    throw new Exception("Unable to create Stream from HttpWebResponse");

                var reader = new System.IO.StreamReader(respStream);
                var sResponse = reader.ReadToEnd();

                resp.HadError = false;
                resp.Body = sResponse;
            }
            catch (Exception ex)
            {
                resp.Body = ex.Message;

                if (ex.Message.Contains("409"))
                    resp.HadError = false;
                else
                    resp.HadError = true;
            }

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse ListDisksInSubscription()
        {
            var resp = new AzureAdminClientLib.HttpResponse();

            try
            {
                var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(
                    string.Format("https://management.core.windows.net/{0}/services/disks",
                    _Connection.SubcriptionID));

                req.Headers["x-ms-version"] = "2014-05-01";
                req.ClientCertificates.Add(_Connection.Certificate);

                System.Xml.Linq.XNamespace xmlns = "http://schemas.microsoft.com/windowsazure";

                req.Method = "GET";
                var response = (System.Net.HttpWebResponse)req.GetResponse();
                var respStream = response.GetResponseStream();

                if (null == respStream)
                    throw new Exception("Unable to create Stream from HttpWebResponse");

                var reader = new System.IO.StreamReader(respStream);
                var sResponse = reader.ReadToEnd();

                resp.HadError = false;
                resp.Body = sResponse;
            }
            catch (Exception ex)
            {
                resp.Body = ex.Message;

                if (ex.Message.Contains("409"))
                    resp.HadError = false;
                else
                    resp.HadError = true;
            }

            return resp;
        }
    }
}


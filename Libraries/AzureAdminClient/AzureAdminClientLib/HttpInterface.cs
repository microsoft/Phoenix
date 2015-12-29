//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//
//******************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;
using System.Text;
using CmpInterfaceModel;
using Newtonsoft.Json;

namespace AzureAdminClientLib
{
    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class HttpResponse
    {
        public bool HadError = false;
        public bool Retry = false;
        public string Operation;
        public string Body;
        public string HTTP;
        public string ProviderRequestState;
        public string StatusCheckUrl;
        public long SourceSizeMBytes = 0;
        public long MBytesTransferred = 0;
        public int ElapsedTimeMinutes = 0;
    }

    public class Response
    {
        public enum ResultStatusEnum {Unassigned,OK,EXCEPTION}
        public ResultStatusEnum ResultStatus = ResultStatusEnum.Unassigned;
        public string ExceptionMessage = null;
        public HttpResponse HttpResp = null;
        public string TaskId = null;
        //public cop
    }

    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public interface IHttpInterface
    {
        HttpResponse PerformRequest(HttpInterface.RequestType_Enum requestType, 
            string requestUrl, string bodyContents = null);

        HttpResponse PerformRequestArm(HttpInterface.RequestType_Enum requestType,
            string requestUrl, string bodyContents = null);
    }

    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class HttpInterface : IHttpInterface
    {
        public enum RequestType_Enum {UNDEF,GET,POST,DELETE,PUT};

        private DateTime ApiVersion { get; set; }
        private IConnection Connection { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public HttpInterface(IConnection connection)
        {
            ApiVersion = new DateTime(2014, 05, 01);
            Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string CleanRequestBody(string requestBody)
        {
            var Out = requestBody.Replace("xsi:type", "i:type");
            Out = Out.Replace(" />", "/>");

            return Out;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestUrl"></param>
        /// <param name="bodyContents"></param>
        /// <returns></returns>
        /// <remarks>
        /// MSDN: http://msdn.microsoft.com/en-us/library/windowsazure/ee460782.aspx
        /// </remarks>
        /// 
        //*********************************************************************

        public HttpResponse PerformRequest(RequestType_Enum requestType, 
            string requestUrl, string bodyContents = null)
        {
            var responseOut = new HttpResponse();

            try
            {
                var operationsResponse = string.Empty;
                var requestUri = new Uri(requestUrl);
                var request = WebRequest.Create(requestUri) as HttpWebRequest;

                if(null == request)
                    throw new Exception("Unable to create a HttpWebRequest for : " + requestUri);

                request.Headers.Add("x-ms-version", ApiVersion.ToString("yyyy-MM-dd"));

                request.Method = requestType.ToString();
                request.ContentType = "application/xml";

                if (requestType == RequestType_Enum.POST)
                {
                    var xDoc = new XmlDocument();
                    xDoc.Load(new StringReader(CleanRequestBody(bodyContents)));

                    var requestStream = request.GetRequestStream();
                    var streamWriter = new StreamWriter(requestStream, Encoding.UTF8);
                    xDoc.Save(streamWriter);

                    streamWriter.Close();
                    requestStream.Close();
                }

                var myCert = Connection.Certificate;
                request.ClientCertificates.Add(myCert);

                var response = (HttpWebResponse)request.GetResponse();
                var responseStatus = response.StatusCode;

                if (response.ContentLength > 0)
                {
                    var responseStream = response.GetResponseStream();

                    if (null == responseStream)
                        throw new Exception("Unable to create Stream from HttpWebResponse");

                    var reader = new StreamReader(responseStream);

                    var xDocResp = new XmlDocument();
                    xDocResp.Load(reader);

                    if (requestUrl.Contains("/operations/"))
                    {
                        var xn = xDocResp.SelectSingleNode("/*[local-name()='Operation' and namespace-uri()='http://schemas.microsoft.com/windowsazure']/*[local-name()='Status' and namespace-uri()='http://schemas.microsoft.com/windowsazure']");

                        if (null != xn)
                        {
                            operationsResponse = xn.InnerXml;
                            responseOut.Operation = operationsResponse;
                        }
                    }

                    responseOut.Body = Util.PrettyPrintXml(xDocResp);

                    responseStream.Close();

                    reader.Close();
                }
                else
                {
                    if (!requestUrl.Contains("/operations/"))
                        responseOut.Body = "No HTTP Response Body Returned.";
                }

                //*** https://management.core.windows.net/26661eda-81ca-4307-b331-698118ad668e/operations/7ee88cae5e15489789c829b28e7759f9

                if (!requestUrl.Contains("/operations/") && responseStatus == HttpStatusCode.Accepted)
                {
                    var responseHeaders = response.Headers;
                    var requestId = responseHeaders.Get("x-ms-request-id");
                    responseOut.StatusCheckUrl = "https://management.core.windows.net/" + 
                        Connection.SubcriptionID + "/operations/" + requestId;
                    responseOut.Body = "202 - Accepted. Asynch processing. Check status at: " + 
                        responseOut.StatusCheckUrl;
                }

                response.Close();
            }
            catch (WebException exWeb)
            {
                var responseStream = exWeb.Response.GetResponseStream();

                if (null == responseStream)
                {
                    responseOut.HadError = true;
                    responseOut.Body += "Error making REST API call.\r\n" + exWeb.Message + "\r\n";
                    return responseOut;
                }

                var reader = new StreamReader(responseStream);

                var xDocResp = new XmlDocument();
                xDocResp.Load(reader);

                var responseStatus = (HttpWebResponse)exWeb.Response;
                responseOut.HTTP = (int)responseStatus.StatusCode + " - " + 
                    responseStatus.StatusCode.ToString();
                responseOut.HadError = true;
                responseOut.Body += "Web Error making REST API call.\r\nMessage: " + 
                    exWeb.Message + "\r\nResponse:\r\n" + Util.PrettyPrintXml(xDocResp);

                responseStream.Close();

                reader.Close();
            }
            catch (Exception ex)
            {
                responseOut.HadError = true;
                responseOut.Body += "Error making REST API call.\r\n" + ex.Message + "\r\n";
            }

            return responseOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestUrl"></param>
        /// <param name="bodyContents"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse PerformRequestArm(RequestType_Enum requestType,
            string requestUrl, string bodyContents = null)
        {
            var responseOut = new HttpResponse();

            try
            {
                var requestUri = new Uri(requestUrl);
                var request = WebRequest.Create(requestUri) as HttpWebRequest;

                if (null == request)
                    throw new Exception("Unable to create a HttpWebRequest for : " + requestUri);

                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Connection.AdToken.AccessToken);

                request.Method = requestType.ToString();
                request.ContentType = "application/json";
                request.Accept = "application/json";

                if (requestType == RequestType_Enum.POST | requestType == RequestType_Enum.PUT)
                {
                    var requestStream = request.GetRequestStream();
                    var streamWriter = new StreamWriter(requestStream, Encoding.UTF8);

                    streamWriter.Write(bodyContents);

                    streamWriter.Close();
                    requestStream.Close();
                }

                var response = (HttpWebResponse)request.GetResponse();

                if (response.ContentLength > 0)
                {
                    var responseStream = response.GetResponseStream();

                    if (null == responseStream)
                        throw new Exception("Unable to create Stream from HttpWebResponse");

                    var reader = new StreamReader(responseStream, Encoding.ASCII);
                    responseOut.Body = reader.ReadToEnd();

                    var err = FetchJsonValue(responseOut.Body, "error") as Dictionary<string, dynamic>;

                    if (null != err)
                    {
                        responseOut.HadError = true;
                        responseOut.Body += string.Format("Web Error making REST API call.\r\nCode: {0}, Message: {1}",
                            err["code"], err["code"]);
                    }

                    var props = FetchJsonValue(responseOut.Body, "properties") as Newtonsoft.Json.Linq.JObject;

                    if (null != props)
                    {
                        if (null != props["provisioningState"])
                            responseOut.ProviderRequestState = props["provisioningState"].ToString();

                        if (null != props["correlationId"])
                        {
                            var now = DateTime.UtcNow;

                            responseOut.StatusCheckUrl = AzureEventOps.BuildEventFetchUrl(
                                props["correlationId"].ToString(),
                                Connection.SubcriptionID, now.AddMinutes(-5), now.AddHours(4));
                        }

                        responseOut.HadError = false;
                    }

                    reader.Close();
                    responseStream.Close();
                }
                else
                {
                    if (!requestUrl.Contains("/operations/"))
                        responseOut.Body = "No HTTP Response Body Returned.";
                }

                foreach (var key in response.Headers.Keys)
                {
                    var keyName = key as string;

                    if (keyName.Equals("Azure-AsyncOperation")) 
                        responseOut.StatusCheckUrl = response.Headers.GetValues(keyName)[0] as string;
                }

                response.Close();
            }
            catch (WebException exWeb)
            {
                var responseStream = exWeb.Response.GetResponseStream();

                if (null == responseStream)
                {
                    responseOut.HadError = true;
                    responseOut.Body += "Error making REST API call.\r\n" + exWeb.Message + "\r\n";
                    return responseOut;
                }

                var reader = new StreamReader(responseStream, Encoding.ASCII);

                var responseStatus = (HttpWebResponse)exWeb.Response;
                responseOut.HTTP = (int)responseStatus.StatusCode + " - " +
                    responseStatus.StatusCode.ToString();
                responseOut.HadError = true;
                responseOut.Body += "Web Error making REST API call.\r\nMessage: " +
                    exWeb.Message + "\r\nResponse:\r\n" + reader.ReadToEnd();

                responseStream.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                responseOut.HadError = true;
                responseOut.Body += "Error making REST API call.\r\n" + ex.Message + "\r\n";
            }

            return responseOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonIn"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static object FetchJsonValue(string jsonIn, string keyName)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonIn);
                return !jsonResult.ContainsKey(keyName) ? null : jsonResult[keyName];
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot deserialize"))
                    return null;

                throw new Exception("Exception in FetchJsonValue() : " + Utilities.UnwindExceptionMessages(ex));
            }
        }
    }
}

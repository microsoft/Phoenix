using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpInterfaceModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace AzureAdminClientLib
{
    public class AzureEventOps
    {
        private const string FETCHEVENT_URL_TEMPLATE =
            @"https://{0}/subscriptions/{1}/providers/microsoft.insights/eventtypes/management/values?api-version={2}&$filter={3}";
        //private const string FETCHEVENT_CORRELATIONFILTER_TEMPLATE =
        //    @"eventTimestamp ge '{0}' and eventTimestamp le '{1}' and correlationId eq '{2}'";
        private const string FETCHEVENT_CORRELATIONFILTER_TEMPLATE =
            @"eventTimestamp ge '{0}' and correlationId eq '{1}'";
        private const string FETCHEVENT_AIPVERSION =
            "2015-04-01";

        private Connection _connection = null;
        public AzureEventOps(Connection connection)
        {
            _connection = connection;
        }

        public class AzureException
        {
            public string StatusCode;
            public string ErrorCode;
            public string ErrorMessage;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="exceptionList"></param>
        /// <param name="statusCode"></param>
        ///  <param name="errorCode"></param>
        ///  <param name="errorMessage"></param>
        ///  
        //*********************************************************************
        private static List<AzureException> AddExceptionToList( List<AzureException> exceptionList, string statusCode, string errorCode, string errorMessage )
        {
            if (null == exceptionList)
                exceptionList = new List<AzureException>();
            else if (exceptionList.Any(ex => ex.ErrorMessage.Equals(errorMessage)))
                return exceptionList;

            exceptionList.Add(new AzureException()
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            });

            return exceptionList;
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

        private static object FetchJsonValue(string jsonIn, string keyName)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonIn);

                if (null == keyName)
                    return jsonResult;

                return !jsonResult.ContainsKey(keyName) ? null : jsonResult[keyName];
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchJsonValue() : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventListJson"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static List<AzureException> ExtractEventExceptions(string eventListJson)
        {
            try
            {
                var exceptionList = new List<AzureException>();
                var jvList = FetchJsonValue(eventListJson, null);

                var jvDict = jvList as Dictionary<string, object>;

                if (null != jvDict) foreach (var mainBlock in jvDict.Values)
                {
                    var azrEventList = mainBlock as Newtonsoft.Json.Linq.JArray;

                    if (null != azrEventList) foreach (var azureEvent in azrEventList)
                    {
                        var azureEventObj = azureEvent as Newtonsoft.Json.Linq.JObject;

                        if (null != azureEventObj) foreach (var child in azureEventObj)
                        {
                            if (child.Key.Equals("properties"))
                            {
                                var statusCode = FetchJsonValue(child.Value.ToString(), "statusCode");
                                var statusBlock = FetchJsonValue(child.Value.ToString(), "statusMessage");

                                if (null != statusCode) if (statusCode.Equals("BadRequest"))
                                {
                                    var errorBlock = FetchJsonValue(statusBlock.ToString(), "error");
                                    var errorCode = FetchJsonValue(errorBlock.ToString(), "code");
                                    var errorMessage = FetchJsonValue(errorBlock.ToString(), "message");

                                    exceptionList = AddExceptionToList( exceptionList,
                                        statusCode.ToString(),
                                        null != errorCode ? errorCode.ToString() : null,
                                        null != errorMessage ? errorMessage.ToString() : null);
                                }
                            }
                        }
                    }
                }

                return exceptionList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ExtractEventExceptions() : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public static bool IsComplete(string eventListJson, string resourceProviderName, out List<AzureException> exceptionList)
        {
            try
            {
                exceptionList = null;
                var jvList = FetchJsonValue(eventListJson, null);
                string foundResProvName = "";
                string foundStatus = "";
                bool haveProperties = false;

                var jvDict = jvList as Dictionary<string, object>;

                if (null != jvDict) foreach (var mainBlock in jvDict.Values)
                    {
                        var azrEventList = mainBlock as Newtonsoft.Json.Linq.JArray;

                        if (null != azrEventList) foreach (var azureEvent in azrEventList)
                            {
                                foundResProvName = "";
                                foundStatus = "";
                                haveProperties = false;

                                var azureEventObj = azureEvent as Newtonsoft.Json.Linq.JObject;

                                if (null != azureEventObj) foreach (var child in azureEventObj)
                                    {
                                        if (child.Key.Equals("properties"))
                                        {
                                            var statusCode = FetchJsonValue(child.Value.ToString(), "statusCode");
                                            var statusBlock = FetchJsonValue(child.Value.ToString(), "statusMessage");

                                            if (null != statusCode) if (statusCode.Equals("BadRequest"))
                                                {
                                                    var errorBlock = FetchJsonValue(statusBlock.ToString(), "error");
                                                    var errorCode = FetchJsonValue(errorBlock.ToString(), "code");
                                                    var errorMessage = FetchJsonValue(errorBlock.ToString(), "message");
                                                    haveProperties = true;

                                                    exceptionList = AddExceptionToList(exceptionList,
                                                        statusCode.ToString(),
                                                        null != errorCode ? errorCode.ToString() : null,
                                                        null != errorMessage ? errorMessage.ToString() : null);
                                                }
                                        }
                                        else if (child.Key.Equals("resourceProviderName"))
                                        {
                                            var temp = FetchJsonValue(child.Value.ToString(), "value");
                                            if (null != temp) foundResProvName = temp.ToString();
                                        }
                                        else if (child.Key.Equals("status"))
                                        {
                                            if (haveProperties)
                                                continue;

                                            var temp = FetchJsonValue(child.Value.ToString(), "value");
                                            if (null != temp) foundStatus = temp.ToString();

                                            if (!foundStatus.Equals("Succeeded"))
                                                continue;

                                            if (foundResProvName.Equals(resourceProviderName))
                                                return true;
                                        }
                                    }
                            }
                    }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ExtractEventExceptions() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Example: https://management.azure.com/subscriptions/00f885db-ef1d-4545-ad2d-64c0caf93384/providers/microsoft.insights/eventtypes/management/values?api-version=2015-04-01&$filter=eventTimestamp ge '2015-8-18T16:0:9.0Z' and correlationId eq '6b4ced55-7c65-46c1-b20c-57e66341cf30'</param>
        /// <param name="exceptionList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchEvents(string url, out List<AzureException> exceptionList)
        {
            try
            {
                var hi = new HttpInterface(_connection);
                var res = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);
                exceptionList = ExtractEventExceptions(res.Body);

                return res.Body;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchEvents() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="azureSubId"></param>
        /// <param name="timeBegin"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static string BuildEventFetchUrl(string correlationId,
            string azureSubId, string timeBegin, string timeEnd)
        {
            try
            {
                //var eventFilter = String.Format(FETCHEVENT_CORRELATIONFILTER_TEMPLATE,
                //    timeBegin, timeEnd, correlationId);

                var eventFilter = String.Format(FETCHEVENT_CORRELATIONFILTER_TEMPLATE,
                    timeBegin, correlationId);

                var url = string.Format(FETCHEVENT_URL_TEMPLATE,
                    Constants.ARMMANAGEMENTADDRESS, azureSubId, FETCHEVENT_AIPVERSION, eventFilter);

                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildEventFetchUrl() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="azureSubId"></param>
        /// <param name="timeBeginZ"></param>
        /// <param name="timeEndZ"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static string BuildEventFetchUrl(string correlationId,
            string azureSubId, DateTime timeBeginZ, DateTime timeEndZ)
        {
            try
            {
                timeBeginZ = timeBeginZ.ToUniversalTime();
                timeEndZ = timeEndZ.ToUniversalTime();

                var timeBegin = string.Format("{0}-{1}-{2}T{3}:{4}:{5}.{6}Z",
                    timeBeginZ.Year, timeBeginZ.Month, timeBeginZ.Day,
                    timeBeginZ.Hour, timeBeginZ.Minute, timeBeginZ.Second,
                    timeBeginZ.Millisecond);

                var timeEnd = string.Format("{0}-{1}-{2}T{3}:{4}:{5}.{6}Z",
                    timeEndZ.Year, timeEndZ.Month, timeEndZ.Day,
                    timeEndZ.Hour, timeEndZ.Minute, timeEndZ.Second,
                    timeEndZ.Millisecond);

                return BuildEventFetchUrl( correlationId,
                    azureSubId, timeBegin, timeEnd);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildEventFetchUrl() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="azureSubId"></param>
        /// <param name="timeBegin">Format: yyyy-mm-ddThh:mm:ss.msZ Example 2015-8-18T15:57:57.0Z</param>
        /// <param name="timeEnd">Format: yyyy-mm-ddThh:mm:ss.msZ Example 2015-8-18T15:57:57.0Z</param>
        /// <param name="exceptionList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchEvents(string correlationId, string azureSubId,
            string timeBegin, string timeEnd, out List<AzureException> exceptionList)
        {
            try
            {
                return FetchEvents(BuildEventFetchUrl(correlationId,
                    azureSubId, timeBegin, timeEnd), out exceptionList);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchEvents() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="azureSubId"></param>
        /// <param name="timeBeginZ"></param>
        /// <param name="timeEndZ"></param>
        /// <param name="exceptionList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchEvents(string correlationId, string azureSubId,
            DateTime timeBeginZ, DateTime timeEndZ, out List<AzureException> exceptionList)
        {
            try
            {
                return FetchEvents(BuildEventFetchUrl(correlationId,
                    azureSubId, timeBeginZ, timeEndZ), out exceptionList);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchEvents() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }
    }
}

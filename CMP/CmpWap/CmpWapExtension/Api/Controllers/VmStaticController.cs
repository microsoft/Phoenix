using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using Microsoft.WindowsAzurePack.Samples.DataContracts;
using Microsoft.WindowsAzurePack.Samples;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Web.Http.OData;
using System.Text;
using System.Text.RegularExpressions;



namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This class contains methods that provide VM related information.
    /// </remarks>
    public class VmStaticController : ApiController
    {
        private static List<CreateVm> VmList = new List<CreateVm>();
        private static bool _havePendingStatus = false;

        string adminserviceEndpoint = System.Configuration.ConfigurationManager.AppSettings["AdminserviceEndpoint"];
        string windowsAuthSiteEndpoint = System.Configuration.ConfigurationManager.AppSettings["WindowsAuthEndpoint"];

        [HttpPost]
        public async Task<HttpResponseMessage> CreateVmFromStaticTemplate([FromBody] CreateVm vM)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    var result = new HttpResponseMessage()
                //    {
                //        StatusCode = HttpStatusCode.BadRequest,
                //        Content = new StringContent(
                //            "Model State is not valid", Encoding.UTF8,
                //            "application/json"),
                //        ReasonPhrase = "ModelState is not valid"
                //    };
                //    return result;
                //}

                LogThis(EventLogEntryType.Information, "VM Create Request Submitted", 2, 1);

                vM.CreatedBy = await GetWapAdmin(vM.SubscriptionId); 
                //return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };

                var cwdb = new CmpWapDb();

                //*** Map WAP sub to ResProvGroupId

                var resourceProviderGroupName =
                    cwdb.FetchDefaultResourceProviderGroupName(vM.SubscriptionId);

                if (null == resourceProviderGroupName)
                    throw new Exception("Could not locate DefaultResourceProviderGroupName for WAP subscription");

                vM.EnvResourceGroupName = resourceProviderGroupName;

                var cmpi = new VMServiceRepository(_eventLog);

                lock (vM)
                {
                    //Insert app data to the DB 
                    //cmpi.PerformAppDataOps(new CreateVm
                    //{
                    //    VmAppName = vM.VmAppName,
                    //    VmAppId = vM.VmAppId,
                    //    SubscriptionId = vM.SubscriptionId,
                    //    AccountAdminLiveEmailId = vM.AccountAdminLiveEmailId,
                    //    VmRegion = vM.VmRegion
                    //});
                    //Submit VM information to the WAP DB
                    //vM = cmpi.SubmitVmRequest(vM);
                    vM = cmpi.SubmitVmRequestForStaticTemplate(vM);

                    
                    AddVmToList(vM);
                }

                LogThis(EventLogEntryType.Information, "VM Create Request Submitted OK", 2, 2);

                //return Ok(vM);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmStaticController.CreateVmFromStaticTemplate()", 100, 1);

                //return InternalServerError(ex);
                //throw new Microsoft.WindowsAzurePack.CmpWapExtension.Common.PortalException(ex.Message);
                var reason = "Exception while submitting request to Create VM : " +
                    Regex.Replace(CmpCommon.Utilities.UnwindExceptionMessages(ex), @"\t|\n|\r", "");

                // Making this as a bad request instead of Internal Server error because the reason phrase for Internal Server error is not able to be customized.
                var result = new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = reason };
                return result;
            }
        }


        private void AddVmToList(CreateVm vM)
        {
            if (!vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()))
                _havePendingStatus = true;

            VmList.Add(vM);
        }

        private async Task<string> GetWapAdmin(string wapSubscriptionId)
        {
            return wapSubscriptionId;

            AdminManagementClient adminClient;
            var token = TokenIssuer.GetWindowsAuthToken(windowsAuthSiteEndpoint, null, null, null, false);

            adminClient = new AdminManagementClient(new Uri(adminserviceEndpoint), token);

            var result = await adminClient.GetSubscriptionAsync(wapSubscriptionId);

            return result.AccountAdminLiveEmailId;
        }

        static EventLog _eventLog = null;


        public static EventLog eventLog
        {
            set { _eventLog = value; }
            get
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application")
                        {
                            Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName
                        };
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
            }
        }
        private void LogThis(EventLogEntryType type, string message,
             int id, short category)
        {
            try
            {
                if (null != eventLog)
                    eventLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            try
            {
                if (null != eventLog)
                    eventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
    }
}
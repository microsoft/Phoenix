using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Diagnostics;
using CmpInterfaceModel.Models;

namespace CmpAzureServiceWebRole.Controllers
{
    //*************************************************************************
    ///
    /// <summary>
    /// Handler for basic creds incoming from client requests
    /// </summary>
    /// 
    //*************************************************************************

    /*public class BasicAuthMessageHandler : DelegatingHandler
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";
 
        //public IProvidePrincipal PrincipalProvider { get; set; }
 
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            System.Net.Http.Headers.AuthenticationHeaderValue authValue = request.Headers.Authorization;
            if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);
                if (parsedCredentials != null)
                {
                    //System.Threading.Thread.CurrentPrincipal = PrincipalProvider
                    //    .CreatePrincipal(parsedCredentials.Username, parsedCredentials.Password);
                }
            }
            return base.SendAsync(request, cancellationToken)
               .ContinueWith(task =>
               {
                   var response = task.Result;
                   if (response.StatusCode == HttpStatusCode.Unauthorized
                       && !response.Headers.Contains(BasicAuthResponseHeader))
                   {
                       response.Headers.Add(BasicAuthResponseHeader
                           , BasicAuthResponseHeaderValue);
                   }
                   return response;
               });
        }
 
        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = System.Text.Encoding.ASCII.GetString(Convert
                                                            .FromBase64String(authHeader))
                                                            .Split(
                                                            new[] { ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1])) return null;
            return new Credentials()
                       {
                           Username = credentials[0],
                           Password = credentials[1],
                       };
        }
    }

    //*************************************************************************
    ///
    /// <summary>
    /// Handler for client certificates incoming from client requests
    ///
    /// </summary>
    /// http://blogs.msdn.com/b/hongmeig1/archive/2012/05/11/how-to-access-clientcertificate-in-a-host-agnostic-manner.aspx
    /// http://blogs.msdn.com/b/hongmeig1/rss.aspx
    /// 
    //*************************************************************************

    public class CustomCertificateMessageHandler : System.Net.Http.DelegatingHandler
    {
        protected override Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate cert = request.GetClientCertificate();

            if (cert != null)
            {
                if (cert.Subject.Contains("Some Name you are expecting"))
                {
                    System.Threading.Thread.CurrentPrincipal =
                        new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(cert.Subject), new[] { "Administrators" });
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }*/

    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using CmpInterfaceModel.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<VmDeploymentRequest>("VmDeployments");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */

    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class VmDeploymentsController : ODataController
    {
        //private VmDeploymetRequestContext db = new VmDeploymetRequestContext();
        static EventLog _EventLog = null;
        static string _CmpDbConnectionString = null;
        static string _AftsDbConnectionString = null;

        /// <summary></summary>
        public static EventLog eventLog { set { _EventLog = value; } }

        /// <summary></summary>
        public static string cmpDbConnectionString 
        {
            set { _CmpDbConnectionString = value; }
            get { return _CmpDbConnectionString; }
        }

        public static string aftsDbConnectionString
        {
            set { _AftsDbConnectionString = value; }
            get { return _AftsDbConnectionString; }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(Exception ex, EventLogEntryType type, string prefix, 
            int id, short category )
        {
            if (null != _EventLog)
                _EventLog.WriteEntry(prefix + " : " + 
                    CmpCommon.Utilities.UnwindExceptionMessages(ex), type, id, category);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        int NonNull(int? value)
        {
            if (null == value)
                return 0;
            return (int)value;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmdeploymentrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        CmpInterfaceModel.Models.VmDeploymentRequest ServToInt(
            CmpServiceLib.Models.VmDeploymentRequest vmdeploymentrequest)
        {
            if (null == vmdeploymentrequest)
                return null;

            var ret = new CmpInterfaceModel.Models.VmDeploymentRequest
            {
                ID = vmdeploymentrequest.ID,
                ExceptionMessage = vmdeploymentrequest.ExceptionMessage,
                LastStatusUpdate = CmpServiceLib.Utilities.GetDbDateTime(vmdeploymentrequest.LastStatusUpdate),
                ParentAppName = vmdeploymentrequest.ParentAppName,
                RequestDescription = vmdeploymentrequest.RequestDescription,
                RequestName = vmdeploymentrequest.RequestName,
                SourceServerName = vmdeploymentrequest.SourceServerName,
                SourceVhdFilesCSV = vmdeploymentrequest.SourceVhdFilesCSV,
                Status = vmdeploymentrequest.StatusCode,
                TagData = vmdeploymentrequest.TagData,
                TargetLocation = vmdeploymentrequest.TargetLocation,
                TargetLocationType = vmdeploymentrequest.TargetLocationType,
                TargetVmName = vmdeploymentrequest.TargetVmName,
                TargetServiceName = vmdeploymentrequest.TargetServicename,
                VmSize = vmdeploymentrequest.VmSize,
                WhenRequested = CmpServiceLib.Utilities.GetDbDateTime(vmdeploymentrequest.WhenRequested),
                WhoRequested = vmdeploymentrequest.WhoRequested,
                Active = CmpServiceLib.Utilities.GetDbBool(vmdeploymentrequest.Active),
                AftsID = CmpServiceLib.Utilities.GetDbInt(vmdeploymentrequest.AftsID),
                Config = vmdeploymentrequest.Config,
                ParentAppID = vmdeploymentrequest.ParentAppID,
                RequestType = vmdeploymentrequest.RequestType,
                StatusMessage = vmdeploymentrequest.StatusMessage,
                TargetAccount = vmdeploymentrequest.TargetAccount,
                TargetAccountCreds = vmdeploymentrequest.TargetAccountCreds,
                TargetAccountType = vmdeploymentrequest.TargetAccountType,
                TargetServiceProviderType = vmdeploymentrequest.TargetServiceProviderType,
                TagID = CmpServiceLib.Utilities.GetDbInt(vmdeploymentrequest.TagID),
                ExceptionTypeCode = vmdeploymentrequest.ExceptionTypeCode,
                SourceServerRegion = vmdeploymentrequest.SourceServerRegion,
                ValidationResults = vmdeploymentrequest.ValidationResults,
                TargetServiceProviderAccountID = NonNull(vmdeploymentrequest.ServiceProviderAccountID)
            };

            return ret;
        }

        //*********************************************************************
        ///
        // GET odata/VmDeployments
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        //[RequireClientCertAttribute]
        [Queryable]
        public IQueryable<CmpInterfaceModel.Models.VmDeploymentRequest> VmDeployments()
        {
            try
            {
                var vdr = new List<CmpInterfaceModel.Models.VmDeploymentRequest>();

                using (var cmp = new CmpServiceLib.CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString))
                {
                    var vmdeploymentrequestList = cmp.FetchVmDepRequests();

                    foreach (var vmdeploymentrequest in vmdeploymentrequestList)
                        if (null != vmdeploymentrequest)
                            vdr.Add(ServToInt(vmdeploymentrequest));
                }
                    
                return vdr.AsQueryable();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in GetVmDeploymentRequest()", 100, 100);
                throw;
            }
        }

        //*********************************************************************
        ///
        // GET odata/VmDeployments(5)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        //[RequireClientCertAttribute]
        [Queryable]
        public SingleResult<CmpInterfaceModel.Models.VmDeploymentRequest> 
            GetVmDeploymentRequest([FromODataUri] int key)
        {
            try
            {
                //****************************

                //var testReqList = new List<VmDeploymentRequest>();
                //testReqList.Add(new CmpInterfaceModel.Models.VmDeploymentRequest());
                //return SingleResult.Create(testReqList.AsQueryable());

                //****************************

                using (var cmp = new CmpServiceLib.CmpService(_EventLog, 
                    _CmpDbConnectionString, _AftsDbConnectionString))
                {
                    var vmdeploymentrequest = cmp.FetchVmDepRequest(key);

                    if (null == vmdeploymentrequest)
                        return null;

                    var vdr = new List<CmpInterfaceModel.Models.VmDeploymentRequest> 
                    {ServToInt(vmdeploymentrequest)};

                    return SingleResult.Create(vdr.AsQueryable());
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, 
                    "Exception in GetVmDeploymentRequest()", 100, 100);
                throw;
            }

            //return SingleResult.Create(db.VmDeploymentRequests.Where(vmdeploymentrequest => vmdeploymentrequest.ID == key));
        }

        //*********************************************************************
        ///
        // PUT odata/VmDeployments(5)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vmdeploymentrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, 
            CmpInterfaceModel.Models.VmDeploymentRequest vmdeploymentrequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != vmdeploymentrequest.ID)
            {
                return BadRequest();
            }

            /*db.Entry(vmdeploymentrequest).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VmDeploymentRequestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            return Updated(vmdeploymentrequest);
        }

        //*********************************************************************
        ///
        // POST odata/VmDeployments
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmdeploymentrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Post(
            CmpInterfaceModel.Models.VmDeploymentRequest vmdeploymentrequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var checkForExistingVm = true;
                var newRequest = true;
                CmpServiceLib.Models.VmDeploymentRequest response = null;

                var cmp = new CmpServiceLib.CmpService(_EventLog, 
                    _CmpDbConnectionString, _AftsDbConnectionString);


                vmdeploymentrequest.LastStatusUpdate = DateTime.Now;

                if (0 == vmdeploymentrequest.ID)
                {
                    if(cmp.IsVmDepRequestInProcess(vmdeploymentrequest.TargetVmName))
                        throw new Exception(string.Format(
                            "A request for deployment for a server with name '{0}' is currently in process. Duplicates not allowed.", 
                            vmdeploymentrequest.TargetVmName));

                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmdeploymentrequest.Config);

                    if (null != vmCfg)
                        if (null != vmCfg.Placement)
                            if (vmCfg.Placement.RebuildRequest)
                                checkForExistingVm = false;

                    if (checkForExistingVm)
                        if (DoesNameResolve(vmdeploymentrequest.TargetVmName))
                            throw new Exception(string.Format(
                                "A DNS record for a server with name '{0}' already exists. Duplicates not allowed.", 
                                vmdeploymentrequest.TargetVmName));

                    if (vmdeploymentrequest.WhenRequested.Year < 2013)
                        vmdeploymentrequest.WhenRequested = DateTime.Now;

                    vmdeploymentrequest.Status = 
                        CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString();
                }
                else
                    newRequest = false;

                var vmDepReq = new CmpServiceLib.Models.VmDeploymentRequest
                {
                    ID = vmdeploymentrequest.ID,
                    ExceptionMessage = vmdeploymentrequest.ExceptionMessage,
                    LastStatusUpdate = vmdeploymentrequest.LastStatusUpdate,
                    ParentAppName = vmdeploymentrequest.ParentAppName,
                    RequestDescription = vmdeploymentrequest.RequestDescription,
                    RequestName = vmdeploymentrequest.RequestName,
                    SourceServerName = vmdeploymentrequest.SourceServerName,
                    SourceVhdFilesCSV = vmdeploymentrequest.SourceVhdFilesCSV,
                    SourceServerRegion = vmdeploymentrequest.SourceServerRegion,
                    StatusCode = vmdeploymentrequest.Status,
                    TagData = vmdeploymentrequest.TagData,
                    TargetLocation = vmdeploymentrequest.TargetLocation,
                    TargetLocationType = vmdeploymentrequest.TargetLocationType,
                    TargetVmName = vmdeploymentrequest.TargetVmName,
                    TargetServicename = vmdeploymentrequest.TargetServiceName,
                    VmSize = vmdeploymentrequest.VmSize,
                    WhenRequested = vmdeploymentrequest.WhenRequested,
                    WhoRequested = vmdeploymentrequest.WhoRequested,
                    Active = vmdeploymentrequest.Active,
                    AftsID = vmdeploymentrequest.AftsID,
                    Config = vmdeploymentrequest.Config,
                    ConfigOriginal = vmdeploymentrequest.Config,
                    ParentAppID = vmdeploymentrequest.ParentAppID,
                    RequestType = vmdeploymentrequest.RequestType,
                    StatusMessage = vmdeploymentrequest.StatusMessage,
                    TargetAccount = vmdeploymentrequest.TargetAccount,
                    TargetAccountCreds = vmdeploymentrequest.TargetAccountCreds,
                    TargetAccountType = vmdeploymentrequest.TargetAccountType,
                    TargetServiceProviderType = vmdeploymentrequest.TargetServiceProviderType,
                    ServiceProviderAccountID = 1,
                    ServiceProviderStatusCheckTag = null,
                    TagID = vmdeploymentrequest.TagID,
                    OverwriteExisting = false,
                    ValidationResults = vmdeploymentrequest.ValidationResults,
                    ExceptionTypeCode = vmdeploymentrequest.ExceptionTypeCode, 
                    //CurrentStateStartTime  = Now, 
                    //CurrentStateTryCount = 0, 
                    //LastState = null, 
                    //ServiceProviderResourceGroup = null, 
                    //Warnings = null
                };

                if (newRequest)
                    response = cmp.InsertVmDepRequest(vmDepReq);
                else
                    cmp.ResubmitVmDepRequest(vmDepReq);

                if (response != null) 
                    vmdeploymentrequest.ID = response.ID;

                return Created(vmdeploymentrequest);
            }
            catch(Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in Post()", 100, 100);
                throw;
            }
        }

        //*********************************************************************
        ///
        // PATCH odata/VmDeployments(5)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CmpInterfaceModel.Models.VmDeploymentRequest> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*CmpInterfaceModel.Models.VmDeploymentRequest vmdeploymentrequest = await db.VmDeploymentRequests.FindAsync(key);
            if (vmdeploymentrequest == null)
            {
                return NotFound();
            }

            patch.Patch(vmdeploymentrequest);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VmDeploymentRequestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(vmdeploymentrequest);*/
            return Updated((CmpInterfaceModel.Models.VmDeploymentRequest)null);
        }

        //*********************************************************************
        ///
        // DELETE odata/VmDeployments(5)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            /*CmpInterfaceModel.Models.VmDeploymentRequest vmdeploymentrequest = await db.VmDeploymentRequests.FindAsync(key);
            if (vmdeploymentrequest == null)
            {
                return NotFound();
            }

            db.VmDeploymentRequests.Remove(vmdeploymentrequest);
            await db.SaveChangesAsync();*/

            return StatusCode(HttpStatusCode.NoContent);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> Reboot([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var param = (int)parameters["Param"];

            /*var product = await db.VmDeploymentRequests.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }*/

            //*** Add reboot code here ***

            return Ok(param);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        //*********************************************************************

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private bool VmDeploymentRequestExists(int key)
        {
            //return db.VmDeploymentRequests.Count(e => e.ID == key) > 0;
            return true;
        }

        public bool DoesNameResolve(string name)
        {
            try
            {
                var addrList = Dns.GetHostAddresses(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

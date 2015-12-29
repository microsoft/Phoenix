using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using CmpInterfaceModel.Models;
using CmpAzureServiceWebRole.Models;
using System.Diagnostics;
using System.Web.Http.OData.Query;
using CmpServiceLib;
using Microsoft.Ajax.Utilities;
using VmMigrationRequest = CmpServiceLib.Models.VmMigrationRequest;

namespace CmpAzureServiceWebRole.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using CmpInterfaceModel.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<VmMigrationRequest>("VmMigrations");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class VmMigrationsController : ODataController
    {
        private DummyContext db = new DummyContext();

        static EventLog _EventLog = null;
        static string _CmpDbConnectionString = null;
        static string _AftsDbConnectionString = null;

        /// <summary></summary>
        public static EventLog eventLog { set { _EventLog = value; } }

        /// <summary></summary>
        public static string cmpDbConnectionString { set { _CmpDbConnectionString = value; } }
        public static string aftsDbConnectionString { set { _AftsDbConnectionString = value; } }

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
            int id, short category)
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
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        bool NonNull(bool? value)
        {
            if (null == value)
                return false;
            return (bool)value;
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

        DateTime NonNull(DateTime? value)
        {
            if (null == value)
                return new DateTime(1970, 1, 1);
            return (DateTime)value;
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

        CmpInterfaceModel.Models.VmMigrationRequest ServToInt(
            CmpServiceLib.Models.VmMigrationRequest vmMigrateRequest)
        {
            if (null == vmMigrateRequest)
                return null;

            var Ret =
                new CmpInterfaceModel.Models.VmMigrationRequest
                {
                    ID = vmMigrateRequest.ID,
                    VmDeploymentRequestID = vmMigrateRequest.VmDeploymentRequestID,
                    VmSize = vmMigrateRequest.VmSize,
                    Config = vmMigrateRequest.Config ?? GetTemplateXML(),
                    TargetVmName = vmMigrateRequest.TargetVmName,
                    SourceServerName = vmMigrateRequest.SourceServerName,
                    SourceVhdFilesCSV = vmMigrateRequest.SourceVhdFilesCSV,
                    ExceptionMessage = vmMigrateRequest.ExceptionMessage,
                    LastStatusUpdate = NonNull(vmMigrateRequest.LastStatusUpdate),
                    StatusCode = vmMigrateRequest.StatusCode,
                    StatusMessage = vmMigrateRequest.StatusMessage,
                    AgentRegion = vmMigrateRequest.AgentRegion,
                    AgentName = vmMigrateRequest.AgentName,
                    CurrentStateStartTime = NonNull(vmMigrateRequest.CurrentStateStartTime),
                    CurrentStateTryCount = NonNull(vmMigrateRequest.CurrentStateTryCount),
                    Warnings = vmMigrateRequest.Warnings,
                    Active = NonNull(vmMigrateRequest.Active)
                };

            return Ret;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmMigrateRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        CmpServiceLib.Models.VmMigrationRequest IntToServ(
            CmpInterfaceModel.Models.VmMigrationRequest vmMigrateRequest)
        {
            if (null == vmMigrateRequest)
                return null;

            var Ret =
                new CmpServiceLib.Models.VmMigrationRequest
                {
                    ID = vmMigrateRequest.ID,
                    VmDeploymentRequestID = vmMigrateRequest.VmDeploymentRequestID,
                    VmSize = vmMigrateRequest.VmSize,
                    Config = vmMigrateRequest.Config,
                    TargetVmName = vmMigrateRequest.TargetVmName,
                    SourceServerName = vmMigrateRequest.SourceServerName,
                    SourceVhdFilesCSV = vmMigrateRequest.SourceVhdFilesCSV,
                    ExceptionMessage = vmMigrateRequest.ExceptionMessage,
                    LastStatusUpdate = NonNull(vmMigrateRequest.LastStatusUpdate),
                    StatusCode = vmMigrateRequest.StatusCode,
                    StatusMessage = vmMigrateRequest.StatusMessage,
                    AgentRegion = vmMigrateRequest.AgentRegion,
                    AgentName = vmMigrateRequest.AgentName,
                    CurrentStateStartTime = NonNull(vmMigrateRequest.CurrentStateStartTime),
                    CurrentStateTryCount = NonNull(vmMigrateRequest.CurrentStateTryCount),
                    Warnings = vmMigrateRequest.Warnings,
                    Active = NonNull(vmMigrateRequest.Active)
                };

            return Ret;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qS"></param>
        /// <param name="Status"></param>
        /// <param name="Region"></param>
        /// 
        //*********************************************************************

        string ExtractQsValue(string qS, string Key)
        {
            // StatusCode eq 'Submitted' and AgentRegion eq 'Tuk5'

            try
            {
                int index = qS.IndexOf(Key);

                if (-1 == index)
                    return null;

                qS = qS.Substring(index);
                index = qS.IndexOf('\'');

                if (-1 == index)
                    return null;

                qS = qS.Substring(index + 1);
                index = qS.IndexOf('\'');

                if (-1 == index)
                    return null;

                qS = qS.Substring(0, index);

                return qS;
            }
            catch (Exception)
            {
                return null;
            }
        }

        string ExtractQsValueByKey(string qS, string Key)
        {
            // StatusCode eq 'Submitted' and AgentRegion eq 'Tuk5'

            try
            {
                int index = qS.IndexOf(Key);

                if (-1 == index)
                    return null;

                qS = qS.Substring(index);
                index = qS.IndexOf("eq ");

                if (-1 == index)
                    return null;

                qS = qS.Substring(index + 2);
                qS = qS.Replace("'", "");
                return qS.Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// GET odata/VmMigrations
        /// http://127.0.0.1:81/Odata/VmMigrations
        /// http://127.0.0.1:81/Odata/VmMigrations?$filter=StatusCode%20eq%20'Submitted'%20and%20AgentRegion%20eq%20'XXX'
        /// </summary>
        /// <returns></returns>
        /// http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        /// http://blogs.msdn.com/b/webdev/archive/2013/02/25/translating-odata-queries-to-hql.aspx
        /// 
        //*********************************************************************

        //[Authorize]
        [Queryable]
        public IQueryable<CmpInterfaceModel.Models.VmMigrationRequest> GetVmMigrations(ODataQueryOptions queryString)
        {
            string filterText = null;
            string reqStatus = null;
            string reqAgentRegion = null;
            string VmDeploymentRequestID = null;
            string reqAgentName = null;
            if (null != queryString.Filter)
            {
                filterText = queryString.Filter.RawValue;
                reqStatus = ExtractQsValue(filterText, "StatusCode");
                reqAgentRegion = ExtractQsValue(filterText, "AgentRegion");
                reqAgentName = ExtractQsValue(filterText, "AgentName");
                VmDeploymentRequestID = ExtractQsValueByKey(filterText, "VmDeploymentRequestID");
                reqAgentName = ExtractQsValueByAgent(filterText, "AgentName");
            }

            var vmrOutList = new List<CmpInterfaceModel.Models.VmMigrationRequest>();

            try
            {
                using (CmpServiceLib.CmpService CMP = new CmpServiceLib.CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString))
                {
                    List<CmpServiceLib.Models.VmMigrationRequest> vmMigrationRequestList = null;
                    if (null == VmDeploymentRequestID)
                    {
                        vmMigrationRequestList = CMP.FetchMigrationRequestsByAgent(reqAgentName);
                        var filteredMigrationRequest = vmMigrationRequestList.DistinctBy(o => o.ID);
                        vmMigrationRequestList = filteredMigrationRequest.ToList();
                    }
                    else
                    {
                        vmMigrationRequestList = new List<VmMigrationRequest>();
                        var migrationRequest = CMP.FetchMigrationRequest(Convert.ToInt32(VmDeploymentRequestID),
                            CmpDb.FetchMigrationRequestKeyTypeEnum.DepReqId);
                        if (null != migrationRequest)
                        {
                            vmMigrationRequestList.Add(migrationRequest);
                        }
                    }

                    if (null == vmMigrationRequestList)
                        return null;

                    foreach (CmpServiceLib.Models.VmMigrationRequest vmMigrationRequest in vmMigrationRequestList)
                        vmrOutList.Add(ServToInt(vmMigrationRequest));
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in GetVmMigrationRequests()", 100, 100);
                throw;
            }

            return vmrOutList.AsQueryable();
        }

        private string ExtractQsValueByAgent(string qS, string Key)
        {

            try
            {
                int start = qS.IndexOf("eq ") + 2;
                int end = qS.IndexOf("or", start);
                string result = qS.Substring(start, end - start);
                result = result.Replace("'", "");
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// GET odata/VmMigrations(5)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        [Queryable]
        public SingleResult<CmpInterfaceModel.Models.VmMigrationRequest> GetVmMigrationRequest([FromODataUri] int key)
        {
            try
            {
                using (CmpServiceLib.CmpService CMP = new CmpServiceLib.CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString ))
                {
                    CmpServiceLib.Models.VmMigrationRequest vmMigrationRequest = CMP.FetchMigrationRequest(key);

                    if (null == vmMigrationRequest)
                        return null;

                    var vmr = new List<CmpInterfaceModel.Models.VmMigrationRequest>();
                    vmr.Add(ServToInt(vmMigrationRequest));
                    return SingleResult.Create(vmr.AsQueryable());
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in GetVmMigrationRequest()", 100, 100);
                throw;
            }

            //return SingleResult.Create(db.VmMigrationRequests.Where(vmmigrationrequest => vmmigrationrequest.ID == key));
        }

        //*********************************************************************
        ///
        /// <summary>
        /// PUT odata/VmMigrations(5)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vmmigrationrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Put([FromODataUri] int key,
            CmpInterfaceModel.Models.VmMigrationRequest vmmigrationrequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != vmmigrationrequest.ID)
            {
                return BadRequest();
            }

            /*db.Entry(vmmigrationrequest).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VmMigrationRequestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(vmmigrationrequest);*/

            //////////////////////

            try
            {
                DateTime Now = DateTime.UtcNow;
                vmmigrationrequest.LastStatusUpdate = DateTime.UtcNow;

                CmpServiceLib.Models.VmMigrationRequest vmMigReq = IntToServ(vmmigrationrequest);

                CmpServiceLib.CmpService CMP = new CmpServiceLib.CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString);
                CMP.UpdateVmMigrationRequest(vmMigReq, null, true, null);

                return Updated(vmmigrationrequest);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in Put()", 100, 100);
                throw ex;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// POST odata/VmMigrations
        /// </summary>
        /// <param name="vmmigrationrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Post(CmpInterfaceModel.Models.VmMigrationRequest vmmigrationrequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cmp = new CmpServiceLib.CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString);
            var vmMigReq = IntToServ(vmmigrationrequest);

            if (0 == vmmigrationrequest.ID)
                vmmigrationrequest = ServToInt(cmp.InsertVmMigrationRequest(vmMigReq));
            else
                cmp.UpdateVmMigrationRequest(vmMigReq, null, true, null);

            return Created(vmmigrationrequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// PATCH odata/VmMigrations(5)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [Authorize]
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CmpInterfaceModel.Models.VmMigrationRequest> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CmpInterfaceModel.Models.VmMigrationRequest vmmigrationrequest = await db.VmMigrationRequests.FindAsync(key);
            if (vmmigrationrequest == null)
            {
                return NotFound();
            }

            patch.Patch(vmmigrationrequest);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VmMigrationRequestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(vmmigrationrequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// DELETE odata/VmMigrations(5)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //[Authorize]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CmpInterfaceModel.Models.VmMigrationRequest vmmigrationrequest = await db.VmMigrationRequests.FindAsync(key);
            if (vmmigrationrequest == null)
            {
                return NotFound();
            }

            db.VmMigrationRequests.Remove(vmmigrationrequest);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
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
                db.Dispose();
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

        private bool VmMigrationRequestExists(int key)
        {
            return db.VmMigrationRequests.Count(e => e.ID == key) > 0;
        }

        public string GetTemplateXML()
        {
          
            var configXMLTemplate = new VmConfig
            {
                DiskSpecList = new List<DiskSpec>()
                {
                    new DiskSpec()
                    {
                        DriveLetter = string.Empty,
                        LogicalDiskSizeInGB = string.Empty,
                        Lun =string.Empty,
                        Config = string.Empty,
                        MediaLink = string.Empty,
                        SourceVhdFile = string.Empty,
                        TagData = string.Empty
                    }
                },
                InfoFromVM = new InfoFromVmSpec()
                {
                    BiosAssetTag = string.Empty,
                    ClusteringStatus = string.Empty,
                    ComputerModel = string.Empty,
                    DataCenter = string.Empty,
                    ComputerName = string.Empty,
                    DriveCount = 0,
                    InstalledMemoryGb = 0,
                    MachineDomain = string.Empty,
                    MaxDriveSizeGB = 0,
                    NetworkAdapterCount = 0,
                    NumberOfCores = 0,
                    OperatingSystemVersion = string.Empty,
                    StaticIpAddressCount = 0,
                    TotalDriveSpaceGB = 0,
                    DriveLetters = string.Empty,
                    VmAddress = string.Empty,
                }
            };
            return configXMLTemplate.Serialize();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using CmpInterfaceModel.Models;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using AzureAdminClientLib;
using SMAApi;
using SMAApi.Interface;
using VhdInfo = CmpCommon.VhdInfo;
using VmInfo = CmpServiceLib.Models.VmInfo;
using CmpServiceLib.Models;

namespace CmpServiceLib
{
    public class CmpService : IDisposable
    {
        private static EventLog _eventLog = null;
        //ThreadStart _WorkerDelegate = null;
        readonly Thread _workerThread = null;
        int _SleepTime = 6000;
        string _cmpDbConnectionString = null;
        string _aftsDbConnectionString = null;
        string ANSWERFILE_SQL_TEMPLATE =
            @"$tempfolder=""{0}:\MSSQL11.MSSQLSERVER\MSSQL\Data""\n
            Set-Service MSSQLSERVER -startuptype ""manual""\n
            Set-Service SQLSERVERAGENT -startuptype ""manual""\n
            if (!(test-path -path $tempfolder)) {{\n
                New-Item -ItemType directory -Path $tempfolder\n
            }}\n
            $tempfolder=""{0}:\MSSQL12.MSSQLSERVER\MSSQL\Data""\n
            if (!(test-path -path $tempfolder)) {{\n
                New-Item -ItemType directory -Path $tempfolder\n
            }}\n
            $tempfolder=""{0}:\MSSQL\SSDCache""\n
            if (!(test-path -path $tempfolder)) {{\n
                New-Item -ItemType directory -Path $tempfolder\n
            }}\n
            Start-Service MSSQLSERVER\n
            Start-Service SQLSERVERAGENT";

        const string URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM = @"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/InstanceView?api-version={3}";

        public void Dispose()
        {
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="eventLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        ///  
        //*********************************************************************

        public CmpService(EventLog eventLog, 
            string cmpDbConnectionString, string aftsDbConnectionString)
        {
            _eventLog = eventLog;
            _cmpDbConnectionString = cmpDbConnectionString;
            _aftsDbConnectionString = aftsDbConnectionString;
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

        private void LogThis(Exception ex, EventLogEntryType type, 
            string prefix, int id, short category)
        {
            if (null != _eventLog)
                _eventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex), type, id, category);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(string message, EventLogEntryType type, 
            int id, short category)
        {
            if (null != _eventLog)
                _eventLog.WriteEntry(message, type, id, category);
        }

        #region Utils section ----------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        AzureAdminClientLib.Connection GetConnection(Models.VmDeploymentRequest vmDepReq)
        {
            if (null == vmDepReq.ServiceProviderAccountID)
                throw new Exception("ServiceProviderAccountID == NULL");

            var connection = ServProvAccount.GetAzureServiceAccountConnection(
                (int)vmDepReq.ServiceProviderAccountID, _cmpDbConnectionString);

            if (null == connection)
                throw new Exception("Unable to locate account for given ServiceProviderAccountID");

            return connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReqId"></param>
        /// <param name="vmDepReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        AzureAdminClientLib.Connection GetConnectionFromVmDepRqId(int vmDepReqId, out Models.VmDeploymentRequest vmDepReq)
        {
            vmDepReq = FetchVmDepRequest(vmDepReqId);

            if (null == vmDepReq)
                throw new Exception("Unable to locate VmDepReq for given vmDepReqId '" + vmDepReqId + "'");

            if (null == vmDepReq.ServiceProviderAccountID)
                throw new Exception("ServiceProviderAccountID == NULL");

            return GetConnection(vmDepReq);
        }

        #endregion

        #region --- Service Region ---------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Worker()
        {
            try
            {
                var xk = new KryptoLib.X509Krypto(null);
                _cmpDbConnectionString = xk.GetKTextConnectionString(
                    "CMPContext", "CMPContextPassword");
                xk.Dispose();

                xk = new KryptoLib.X509Krypto(null);
                _aftsDbConnectionString = xk.GetKTextConnectionString(
                    "AzureFileTransferContext", "AzureFileTransferContextPassword");
                xk.Dispose();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, 
                    "Exception in CmpService.Worker()", 100, 100);
                return;
            }

            while (true)
            {
                try
                {
                    var proc = new Processor(_eventLog);
                    proc.PerformSingleRun(_cmpDbConnectionString, _aftsDbConnectionString);
                }
                catch (Exception ex)
                {
                    LogThis(ex, EventLogEntryType.Error, 
                        "Exception in CmpService.Worker()", 100, 100);
                }

                Thread.Sleep(_SleepTime);
            }
        }

        Processor _Proc = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void AsynchStart()
        {
            var xk = new KryptoLib.X509Krypto(null);
            _cmpDbConnectionString = xk.GetKTextConnectionString(
                "CMPContext", "CMPContextPassword");
            xk.Dispose();

            //*** Uncomment this only if you intend to support VM migrations ***
            //xk = new KryptoLib.X509Krypto(null);
            //_aftsDbConnectionString = xk.GetKTextConnectionString(
            //    "AzureFileTransferContext", "AzureFileTransferContextPassword");
            //xk.Dispose();

            //*** Uncomment this only temporarily to view conn string at runtime ***
            //LogThis("_CmpDbConnectionString : " + _cmpDbConnectionString, EventLogEntryType.Information, 1, 1);
            //LogThis("_AftsDbConnectionString : " + _aftsDbConnectionString, EventLogEntryType.Information, 1, 1);

            _Proc = new Processor(_eventLog);
            _Proc.AsynchStart(_cmpDbConnectionString, _aftsDbConnectionString);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void AsynchStop()
        {
            if (null != _Proc)
                _Proc.AsynchStop();

            if (null != _workerThread)
                _workerThread.Abort();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Runs a single thread, this is useful for dubugging.
        /// </summary>
        /// 
        //*********************************************************************

        public void PerformSingleRun()
        {
            var xk = new KryptoLib.X509Krypto(null);
            _cmpDbConnectionString = xk.GetKTextConnectionString(
                "CMPContext", "CMPContextPassword");
            xk.Dispose();

            //*** Uncomment this only if you intend to support VM migrations ***
            //xk = new KryptoLib.X509Krypto(null);
            //_aftsDbConnectionString = xk.GetKTextConnectionString(
            //    "AzureFileTransferContext", "AzureFileTransferContextPassword");
            //xk.Dispose();

            //*** Uncomment this only temporarily to view conn string at runtime ***
            //LogThis("_CmpDbConnectionString : " + _cmpDbConnectionString, EventLogEntryType.Information, 1, 1);
            //LogThis("_AftsDbConnectionString : " + _aftsDbConnectionString, EventLogEntryType.Information, 1, 1);

            var proc = new Processor(_eventLog);
            proc.PerformSingleRun(_cmpDbConnectionString, _aftsDbConnectionString);
        }

        #endregion


        #region --- VmDepRequest Region ----------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetchVmDepRequests()
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchVmDepRequests("", true);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        public Models.VmDeploymentRequest FetchVmDepRequest(int deploymentRequestId)
        {
            try
            {
                var cdb = new CmpDb(_cmpDbConnectionString);
                return cdb.FetchVmDepRequest(deploymentRequestId);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetcVmDeploymentRequestByName(string vmName)
        {
            try
            {
                var cdb = new CmpDb(_cmpDbConnectionString);
                return cdb.FetchVmDepRequestsByVmName(vmName);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="db"></param>
        ///  <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************

        public void UpdateVmDepRequest(Models.VmDeploymentRequest vmRequest, 
            List<string> warningList)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            cdb.SetVmDepRequestStatus(vmRequest, warningList);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRequest"></param>
        /// 
        //*********************************************************************

        public void ResubmitVmDepRequest(Models.VmDeploymentRequest vmRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            cdb.ResubmitVmDepRequest(vmRequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// 
        //*********************************************************************

        public bool IsVmDepRequestInProcess(string serverName)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.IsVmDepRequestInProcess(serverName);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmMigRequest"></param>
        /// 
        //*********************************************************************

        public void UpdateVmDepRequest(Models.VmMigrationRequest vmMigRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);

            //*** fetch the vmReq
            var vmRequest = cdb.FetchVmDepRequest(vmMigRequest.VmDeploymentRequestID);

            //*** update values
            vmRequest.StatusCode = vmMigRequest.StatusCode;
            vmRequest.StatusMessage = vmMigRequest.StatusMessage;
            vmRequest.ExceptionMessage = vmMigRequest.ExceptionMessage;
            vmRequest.SourceVhdFilesCSV = vmMigRequest.SourceVhdFilesCSV;
            vmRequest.CurrentStateStartTime = vmMigRequest.CurrentStateStartTime;

            //*** update the vmReq
            cdb.SetVmDepRequestStatus(vmRequest, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.VmDeploymentRequest InsertVmDepRequest(
            Models.VmDeploymentRequest vmDepRequest)
        {
           
            var cdb = new CmpDb(_cmpDbConnectionString);
            vmDepRequest = cdb.InsertVmDepRequest(vmDepRequest);

            if (vmDepRequest.RequestType.Equals(
                CmpInterfaceModel.Constants.RequestTypeEnum.MigrateVm.ToString()))
                InsertVmMigrationRequest(vmDepRequest, cdb);
           
            return vmDepRequest;
        }



        #endregion

        #region --- OpsQueue Region ----------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetchOpsRequests()
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchVmDepRequests("", true);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        public Models.VmDeploymentRequest FetchOpsRequest(int deploymentRequestId)
        {
            try
            {
                var cdb = new CmpDb(_cmpDbConnectionString);
                return cdb.FetchVmDepRequest(deploymentRequestId);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="db"></param>
        ///  <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************

        public void UpdateOpsRequest(Models.VmDeploymentRequest vmRequest, 
            List<string> warningList)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            cdb.SetVmDepRequestStatus(vmRequest, warningList);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRequest"></param>
        /// 
        //*********************************************************************

        public void ResubmitOpsRequest(Models.VmDeploymentRequest vmRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            cdb.ResubmitVmDepRequest(vmRequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// 
        //*********************************************************************

        public bool IsOpsRequestInProcess(string serverName)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.IsVmDepRequestInProcess(serverName);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmMigRequest"></param>
        /// 
        //*********************************************************************

        public void UpdateOpsRequest(Models.VmMigrationRequest vmMigRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);

            //*** fetch the vmReq
            var vmRequest = cdb.FetchVmDepRequest(vmMigRequest.VmDeploymentRequestID);

            //*** update values
            vmRequest.StatusCode = vmMigRequest.StatusCode;
            vmRequest.StatusMessage = vmMigRequest.StatusMessage;
            vmRequest.ExceptionMessage = vmMigRequest.ExceptionMessage;
            vmRequest.SourceVhdFilesCSV = vmMigRequest.SourceVhdFilesCSV;
            vmRequest.CurrentStateStartTime = vmMigRequest.CurrentStateStartTime;

            //*** update the vmReq
            cdb.SetVmDepRequestStatus(vmRequest, null);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="opsRequest"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public Models.OpRequest InsertOpsRequest(Models.OpRequest opsRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            opsRequest = cdb.InsertOpsRequest(opsRequest);

            return opsRequest;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public CmpInterfaceModel.Models.OpSpec SubmitOpToQueue(
            CmpInterfaceModel.Models.OpSpec opReq)
        {
            var now = DateTime.UtcNow;

            var oR = new Models.OpRequest
            {
                Active = true,
                RequestType = opReq.Opcode,
                RequestName = opReq.Name,
                RequestDescription = opReq.Name,
                Config = CmpCommon.Utilities.Serialize(typeof(CmpInterfaceModel.Models.OpSpec), opReq),
                LastStatusUpdate = now,
                Id = 0,
                ExceptionMessage = null,
                CurrentStateTryCount = 0,
                CurrentStateStartTime = now,
                ServiceProviderStatusCheckTag = null,
                StatusCode = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                StatusMessage = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                TagData = null,
                TagID = 0,
                TargetName = opReq.TargetName,
                TargetTypeCode = opReq.TargetType,
                Warnings = null,
                WhenRequested = now,
                WhoRequested = opReq.Requestor
            };

            oR = InsertOpsRequest(oR);
            opReq.Id = oR.Id;

            return opReq;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDeploymentRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.OpRequest FetchVmOpRequest(string vmDeploymentRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchVMOpRequest(vmDeploymentRequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDeploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public bool CheckVmDeploymentRequest(int vmDeploymentRequestId)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.CheckVmDepRequest(vmDeploymentRequestId);
        }

        #endregion

        #region --- VmOps Region -------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of detached disks in a VM's subscription
        /// </summary>
        /// <param name="cmpRequestId">The VM whose subscription to check</param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public IEnumerable<VhdInfo> GetDetachedDisks(int? cmpRequestId)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId ?? -1, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            return vmo.GetDetachedDisks().Select(d => new VhdInfo
                {
                    DiskName = d.DiskName,
                });
        }
        
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// 
        //*********************************************************************

        public string VmRestart(int cmpRequestId)
        {
            var restartStatusUrl = "";
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);


            // supress the monitoring
            /*SetMaintenanceMode(vmDepReq.TargetVmName, DateTime.UtcNow, 
                DateTime.UtcNow.AddMinutes(60), MaintenanceModeReason.UnplannedOther, 
                "Delete the Azure VM from WAPRP");*/

           restartStatusUrl = vmo.Restart(vmDepReq.TargetVmName, 
               vmDepReq.TargetServicename).StatusCheckUrl;

           //ClearMaintenanceMode(vmDepReq.TargetVmName);

           return restartStatusUrl;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="statusCheckUrl"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse CheckOpsStatus(int cmpRequestId, string statusCheckUrl)
        {
            var url = statusCheckUrl;
            string body = null;
            var nullUrl = false;

            if (null == statusCheckUrl)
                nullUrl = true;
            else if (0 == statusCheckUrl.Length)
                nullUrl = true;

            if (nullUrl)
            {
                var ret = new HttpResponse
                {
                    HadError = true, 
                    Body = "statusCheckUrl is null or empty"
                };

                return ret;
            }

            Models.VmDeploymentRequest vmDepReq;

            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);
            var hi = new HttpInterface(connection);
            return hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, body);
        }

        //*********************************************************************
        /// 
        ///   <summary>
        ///  
        ///   </summary>
        ///   <param name="vmName"></param>
        /// <param name="resourceGroupName"></param>
        ///  
        //*********************************************************************
        //public string CheckVmOperationStatusArm(int cmpRequestId, string statusCheckUrl)
        //{
        //    try
        //    {
        //        Models.VmDeploymentRequest vmDepReq;
        //        var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);
        //        var hi = new HttpInterface(connection);
        //        var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, statusCheckUrl);

        //        if (resp.HadError)
        //            throw new Exception(resp.Body);

        //        var vmInfoJson = resp.Body;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Exception in GetVmOperationStatusArm() :" + Utilities.UnwindExceptionMessages(ex));
        //    }
        //}

        //*********************************************************************
        ///
        /// <summary>
        /// StartVM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// 
        //*********************************************************************

        public string VmStart(int cmpRequestId)
        {
            try
            {
                Models.VmDeploymentRequest vmDepReq = null;
                var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

                var vmo = new AzureAdminClientLib.VmOps(connection);
                var statusCheckUrl = vmo.Start(vmDepReq.TargetVmName, vmDepReq.TargetServicename);

                if (statusCheckUrl != null)
                {
                    //clear maint mode
                    //ClearMaintenanceMode(vmDepReq.TargetVmName);
                }

                return statusCheckUrl;
            }
            catch(Exception)
            {
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// StopVM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// 
        //*********************************************************************

        public string VmStop(int cmpRequestId)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);
            var domain = Utilities.GetXmlInnerText(vmDepReq.Config, "Domain");          

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var statusCheckUrl = vmo.Stop(vmDepReq.TargetVmName, vmDepReq.TargetServicename);

            if(statusCheckUrl != null)
            {
                // supress the monitoring for 48 hours
                /*SetMaintenanceMode(vmDepReq.TargetVmName, DateTime.UtcNow, 
                    DateTime.UtcNow.AddDays(2), MaintenanceModeReason.UnplannedOther, 
                    "Stop the Azure VM from WAPRP");*/
            }

            return statusCheckUrl;

            //return string.Format(URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM, connection.SubcriptionID, vmDepReq.TargetServicename, vmDepReq.TargetVmName, "2015-06-15");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// DeallocateVM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// 
        //*********************************************************************

        public string VmDeallocate(int cmpRequestId)
        {
            try
            {
                Models.VmDeploymentRequest vmDepReq = null;
                var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

                var vmo = new AzureAdminClientLib.VmOps(connection);
                var statusCheckUrl = vmo.Deallocate(vmDepReq.TargetVmName, vmDepReq.TargetServicename);

                if (statusCheckUrl != null)
                {
                    // supress the monitoring for 48 hours
                    //SetMaintenanceMode(vmDepReq.TargetVmName, DateTime.UtcNow, DateTime.UtcNow.AddDays(2), MaintenanceModeReason.UnplannedOther, "Deallocate the Azure VM from WAPRP");
                }

                return statusCheckUrl;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        ///  AddDisk
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// 
        //*********************************************************************
        public string VmAddDisk(int cmpRequestId, List<VhdInfo> disks)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var lstdisks = disks.Select(d => new AzureAdminClientLib.VhdInfo
            {
                HostCaching = d.HostCaching,                
                LogicalDiskSizeInGB = d.LogicalDiskSizeInGB,
                MediaLink = d.MediaLink
            }).ToList();
            return vmo.AddDisk(vmDepReq.TargetVmName, vmDepReq.TargetServicename, lstdisks);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// <param name="deleteFromStorage"></param>
        /// 
        //*********************************************************************

        public string DetachDisk(int? cmpRequestId, VhdInfo disk, bool deleteFromStorage)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId ?? -1, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            return vmo.DetachDisk(vmDepReq.TargetVmName, vmDepReq.TargetServicename, 
                disk.DiskName, deleteFromStorage);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Attaches an available disk to the specified VM.
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// 
        //*********************************************************************

        public string AttachExistingDisk(int? cmpRequestId, VhdInfo disk)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId ?? -1, 
                out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            return vmo.AddExistingDisk(vmDepReq.TargetVmName, 
                vmDepReq.TargetServicename, disk.DiskName);
        }


        //*********************************************************************
        ///
        ///  <summary>
        ///  GrowDisk
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// 
        //*********************************************************************
        public void VmGrowDisk(int cmpRequestId, List<VhdInfo> disks)
        {
            //throw new NotImplementedException();
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var lstdisks = disks.Select(d => new AzureAdminClientLib.VhdInfo
            {
                DiskLabel = d.DiskLabel,
                DiskName = d.DiskName,
                HostCaching = d.HostCaching,
                LogicalDiskSizeInGB = d.LogicalDiskSizeInGB,
                MediaLink = d.MediaLink
            }).ToList();

           // vmo.GrowDisks(vmDepReq.TargetVmName, vmDepReq.TargetServicename, lstdisks);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="roleSizeName"></param>
        ///  
        //*********************************************************************
        public int FetchDiskCount(int cmpRequestId, out string roleSizeName)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);
            var vmo = new AzureAdminClientLib.VmOps(connection);

            return vmo.FetchDiskCount(vmDepReq.TargetVmName, 
                vmDepReq.TargetServicename, out roleSizeName);
        }

        ////*********************************************************************
        /////
        /////  <summary>
        /////  ResizeVm
        /////  </summary>
        /////  <param name="cmpRequestId"></param>
        ///// <param name="size"></param>
        ///// 
        ////*********************************************************************

        //public string VmResize(int cmpRequestId,string size)
        //{
        //    Models.VmDeploymentRequest vmDepReq = null;
        //    var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

        //    var vmo = new AzureAdminClientLib.VmOps(connection);
        //    return vmo.Resize(vmDepReq.TargetVmName, vmDepReq.TargetServicename, size);
        //}

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="powershellScripts"></param>
        /// <param name="vmReq"></param>
        /// 
        //*********************************************************************

        private void ExecutePowerShellScript(List<string> powershellScripts, 
            Models.VmDeploymentRequest vmReq)
        {

            PowershellLib.Remoting psRem = null;

            try
            {
                psRem = GetPowershellConnection(vmReq);
            }
            catch (Exception)
            {
                throw new Exception("Unable to contact WinRM on target");
            }

            if (null == psRem)
                throw new Exception("Unable to contact WinRM on target");

            try
            {
                var rr = psRem.Execute(null, powershellScripts);
                if (rr.HasErrors)
                    foreach (var ed in rr.ErrorDecsriptionList)
                        throw new Exception(ed);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private PowershellLib.Remoting GetPowershellConnection(Models.VmDeploymentRequest vmReq)
        {
            try
            {
                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                var tempHostName = vmReq.TargetVmName + ".cloudapp.net";
                //var tempDomain = vmc.AzureVmConfig.Name;
                var tempDomain = Utilities.GetXmlInnerText(vmReq.Config, "ComputerName");
                var tempAccountName = Utilities.GetXmlInnerText(vmReq.Config, "AdminUsername");
                var tempPassword = Utilities.GetXmlInnerText(vmReq.Config, "AdminPassword");
                string remotePsUrl = null;

                var connection =
                    ServProvAccount.GetAzureServiceAccountConnection(
                    Convert.ToInt32(vmReq.ServiceProviderAccountID), _cmpDbConnectionString);

                if (null == tempAccountName)
                {
                    var defaultRemotingAccount =
                        Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultRemotingAccount");

                    if (null != defaultRemotingAccount)
                    {
                        var xk = new KryptoLib.X509Krypto(null);
                        defaultRemotingAccount = xk.DecrpytKText(defaultRemotingAccount);

                        var accountParts = defaultRemotingAccount.Split(',');

                        if (2 < accountParts.Count())
                        {
                            tempDomain = accountParts[0];
                            tempAccountName = accountParts[1];
                            tempPassword = accountParts[2];
                        }
                    }

                    string ipAddress;

                    remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                        connection.SubcriptionID, connection.Certificate, connection.AdToken, vmReq.TargetServicename, vmReq.TargetVmName,
                        PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.Private, out ipAddress);

                    try
                    {
                        return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" + tempAccountName, tempPassword);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(
                            "Using 'DefaultRemotingAccount' ({0}\\{1}) : {2}", tempDomain, tempAccountName, ex.Message));
                    }
                }

                //*** Try to connect to private network RPC port ***

                string ipAddress2;

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PrivateHttps, out ipAddress2);

                if (null != remotePsUrl)
                {
                    try
                    {
                        return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" + tempAccountName, tempPassword);
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                    }
                }

                //*** Try to connect to public network RPC port ***

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps, out ipAddress2);

                if (null == remotePsUrl)
                    throw new Exception("Not Found");

                return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" +
                    tempAccountName, tempPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessorVm.GetPowershellConnection() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        ///  ResizeVm
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="size"></param>
        /// 
        //*********************************************************************

        public string VmResize(int cmpRequestId, string size)
        {
            Models.VmDeploymentRequest vmDepReq = null;

            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);
            var vmInfo = VmGet(cmpRequestId);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var resize = vmo.Resize(vmDepReq.TargetVmName, vmDepReq.TargetServicename, size);

            var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmDepReq.Config);

            // once resize is done. Need to check if the VM is D-Series SQL and then impliment the sql standards
            if (size.Contains("Standard_D") && !vmInfo.RoleSize.Contains("Standard_D"))
            {
                // move the page file

                // check if the sql is required to install or not
                if (vmc.ApplicationConfig!= null && vmc.ApplicationConfig.SqlConfig != null && 
                    vmc.ApplicationConfig.SqlConfig.InstallSql == true)
                {
                    // update the sql answer file and run the script                   
                    ANSWERFILE_SQL_TEMPLATE = ANSWERFILE_SQL_TEMPLATE.Replace("{0}", 
                        vmc.PageFileConfig.DiskName);

                    var scriptList = new List<string>
                                                {
                                                    "New-Item -ItemType Directory -Force -Path C:\\SQLStartup",
                                                    "Set-Location c:\\SQLStartup",
                        string.Format("'{0}' | Out-File SQL-Startup.ps1", ANSWERFILE_SQL_TEMPLATE),
                        @"Schtasks.exe /Create /RU 'NT AUTHORITY\\SYSTEM' /F /SC OnStart /Delay 0000:30 /TN 'SqlTempdriveAndStartup' /TR ""powershell.exe -NoLogo -NonInteractive -ExecutionPolicy ByPass -Command 'c:\\SQLStartup\\SQL-Startup.ps1'"""
                                                };

                    // execute the powershell script
                    ExecutePowerShellScript(scriptList, vmDepReq);

                    // restart
                    scriptList = new List<string>(1) { @"Restart-Computer -force" };
                    ExecutePowerShellScript(scriptList, vmDepReq);
                }
            }
            return resize;
        }

      
        //*********************************************************************
        ///
        /// <summary>
        /// Get VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// 
        //*********************************************************************

        public VmInfo  VmGet(int cmpRequestId)
        {
            Models.VmDeploymentRequest vmDepReq = null;
           
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var role = vmo.GetVM(vmDepReq.TargetVmName, vmDepReq.TargetServicename);

            var roleInstance = role.Deployment.RoleInstanceList.FirstOrDefault();

            if (null == roleInstance)
                return null;

            var vm = new VmInfo
            {
                RoleName = role.RoleName,
                DataVirtualHardDisks = role.DataVirtualHardDisks,
                OSVirtualHardDisk =role.OSVirtualHardDisk,
                InternalIP = roleInstance.IpAddress,
                RDPCertificateThumbprint=roleInstance.RemoteAccessCertificateThumbprint,
                DNSName = role.Deployment.Url,
                RoleSize = roleInstance.InstanceSize,
                Status=roleInstance.InstanceStatus.ToString(),
                DeploymentID = role.Deployment.PrivateID,
                MediaLocation = new Uri(role.OSVirtualHardDisk.MediaLink),
                OSVersion = null, 
                Subscription =new SubscriptionInfo 
                {
                SubscriptionID = role.Subscription.SubscriptionID,
                SubscriptionName = role.Subscription.SubscriptionName,
                MaximumCoreCount = role.Subscription.MaximumCoreCount.ToString(),
                CurrentCoreCount = role.Subscription.CurrentCoreCount.ToString()
                }
            };

            return vm;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="netServiceName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public int FetchServicePort(int cmpRequestId, string netServiceName)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var vmo = new AzureAdminClientLib.VmOps(connection);
            var resp = vmo.FetchServicePort(vmDepReq.TargetVmName, 
                vmDepReq.TargetServicename, netServiceName);

            return resp;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Deletes Vm and also associated computer from AD
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="deleteFromStorage"></param>
        /// <param name="thowIfNotFound"></param>
        ///  
        //*********************************************************************

        public string VmDelete(int cmpRequestId, bool deleteFromStorage, bool thowIfNotFound)
        {
            Models.VmDeploymentRequest vmDepReq = null;
            var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

            var domain = Utilities.GetXmlInnerText(vmDepReq.Config, "Domain");          
           
            var vmo = new AzureAdminClientLib.VmOps(connection);
            var resp = vmo.Delete(vmDepReq.TargetVmName, 
                vmDepReq.TargetServicename, deleteFromStorage, thowIfNotFound);

            return resp.StatusCheckUrl;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Deletes VM with Itsmrecord and also associated computer from AD
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="deleteFromStorage"></param>
        /// <param name="thowIfNotFound"></param>
        /// <param name="isItsmrecordDelete"></param>
        ///  
        //*********************************************************************

        public void VmDelete(int cmpRequestId, bool deleteFromStorage,
            bool thowIfNotFound,bool isItsmrecordDelete)
        {
            try
            { 
                Models.VmDeploymentRequest vmDepReq = null;
                var connection = GetConnectionFromVmDepRqId(cmpRequestId, out vmDepReq);

                var domain = Utilities.GetXmlInnerText(vmDepReq.Config, "Domain"); 

                var vmo = new AzureAdminClientLib.VmOps(connection);
                var resp = vmo.Delete(vmDepReq.TargetVmName, 
                    vmDepReq.TargetServicename, deleteFromStorage, thowIfNotFound);

                if(!resp.HadError)
                {
                   
                    DeleteComputerfromAD(vmDepReq.TargetVmName, domain);

                    if (isItsmrecordDelete)
                    {
                        if (deleteFromStorage)
                        {
                            //set status to Archive                        
                            //ItsmInterface.CreateCIwithArchive(vmDepReq, "Retired");
                        }
                        else
                        {
                            //set status to Out of Service
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Exception in CmpService.VmDelete() : " +
                                  CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }
            
        //*********************************************************************
        /// 
        ///   <summary>
        ///   Delete computer from AD
        ///   </summary>
        ///   <param name="computerName"></param>
        ///  <param name="domain"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public void DeleteComputerfromAD(string computerName, string domain)
        {
            try
            {

                var ldapBase = "LDAP://" + domain +"/";
                var path = ldapBase + "RootDSE";
                var count = 0;
                var entry = new DirectoryEntry(path, null, null, AuthenticationTypes.Secure);
                var defaultNamingContext = entry.Properties["defaultNamingContext"][0].ToString();


                path = ldapBase + defaultNamingContext;
                var deBase = new DirectoryEntry(path, null, null, AuthenticationTypes.Secure);

                var dsLookForDomain = new DirectorySearcher(deBase);
                dsLookForDomain.Filter = "(&(cn=" + computerName + "))";
                dsLookForDomain.SearchScope = SearchScope.Subtree;
                dsLookForDomain.PropertiesToLoad.Add("cn");
                dsLookForDomain.PropertiesToLoad.Add("distinguishedName");

                var srcComputer = dsLookForDomain.FindAll();

                count = srcComputer.Count;
                var cmpcount = count;

                // Deleting computer 
                DirectoryEntry computerToDel = null;
                foreach (SearchResult aComputer in srcComputer)
                {

                    computerToDel = aComputer.GetDirectoryEntry();
                    computerToDel.DeleteTree();
                    computerToDel.CommitChanges();
                }
                if (computerToDel != null)
                {
                    computerToDel.Close();
                    computerToDel.Dispose();
                }
                entry.Close();
                entry.Dispose();
                dsLookForDomain.Dispose();
        
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpService.DeleteComputerfromAD() : " + 
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- ServiceProviderAccount Region ----------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public CmpInterfaceModel.Models.ServiceProviderAccountSpec InsertServiceProviderAccount(
            CmpInterfaceModel.Models.ServiceProviderAccountSpec sPa)
        {
            var sPaInt = new CmpServiceLib.Models.ServiceProviderAccount(sPa);

            var cdb = new CmpDb(_cmpDbConnectionString);
            sPaInt = cdb.InsertServiceProviderAccount(sPaInt);

            sPa.ID = sPaInt.ID;

            return sPa;
        }

        #endregion

        #region --- MigrationRequest Region -------------------------------------------

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="migrationRequestId"></param>
        /// <param name="migrationRequestKeyType"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public Models.VmMigrationRequest FetchMigrationRequest(int migrationRequestId, 
            CmpDb.FetchMigrationRequestKeyTypeEnum migrationRequestKeyType = 
            CmpDb.FetchMigrationRequestKeyTypeEnum.MigReqId)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchMigrationRequest(migrationRequestId, migrationRequestKeyType);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqStatus"></param>
        /// <param name="reqAgentRegion"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequests(
            string reqStatus, string reqAgentRegion)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchMigrationRequests(reqStatus, reqAgentRegion);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqAgentRegion"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequests(string reqAgentRegion)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchMigrationRequests(reqAgentRegion);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqAgent"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequestsByAgent(string reqAgent)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.FetchMigrationRequestsByAgent(reqAgent);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmMigRequest"></param>
        ///  <param name="warningList"></param>
        /// <param name="updateVmDepRequest"></param>
        /// <param name="cdb"></param>
        ///  
        //*********************************************************************

        public void UpdateVmMigrationRequest(Models.VmMigrationRequest vmMigRequest,
            List<string> warningList, bool updateVmDepRequest, CmpDb cdb)
        {
            //UpdateVmDepRequest(vmMigRequest);

            var statusCode =
                (CmpInterfaceModel.Constants.StatusEnum)Enum.Parse(typeof(CmpInterfaceModel.Constants.StatusEnum), vmMigRequest.StatusCode);

            /*switch (statusCode)
            {
                case CmpInterfaceModel.Constants.StatusEnum.Converted:
                    vmMigRequest.StatusCode = CmpInterfaceModel.Constants.StatusEnum.ReadyForTransfer.ToString();
                    //InsertAftsRequest(vmMigRequest);
                    break;

                case CmpInterfaceModel.Constants.StatusEnum.QcVmRequest:
                    //*** update the VmReq ***
                    break;
            }*/

            if (null == cdb)
                cdb = new CmpDb(_cmpDbConnectionString);

            cdb.UpdateVmMigrationRequest(vmMigRequest, warningList);

            //*** Update VM Deployment Request ***

            if (updateVmDepRequest) if (0 < vmMigRequest.VmDeploymentRequestID)
                {
                    var vmDepReq = cdb.FetchVmDepRequest(vmMigRequest.VmDeploymentRequestID);
                    if (null != vmDepReq)
                    {
                        vmDepReq.StatusCode = vmMigRequest.StatusCode;
                        vmDepReq.StatusMessage = vmMigRequest.StatusMessage;
                        vmDepReq.Config = vmMigRequest.Config;
                        vmDepReq.SourceServerRegion = vmMigRequest.AgentRegion;
                        if (!string.IsNullOrEmpty(vmMigRequest.ExceptionMessage))
                        {
                            vmDepReq.ExceptionMessage = vmMigRequest.ExceptionMessage;
                        }
                        cdb.SetVmDepRequestStatus(vmDepReq, null);
                    }
                }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmDepRequest"></param>
        /// <param name="cdb"></param>
        ///  
        //*********************************************************************

        public void UpdateVmMigrationRequest(Models.VmDeploymentRequest vmDepRequest, CmpDb cdb)
        {
            if (null == cdb)
                cdb = new CmpDb(_cmpDbConnectionString);

            var vmMigRequest = cdb.FetchMigrationRequest(vmDepRequest.ID,
                CmpDb.FetchMigrationRequestKeyTypeEnum.DepReqId);

            if (null == vmMigRequest)
                throw new Exception("UpdateVmMigrationRequest() : Unable to locate VmMigReq with VmDepReqID == " + vmDepRequest.ID);

            vmMigRequest.StatusCode = vmDepRequest.StatusCode;
            vmMigRequest.StatusMessage = vmDepRequest.StatusMessage;
            vmMigRequest.Config = vmDepRequest.Config;

            UpdateVmMigrationRequest(vmMigRequest, null, false, cdb);
        }

        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmMigRequest"></param>
        /// <returns></returns>
        //*********************************************************************

        public Models.VmMigrationRequest InsertVmMigrationRequest(Models.VmMigrationRequest vmMigRequest)
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            return cdb.InsertVmMigrationRequest(vmMigRequest);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepRequest"></param>
        /// <param name="cDb"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private Models.VmMigrationRequest InsertVmMigrationRequest(
            Models.VmDeploymentRequest vmDepRequest, CmpDb cDb)
        {
            var vmMigRequest = new Models.VmMigrationRequest
            {
                ID = 0,
                VmDeploymentRequestID = vmDepRequest.ID,
                VmSize = vmDepRequest.VmSize,
                TagData = vmDepRequest.TagData,
                TagID = vmDepRequest.TagID,
                Config = vmDepRequest.Config,
                TargetVmName = vmDepRequest.TargetVmName,
                SourceServerName = vmDepRequest.SourceServerName,
                SourceVhdFilesCSV = null,
                ExceptionMessage = null,
                LastStatusUpdate = vmDepRequest.WhenRequested,
                StatusCode = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                StatusMessage = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                AgentRegion = vmDepRequest.SourceServerRegion,
                AgentName = null,
                CurrentStateStartTime = vmDepRequest.WhenRequested,
                CurrentStateTryCount = 0,
                Warnings = null,
                Active = true
            };

            return cDb.InsertVmMigrationRequest(vmMigRequest);
        }

        #endregion

        #region --- AFTS Region -------------------------------------------------------

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmDepRequest"></param>
        /// <param name="destination"></param>
        /// <param name="storageAccountKey"></param>
        /// <param name="deleteSourceAfterTransfer"></param>
        /// <param name="overwriteDestinationBlob"></param>
        ///  
        //*********************************************************************

        public void InsertAftsRequest(Models.VmDeploymentRequest vmDepRequest,
            string destination, string storageAccountKey,
            bool deleteSourceAfterTransfer, bool overwriteDestinationBlob)
        {
            var migReq = FetchMigrationRequest(vmDepRequest.ID, CmpDb.FetchMigrationRequestKeyTypeEnum.DepReqId);
            InsertAftsRequest(migReq, destination, storageAccountKey, 
                deleteSourceAfterTransfer, overwriteDestinationBlob);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmMigRequest"></param>
        /// <param name="destination"></param>
        /// <param name="storageAccountKey"></param>
        /// <param name="deleteSourceAfterTransfer"></param>
        /// <param name="overwriteDestinationBlob"></param>
        ///  
        //*********************************************************************

        public void InsertAftsRequest(Models.VmMigrationRequest vmMigRequest,
            string destination, string storageAccountKey, 
            bool deleteSourceAfterTransfer, bool overwriteDestinationBlob)
        {
            //*** Fetch matching VM Dep Request ***

            var vmDepRequest = FetchVmDepRequest(vmMigRequest.VmDeploymentRequestID);

            if (null == vmDepRequest)
            {
                vmMigRequest.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Exception.ToString();
                vmMigRequest.ExceptionMessage = "Conversion completed, but unable to locate matching VmDeployment Request.";
                return;
            }

            try
            {
                //*** Fix case confict (from PowerShell clients) ***

                vmMigRequest.Config = vmMigRequest.Config.Replace("<IsOS>True</IsOS>", "<IsOS>true</IsOS>");
                vmMigRequest.Config = vmMigRequest.Config.Replace("<IsOS>False</IsOS>", "<IsOS>false</IsOS>");

                //*** Get stuff ***
                //doing a test
               // var vmMigReqConfig = CmpInterfaceModel.Models.MigrationConfig.Deserialize(vmMigRequest.Config);
                var vmMigReqConfig = CmpInterfaceModel.Models.VmConfig.Deserialize(vmMigRequest.Config);
                if (null == vmMigReqConfig)
                    throw new Exception("Config data not found in migration request.");

                var whoRequested = "CMP:" + System.Environment.MachineName;
                //string destination = "";
                //string storageAccountKey = "";

                // Modify the blob destination to always be in a folder of the VM Name
                destination += "/" + vmMigReqConfig.AzureVmConfig.Name;

                //*** Submit request to AFTS ***

                var adb = new AftsDb(_aftsDbConnectionString);

                using (var db = new Models.AzureFileTransferContext())
                {
                    var cs = db.Database.Connection.ConnectionString;

                    foreach (var transferRequest in vmMigReqConfig.DiskSpecList.Select(diskSpec => new Models.Request
                    {
                        Config = "<Config>" + Utilities.Serialize( typeof(DiskSpec), diskSpec ) + "</Config>",
                        Destination = destination,
                        DestinationType = CmpInterfaceModel.Constants.AftsDestinationTypeEnum.BLOB.ToString(),
                        ElapsedTimeMinutes = 0,
                        MBytesTransferred = 0,
                        Name = "VmDepReq:" + vmDepRequest.RequestName + "-Disk:" + diskSpec.DriveLetter,
                        Notes = vmDepRequest.RequestDescription,
                        RateBytesSec = 0,
                        RequestID = adb.FetchMaxRequestId(db) + 1,
                        ResultDescription = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                        ResultStatusCode = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                        ResultTime = null,
                        Source = diskSpec.SourceVhdFile,
                        SourceMBytes = 0,
                        SourceType = CmpInterfaceModel.Constants.AftsSourceTypeEnum.FILE.ToString(),
                        StorageAccountKey = storageAccountKey,
                        TagID = vmDepRequest.ID,
                        TransferStartTime = null,
                        TransferType = CmpInterfaceModel.Constants.AftsTransferTypeEnum.FILETOBLOB.ToString(),
                        WhenRequested = DateTime.UtcNow,
                        WhoRequested = whoRequested,
                        WillTryAgain = true, 
                        AgentName = null,
                        AgentRegion = vmMigRequest.AgentRegion,
                        TagData = vmMigRequest.Config, 
                        DeleteSourceAfterTransfer = deleteSourceAfterTransfer, 
                        OverwriteDestinationBlob = overwriteDestinationBlob, 
                        Active = true
                    }))
                    {
                        adb.InsertTransferRequest(transferRequest, db);
                    }
                }

                //vmDepRequest.StatusCode = CmpInterfaceModel.Constants.StatusEnum.ReadyForTransfer.ToString();
                //vmDepRequest.StatusMessage = "Submitted to AFTS for file transfer";
            }
            catch (Exception ex)
            {
                //vmDepRequest.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Exception.ToString();
                //vmDepRequest.ExceptionMessage = "Exception while creating AFTS request(s) : " + Utilities.UnwindExceptionMessages(ex);
                //vmDepRequest.StatusMessage = vmDepRequest.ExceptionMessage;

                throw new Exception("Exception while creating AFTS request(s) : " + Utilities.UnwindExceptionMessages(ex));
            }

            //*** Update VmDepReq ***

            //UpdateVmDepRequest(vmDepRequest, null);
        }

        #endregion

        #region --- User Accounts Region ----------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.CmpServiceUserAccount> FetchUserAccounts()
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.FetchCmpServiceUserAccounts(null, true);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, 
                    "Exception in CmpService.FetchUserAccounts()", 100, 100);
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.ServiceProviderAccount> 
            FetchServiceProviderAccounts(string groupName)
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.FetchServiceProviderAccountList(groupName);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpService.FetchServiceProviderAccounts()", 100, 100);
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.ServiceProviderAccount>
            FetchServiceProviderAccounts(IEnumerable<int> idsToSearch)
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.FetchServiceProviderAccountList(idsToSearch).ToList();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpService.FetchServiceProviderAccounts()", 100, 100);
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.ServiceProviderAccount>
            FetchServiceProviderAccounts()
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.FetchServiceProviderAccountList().ToList();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpService.FetchServiceProviderAccounts()", 100, 100);
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ServiceProviderAccount InsertServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.InsertServiceProviderAccount(sPa);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpService.InsertServiceProviderAccount()", 100, 100);
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ServiceProviderAccount UpdateServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                if (null == _cmpDbConnectionString)
                {
                    var xk = new KryptoLib.X509Krypto(null);
                    _cmpDbConnectionString = xk.GetKTextConnectionString(
                        "CMPContext", "CMPContextPassword");
                }

                var cdb = new CmpServiceLib.CmpDb(_cmpDbConnectionString);
                return cdb.UpdateServiceProviderAccount(sPa);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpService.InsertServiceProviderAccount()", 100, 100);
                return null;
            }
        }

        #endregion
    }
}


//*****************************************************************************
// File: VmOpsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide methods for various virtual machine
//          operations. 
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using CmpCommon;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using Microsoft.WindowsAzurePack.CmpWapExtension.Common;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using CmpInterfaceModel.Models;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to provide methods for various virtual machine
    /// operations.
    /// </remarks>
    [PortalAPIExceptionHandler] // addsa - Web API attribute class to perform exception handling
    public class VmOpsController : ApiController
    {
        //*********************************************************************
        /// 
        ///  <summary>
        ///      This method is used to remove VM from VM List
        ///  </summary>
        //*********************************************************************
        private IVMServiceRepository _vmgr;
        private IEnumerable<CreateVm> vmList;
        static EventLog _eventLog = null;

        public VmOpsController()
            : this(new VMServiceRepository(_eventLog))
        {

        }

        public VmOpsController(IVMServiceRepository vMServiceManager)
        {
            // TODO: Complete member initialization
            _vmgr = vMServiceManager;
        }

        private void RemoveVmFromList(int vMId)
        {
            VmsController.RemoveVmFromList(vMId);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to convert VMOp type in OpSpec type
        ///  </summary>
        /// <param name="vmOp"></param>
        ///  
        //*********************************************************************

        public static OpSpec Translate(VmOp vmOp)
        {
            var cwdb = new CmpWapDb();
            var foundVmDepRequest = cwdb.FetchVmDepRequest(vmOp.VmId);

            if (null == foundVmDepRequest)
                
                throw new Exception("VM not found in DB");

            if (null == foundVmDepRequest.CmpRequestID)
                throw new Exception("CmpRequestID not found for given VM");

            if (vmOp.Name == null)
            {
                vmOp.Name = string.Format("{0} : {1}", vmOp.Opcode, foundVmDepRequest.TargetVmName);
            }

            List<CmpCommon.VhdInfo> diskList = null;

            if (vmOp.disks != null)
            {
                diskList = BuildDiskObject(vmOp.disks, foundVmDepRequest).ToList();
            }

            if (!String.IsNullOrEmpty(foundVmDepRequest.Domain))
            {
                vmOp.Name += "." + foundVmDepRequest.Domain;
                foundVmDepRequest.TargetVmName += "." + foundVmDepRequest.Domain;
            }

            return new OpSpec()
            {
                Config = vmOp.Config,
                Disks = diskList,
                Id = vmOp.Id,
                Name = vmOp.Name,
                Opcode = vmOp.Opcode,
                StatusCode = vmOp.StatusCode,
                StatusMessage = vmOp.StatusMessage,
                TargetId = (int)foundVmDepRequest.CmpRequestID,
                TargetName = foundVmDepRequest.TargetVmName,
                TargetType = "VM",
                Vmsize = vmOp.Vmsize,
                iData = vmOp.iData,
                sData = vmOp.sData
            };
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///      This method is used to submit VM operations to a queue to be
        ///      processed by CMP.
        ///  </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmOp"></param>
        ///  <returns>returns status code such as OK or bad request.</returns>
        ///  
        //*********************************************************************
        [HttpPost]
        public HttpResponseMessage SubmitOp(string subscriptionId, [FromBody] VmOp vmOp)
        {
            try
            {
                LogThis(EventLogEntryType.Information, "VmOp Request Submitted", 2, 1);

                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    var result = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("Model State is not valid", Encoding.UTF8, "application/json"),
                        ReasonPhrase = "ModelState is not valid"
                    };

                    return result;
                }

                CmpInterfaceModel.Constants.VmOpcodeEnum opCode;

                if (!Enum.TryParse(vmOp.Opcode, true, out opCode))
                    throw new Exception("Unknown opcode '" + vmOp.Opcode + "'");

                if (vmOp.IsMultiOp) // If is Multiple Operation
                {
                    vmList = _vmgr.FetchVms(subscriptionId);

                    foreach (var vm in vmList)
                    {
                        var opSpec = Translate(new VmOp { Opcode = vmOp.Opcode, VmId = vm.Id, sData = vmOp.sData, iData = vmOp.iData });
                        opSpec = new VMServiceRepository(_eventLog).SubmitOperation(opSpec);
                    }
                }
                else
                {
                    if (vmOp.Opcode != CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEONEXCEPTION.ToString())
                    {
                        var opSpec = Translate(vmOp);
                        opSpec = new VMServiceRepository(_eventLog).SubmitOperation(opSpec);
                    }
                    else
                    {
                        DeleteWapdbRecord(vmOp.VmId);
                    }
                }

                LogThis(EventLogEntryType.Information, "VmOp Request Submitted OK", 2, 2);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.SubmitOp()", 100, 1);

                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Exception while submitting request to OPS queue : " +
                        Regex.Replace(CmpCommon.Utilities.UnwindExceptionMessages(ex), @"\t|\n|\r", "")
                };
            }
        }

        [HttpGet]
        public IHttpActionResult GetVmOpsQueueStatus(string subscriptionId, int Id)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(Id);

                if (null != foundVmDepRequest)
                {
                    var cmpi = new VMServiceRepository(_eventLog);
                    var vmOpsInfo = cmpi.GetVmOpsRequestSpec(foundVmDepRequest.TargetVmName);

                    if (vmOpsInfo == null)
                    {
                        BadRequest("Can't retrieve ops queue status");
                    }
                    else
                    {
                        if (vmOpsInfo.StatusCode == CmpInterfaceModel.Constants.StatusEnum.Complete.ToString())
                        {
                            if (vmOpsInfo.Opcode == CmpInterfaceModel.Constants.VmOpcodeEnum.DELETE.ToString() ||  // Delete VM record from wap db if delete operation is selected
                                vmOpsInfo.Opcode == CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEFROMSTORAGE.ToString())
                            {
                                DeleteWapdbRecord(Id, cwdb);
                            }
                        }
                        return Ok(vmOpsInfo);
                    }
                }
                return BadRequest("Can't find VM deployment request data");
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VMOpsController.GetVMOpsQueueStatus()", 100, 1);
                return BadRequest(ex.Message);
            }
        }

        #region StartVM

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to start a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        ///  
        //*********************************************************************
        private void StartVm(int vMId)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.StartVm(Convert.ToInt32(foundVmDepRequest.CmpRequestID));
                    }


            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.StartVM()", 100, 1);
                throw;
            }
        }

        #endregion

        #region StopVM
        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to stop a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        ///  
        //*********************************************************************
        private void StopVm(int vMId)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.StopVm(Convert.ToInt32(foundVmDepRequest.CmpRequestID));
                    }


            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.StopVm()", 100, 1);
                throw;
            }
        }

        #endregion

        #region DeallocateVM
        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to deallocate a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        ///  
        //*********************************************************************
        private void DeallocateVm(int vMId)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.DeallocateVm(Convert.ToInt32(foundVmDepRequest.CmpRequestID));

                        //cmpi.SubmitOpToQueue()
                    }


            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.DeallocateVm()", 100, 1);
                throw;
            }
        }

        #endregion

        #region ResizeVm

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to resize a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        ///  <param name="size"></param>
        ///  
        //*********************************************************************
        private void ResizeVm(int vMId, string size)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.ResizeVM(Convert.ToInt32(foundVmDepRequest.CmpRequestID), size);
                    }

                cwdb.UpdateVmSize(vMId, size);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.ResizeVM()", 100, 1);
                throw;
            }
        }

        #endregion

        #region AddDisk

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to check whether a disk can be added to a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="addDiskCount"></param>
        /// <returns>boolean</returns>
        /// 
        //*********************************************************************

        private bool CanAddDisks(int cmpRequestId, int addDiskCount)
        {
            try
            {
                string roleSizeName;

                //*** Fetch disk count from Azure ***

                var cmpi = new VMServiceRepository(_eventLog);
                var count = cmpi.FetchDiskCount(cmpRequestId, out roleSizeName);

                //*** fetch role size disc capacity info ***

                var cwdb = new CmpWapDb();
                var roleSizeInfo = cwdb.FetchVmSizeInfo(roleSizeName);

                if (null == roleSizeInfo)
                    throw new Exception(string.Format(
                        "Could not locate given VM role size: '{0}' in server Role Size table", roleSizeName));

                //*** Check capacity ***

                return count + addDiskCount <= roleSizeInfo.MaxDataDiskCount;
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.CanAddDisks()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to add disk to a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        /// <param name="disks"></param>
        ///  
        //*********************************************************************
        private void AddDisk(int vMId, List<object> disks)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        if (!CanAddDisks((int)foundVmDepRequest.CmpRequestID, disks.Count))
                            throw new Exception("Role size of given VM does not support the requested number of data disks");

                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.AddDisk(Convert.ToInt32(foundVmDepRequest.CmpRequestID), BuildDiskObject(disks, foundVmDepRequest).ToList());
                    }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.AddDisk()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to build a disk object.
        /// </summary>
        /// <param name="jsonDisks"></param>
        /// <param name="vmReq"></param>
        /// <returns>VHD object</returns>
        /// 
        //*********************************************************************

        private static IEnumerable<VhdInfo> BuildDiskObject(IEnumerable<object> jsonDisks, Models.CmpRequest vmReq)
        {
            var config = new XmlDocument();
            config.LoadXml(vmReq.Config);
            XmlNodeList nodes = config.SelectNodes(@"/VmConfig/Placement/StorageContainerUrl");

            foreach (var jsonDisk in jsonDisks)
            {
                var disks = JsonConvert.DeserializeObject<IEnumerable<VhdInfo>>(jsonDisk.ToString());
                foreach (var disk in disks)
                {
                    disk.MediaLink = nodes[0].InnerText;
                    yield return disk;
                }
            }
        }
        #endregion

        #region DeleteVm
        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to delete a VM.
        ///  </summary>
        ///  <param name="vMId"></param>
        /// <param name="deleteFromStorage"></param>
        ///  
        //*********************************************************************

        private void DeleteVm(int vMId, bool deleteFromStorage)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest)
                {
                    var cmpi = new VMServiceRepository(_eventLog);
                    cmpi.DeleteVm(Convert.ToInt32(foundVmDepRequest.CmpRequestID), deleteFromStorage);
                }

                DeleteWapdbRecord(vMId, cwdb);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.DeleteVm()", 100, 1);
                throw;
            }
        }

        #endregion

        #region DetachDisks
        private void DetachDisks(int vMId, IList<object> disks, bool deleteFromStorage)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (foundVmDepRequest != null)
                {
                    var cmpi = new VMServiceRepository(_eventLog);
                    var disk = disks.Select(d => new VhdInfo { DiskName = d.ToString() }).FirstOrDefault();
                    cmpi.DetachDisk(foundVmDepRequest.CmpRequestID, disk, deleteFromStorage);
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.DetachDisks()", 100, 1);
                throw;
            }
        }
        #endregion

        #region AttachExistingDisks
        private void AttachExistingDisks(int vMId, IList<object> disks)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (foundVmDepRequest != null)
                {
                    var cmpi = new VMServiceRepository(_eventLog);
                    var disk = disks.Select(d => new VhdInfo { DiskName = d.ToString() }).FirstOrDefault();
                    cmpi.AttachExistingDisk(foundVmDepRequest.CmpRequestID, disk);
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.AttachExistingDisks()", 100, 1);
                throw;
            }
        }
        #endregion

        #region RestartVM
        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to restart a VM.
        /// </summary>
        /// <param name="vMId"></param>
        /// 
        //*********************************************************************

        private void RestartVm(int vMId)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vMId);

                if (null != foundVmDepRequest) if (null != foundVmDepRequest.CmpRequestID)
                    {
                        var cmpi = new VMServiceRepository(_eventLog);
                        cmpi.RebootVm((int)foundVmDepRequest.CmpRequestID);
                    }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.ResetVm()", 100, 1);
                throw;
            }
        }

        #endregion

        #region Delete WAP DB record
        private void DeleteWapdbRecord(int id, CmpWapDb cwdb = null)
        {
            try
            {
                if (cwdb == null)
                {
                    cwdb = new CmpWapDb();
                }
                cwdb.DeleteVmDepRequest(id);
                RemoveVmFromList(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region --- Utilities -------------------------------------------------

        /// <summary></summary>
        public static EventLog EventLog
        {
            set { _eventLog = value; }
            get
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application");
                        _eventLog.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
            }
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
            int id, short category)
        {
            try
            {
                if (null != EventLog)
                    EventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
        private void LogThis(EventLogEntryType type, string message,
            int id, short category)
        {
            try
            {
                if (null != EventLog)
                    EventLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }

        #endregion
    }
}

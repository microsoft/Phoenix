using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class WaitForRebootStage : Stage
    {
        public int ContactingVmMinutesTTL { get; set; }

        public int DeleteDwelltimeMinutes { get; set; }

        public Action<VmDeploymentRequest, int, bool, bool> DeleteVm { get; set; }

        public Func<VmDeploymentRequest, PowershellLib.Remoting> GetPowershellConnection { get; set; }

        public Func<VmDeploymentRequest, int, bool> HasTimedOut { get; set; }

        public Action<VmDeploymentRequest, string, CmpDb, bool, bool> ResubmitRequest { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            PowershellLib.Remoting psRem = null;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.WaitForReboot1.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessWaitForReboot1() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.WaitForReboot1.ToString());

                    //*** Experiment with dwell time ***
                    if (null != vmReq.CurrentStateStartTime)
                        if (2 > DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                            continue;

                    //*** Contact remote Powershell ***

                    try
                    {
                        psRem = GetPowershellConnection(vmReq);

                        var vmMtuSetting =
                            Microsoft.Azure.CloudConfigurationManager.GetSetting("VMMTUSETTING");

                        if (null != psRem && null != vmMtuSetting)
                            if (!vmMtuSetting.Equals("0"))
                            {
                                var commandList = new List<string>(4)
                                {
                                   @"$a = netsh int ipv4 sho int
                                    $maxNic = $a.count
                                    for ($i=3;$i -lt $maxNic-1;$i++) 
                                    { 
                                     $nic = ($a[$i].split("" "") | ?{!$_ -eq """"})
                                    if (([string]$nic[4]).contains(""Ethernet"") ) 
                                     {
  
                                     Netsh int ipv4 set subinterface $nic[0] mtu= "+vmMtuSetting+" store=persistent } }",
                                                                                   @"$a = netsh int ipv4 sho int
                                    $maxNic = $a.count
                                    for ($i=3;$i -lt $maxNic-1;$i++) 
                                    { 
                                     $nic = ($a[$i].split("" "") | ?{!$_ -eq """"})
                                    if (([string]$nic[4]).Contains(""Local"")-and ([string]$nic[5]).contains(""Area"") -and ([string]$nic[6]).contains(""Connection"") ) 
                                     {
  
                                     Netsh int ipv4 set subinterface $nic[0] mtu= "+vmMtuSetting+" store=persistent} }"

                                };

                                var rr = psRem.Execute(null, commandList);

                                var remoteErrorDescriptionList = new List<string>();

                                //remoteErrorDescriptionList.Add("Setting MTU :" + vmMtuSetting + "---" );

                                if (rr.HasErrors)
                                    remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed => "Setting MTU : " + ed));
                            }
                    }
                    catch (Exception ex)
                    {
                        if (CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex).Contains("Access is denied"))
                            throw;

                        if (HasTimedOut(vmReq, ContactingVmMinutesTTL))
                        {
                            if (vmReq.RequestType.Equals(
                                    CmpInterfaceModel.Constants.RequestTypeEnum.MigrateVm.ToString()))
                                throw;

                            DeleteVm(vmReq, DeleteDwelltimeMinutes, true, true);
                            ResubmitRequest(vmReq, "Timeout " + Constants.StatusEnum.WaitForReboot1, cdb, false,
                                false);
                        }

                        continue;
                    }

                    if (null == psRem)
                    {
                        if (HasTimedOut(vmReq, ContactingVmMinutesTTL))
                        {
                            DeleteVm(vmReq, DeleteDwelltimeMinutes, true, true);
                            ResubmitRequest(vmReq, "Timeout " + Constants.StatusEnum.WaitForReboot1, cdb, false, false);
                        }

                        //*** It can take time to spin up WinRM, if we can't connect then break out and try again next cycle
                        continue;
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    //vmReq.CurrentStateTryCount = 0;
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Contacted After Reboot";
                    vmReq.StatusCode = Constants.StatusEnum.CreatingDrives.ToString();
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    if (null == vmReq)
                    {
                        LogThis(null, EventLogEntryType.Error, "Exception in ProcessWaitForReboot1() : vmReq == null", 1, 1);
                        return -1;
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);
                    Utilities.SetVmReqExceptionType(vmReq,
                        CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                finally
                {
                    if (null != psRem)
                        psRem.Dispose();
                }
            }

            return 0;
        }
    }
}
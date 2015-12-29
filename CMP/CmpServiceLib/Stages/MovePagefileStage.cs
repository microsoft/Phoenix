using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;

namespace CmpServiceLib.Stages
{
    public class MovePagefileStage : Stage
    {
        public bool DisableSmartCardAuth { get; set; }

        public int DwellTime { get; set; }

        public Func<VmDeploymentRequest, PowershellLib.Remoting> GetPowershellConnection { get; set; }

        public bool IsMsitDeployment { get; set; }

        public bool MakeIpStatic { get; set; }

        public bool MovePagefile { get; set; }

        public string REBOOT_EXIT_TRAP_MATCH { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var remoteErrorDescriptionList = new List<string>();
            var remoteResultDescriptionList = new List<string>();
            PowershellLib.Remoting psRem = null;
            var restart = true;
            var appIdList = new HashSet<string>();

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.MovingPagefile.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessTransferedSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.MovingPagefile.ToString());

                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    //*** Which Azure API are we looking at here? ***
                    if (null == vmCfg.AzureApiConfig)
                        vmCfg.AzureApiConfig = null == vmCfg.AzureArmConfig
                            ? new AzureApiSpec() { Platform = Constants.AzureApiType.RDFE.ToString() }
                            : new AzureApiSpec() { Platform = Constants.AzureApiType.ARM.ToString() };

                    //*** Don't try ops on the same service twice in the same run ***
                    if (vmCfg.AzureApiConfig.Platform.Equals(Constants.AzureApiType.RDFE.ToString()))
                    {
                        if (appIdList.Contains(vmReq.ParentAppID))
                            continue;

                        appIdList.Add(vmReq.ParentAppID);
                    }

                    //*** Set status to 'MovingPagefile' ***
                    vmReq.StatusCode = Constants.StatusEnum.MovingPagefile.ToString();
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Moving Pagefile";
                    cdb.SetVmDepRequestStatus(vmReq, null);

                    try
                    {
                        psRem = GetPowershellConnection(vmReq);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (null == psRem)
                        throw new Exception("Unable to contact WinRM on target");

                    //*** TODO *** Let it settle for a moment, this may get rid of the random disk move failure error
                    Thread.Sleep(60000);

                    var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    List<string> commandList = null;
                    PowershellLib.RemotingResult rr = null;

                    //*** Force DNS registration now ***
                    commandList = new List<string>(1) { @"ipconfig /registerdns" };

                    rr = psRem.Execute(null, commandList);

                    if (rr.HasErrors)
                        remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed => "DNS Registration : " + ed));

                    //*** Fetch BIOS asset tag ***
                    commandList = new List<string>(1)
                    {
                       @"$computerSystemProduct = gwmi -Class Win32_computerSystemproduct -computername localhost -namespace ""root\CIMV2""
                         $computerSystemProduct.uuid"

                    };
                    rr = psRem.Execute(null, commandList);

                    if (rr.HasErrors)
                        remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed =>
                            "Fetch BIOS asset tag : " + ed));

                    if (null != rr.StringOutput)
                        foreach (var outLine in rr.StringOutput)
                        {
                            if (null == vmc.InfoFromVM)
                                vmc.InfoFromVM = new CmpInterfaceModel.Models.InfoFromVmSpec();
                            if (null == vmc.PostInfoFromVM)
                                vmc.PostInfoFromVM = new CmpInterfaceModel.Models.PostInfoFromVmSpec();

                            vmc.InfoFromVM.BiosAssetTag = outLine;
                            vmc.PostInfoFromVM.BiosAssetTag = outLine;
                            vmReq.Config = vmc.Serialize();
                        }

                    if (vmReq.RequestType.Equals(CmpInterfaceModel.Constants.RequestTypeEnum.NewVM.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {

                        if (IsMsitDeployment)
                        {
                            //*** Set firewall ***

                            commandList = new List<string>(9)
                            {
                                @"netsh advfirewall firewall set rule group=""File and Printer Sharing"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Remote Desktop"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Remote Event Log Management"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Windows Management Instrumentation (WMI)"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Remote Volume Management"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Remote Scheduled Tasks Management"" new enable=yess",
                                @"netsh advfirewall firewall set rule group=""Remote Service Management"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Windows Firewall Remote Management"" new enable=yes",
                                @"netsh advfirewall firewall set rule group=""Windows Remote Management"" new enable=yes"
                            };

                            rr = psRem.Execute(null, commandList);

                            if (rr.HasErrors)
                                remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed =>
                                    "Setting Firewall : " + ed));
                        }

                        //*** Activate Windows ***

                        commandList = new List<string>(1) { "slmgr.vbs /ato" };
                        rr = psRem.Execute(null, commandList);

                        if (rr.HasErrors)
                            remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed =>
                                string.Format("Windows Activation : {0}", ed)));

                        if (DisableSmartCardAuth)
                        {
                            //*** Disable smartcard task ***

                            commandList = new List<string>(1)
                            {
                                @"schtasks /Create /RU ""NT AUTHORITY\SYSTEM"" /F /SC ""OnStart"" /delay ""0001:00"" /TN ""ITCU-BuildSCDisable"" /TR ""cmd.exe /c reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\policies\system /v scforceoption /t REG_DWORD /d 0 /f"""
                            };

                            rr = psRem.Execute(null, commandList);

                            if (rr.HasErrors)
                                remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed =>
                                    "Disable smartcard task : " + ed));
                        }
                    }

                    if (MovePagefile)
                    {
                        //*** Move pagefile as needed ***

                        if (null != vmc.PageFileConfig)
                        {
                            //*** TODO : Fix the AMP client to set this ***
                            if (null == vmc.PageFileConfig.DiskName)
                                vmc.PageFileConfig.DiskName = "X";
                            if (1 != vmc.PageFileConfig.DiskName.Length)
                                vmc.PageFileConfig.DiskName = "X";

                            if (null == vmc.PageFileConfig.DiskName)
                                remoteErrorDescriptionList.Add("Null DiskName in PageFileConfig");
                            else if (1 != vmc.PageFileConfig.DiskName.Length)
                                remoteErrorDescriptionList.Add("DiskName length != 1 in PageFileConfig");
                            else
                            {
                                restart = true;
                                commandList = new List<string>(9)
                                {
                                    @"$ComputerSystem = Get-WmiObject -Class Win32_ComputerSystem -EnableAllPrivileges",
                                    @"$ComputerSystem.AutomaticManagedPagefile = $false",
                                    @"$ComputerSystem.Put()",
                                    @"Start-Sleep -s 2",
                                    @"$CurrentPageFile = Get-WmiObject -Class Win32_PageFileSetting",
                                    @"$CurrentPageFile.Delete()",
                                    @"Start-Sleep -s 2",
                                    @"Set-WmiInstance -Class Win32_PageFileSetting -Arguments @{Name=""c:\pagefile.sys""; InitialSize = 0; MaximumSize = 0}"
                                };

                                rr = psRem.Execute(null, commandList);

                                if (rr.HasErrors)
                                    remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed =>
                                        "Moving pagefile to C drive : " + ed));
                            }
                        }
                    }

                    //If this is ARM, we should skip this part about Making the IP static.
                    if (MakeIpStatic)
                    {
                        //*** Make IP static as needed ********************

                        if (vmc.Placement != null)
                            if (vmc.Placement.Config != null)
                            {
                                var pc = PlacementConfig.Deserialize(vmc.Placement.Config);
                                if (pc.UseStaticIpAddr)
                                {
                                    var connection =
                                        ServProvAccount.GetAzureServiceAccountConnection(
                                            Convert.ToInt32(vmReq.ServiceProviderAccountID), CmpDbConnectionString);

                                    var vmo = new AzureAdminClientLib.VmOps(connection);
                                    var ipstatic = vmo.MakeIpStatic(vmReq.TargetVmName, vmReq.TargetServicename);
                                    if (ipstatic != null)
                                    {

                                        vmc.InfoFromVM.VmAddress = ipstatic;
                                        vmc.PostInfoFromVM.VmAddress = ipstatic;
                                        vmReq.Config = vmc.Serialize();
                                    }
                                    appIdList.Add(vmReq.ParentAppID);

                                    restart = false;
                                }
                            }
                    }

                    //*** Reboot as needed ***

                    if (restart)
                    {
                        try
                        {
                            commandList = new List<string>(1) { @"Restart-Computer -force" };
                            psRem.Execute(null, commandList);
                            Thread.Sleep(DwellTime);
                        }
                        catch (Exception ex)
                        {
                            var message = CmpCommon.Utilities.UnwindExceptionMessages(ex);

                            if (!message.Contains(REBOOT_EXIT_TRAP_MATCH))
                                remoteErrorDescriptionList.Add("Exception during first VM restart (possibly benign) : " + message);
                        }
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Rebooting";
                    vmReq.StatusCode = Constants.StatusEnum.WaitForReboot1.ToString();
                    cdb.SetVmDepRequestStatus(vmReq, remoteErrorDescriptionList);
                }
                catch (Exception ex)
                {
                    var message = Utilities.UnwindExceptionMessages(ex);

                    if (ex.Message.Contains("(503)"))
                    {
                        vmReq.StatusMessage = "Retry on '(503)' condition";
                        cdb.SetVmDepRequestStatus(vmReq, null);
                        continue;
                    }

                    if (message.ToLower().Contains("retry"))
                    {
                        vmReq.StatusMessage = "Retry on 'Please retry the request' condition";
                        cdb.SetVmDepRequestStatus(vmReq, null);
                        continue;
                    }

                    if (message.Contains("requires exclusive access"))
                    {
                        vmReq.StatusMessage = "Retry on 'requires exclusive access' condition";
                        cdb.SetVmDepRequestStatus(vmReq, null);
                        continue;
                    }

                    if (message.Contains("another operation is pending"))
                    {
                        vmReq.StatusMessage = "Retry on 'another operation is pending' condition";
                        cdb.SetVmDepRequestStatus(vmReq, null);
                        continue;
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessMovePagefile() " + Utilities.UnwindExceptionMessages(ex);
                    Utilities.SetVmReqExceptionType(vmReq,
                        CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);

                    cdb.SetVmDepRequestStatus(vmReq, remoteErrorDescriptionList);
                }
                finally
                {
                    if (null != psRem)
                        try
                        {
                            psRem.Dispose();
                        }
                        catch (Exception)
                        {
                            //*** TODO: markwes : now what?
                        }
                }
            }

            return 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class CreateDrivesStage : Stage
    {
        public bool AddUsersToGroups { get; set; }

        public string ADDUSERTOGROUPTEMPLATE { get; set; }

        public int DefaultRebootDwellTimeMinutes { get; set; }

        public Func<VmDeploymentRequest, PowershellLib.Remoting> GetPowershellConnection { get; set; }

        public bool MovePagefile { get; set; }

        public bool ProvisionDisks { get; set; }

        protected virtual Constants.StatusEnum NextStatusCode
        {
            get { return Constants.StatusEnum.StartingSequences; }
        }

        protected virtual string NextStatusMessage
        {
            get { return "Starting final stage sequences"; }
        }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var HaltSequence = false;
            var remoteErrorDescriptionList = new List<string>();
            PowershellLib.Remoting psRem = null;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.CreatingDrives.ToString(), true);
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
                    if (null == vmReq)
                    {
                        continue;
                    }

                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.CreatingDrives.ToString());

                    //*** Experiment with dwell time ***
                    if (null != vmReq.CurrentStateStartTime)
                        if (DefaultRebootDwellTimeMinutes > DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                            continue;

                    //*** Set status to 'PostProcessing' ***
                    vmReq.StatusCode = Constants.StatusEnum.CreatingDrives.ToString();
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Creating Drives";
                    cdb.SetVmDepRequestStatus(vmReq, null);

                    //*** Contact remote Powershell ***

                    try
                    {
                        psRem = GetPowershellConnection(vmReq);
                    }
                    catch (Exception)
                    {
                        //*** TODO: Mark West : Try this for now
                        continue;

                        /*if( ex.Message.Contains("Not Found") )
                        {
                            VmReq.CurrentStateStartTime = DateTime.UtcNow;
                            VmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                            VmReq.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);
                            CDB.SetVmDepRequestStatus(VmReq, null);
                            continue;
                        }*/
                    }

                    if (null == psRem)
                    {
                        //*** It can take time to spin up WinRM, if we can't connect then break out and try again next cycle
                        continue;
                    }

                    var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
                    List<string> commandList = null;

                    //*** Move temp drive to specified drive ***
                    PowershellLib.RemotingResult RR = null;
                    if (MovePagefile && (null != vmc.PageFileConfig))
                    {
                        //*** Relabel drive 'D' to drive 'X' ***

                        if (null == vmc.PageFileConfig.DiskName)
                            vmc.PageFileConfig.DiskName = "X";
                        if (0 == vmc.PageFileConfig.DiskName.Length)
                            vmc.PageFileConfig.DiskName = "X";

                        commandList = new List<string>(2)
                        {
                            @"$drive = Get-WmiObject -Class win32_volume -Filter ""Label = 'Temporary Storage'""",
                            "Set-WmiInstance -input $drive -Arguments @{DriveLetter=\"" +
                                vmc.PageFileConfig.DiskName + ":\";}"
                        };

                        RR = psRem.Execute(null, commandList);

                        if (RR.HasErrors)
                        {
                            remoteErrorDescriptionList.AddRange(RR.ErrorDecsriptionList.Select(
                                ed => string.Format("Moving 'Temporary Storage' drive to {0}: - {1}",
                                    vmc.PageFileConfig.DiskName, ed)));

                            //*** Try again ***

                            RR = psRem.Execute(null, commandList);

                            if (RR.HasErrors)
                                remoteErrorDescriptionList.AddRange(RR.ErrorDecsriptionList.Select(
                                    ed => string.Format("Moving 'Temporary Storage' drive to {0}: - {1}",
                                        vmc.PageFileConfig.DiskName, ed)));
                        }

                        //*** Relable DVD drive 'E' to drive 'Z' ***

                        commandList = new List<string>(2)
                        {
                            @"$drive = Get-WmiObject -Class win32_volume -Filter ""DriveType = '5'""",
                            "Set-WmiInstance -input $drive -Arguments @{DriveLetter=\"" + "Z" + ":\";}"
                        };

                        RR = psRem.Execute(null, commandList);

                        //*** Move pagefile to 'X' ***

                        commandList = new List<string>(8)
                        {
                            @"$ComputerSystem = Get-WmiObject -Class Win32_ComputerSystem -EnableAllPrivileges",
                            @"$ComputerSystem.AutomaticManagedPagefile = $false",
                            @"$ComputerSystem.Put()",
                            @"Start-Sleep -s 2",
                            @"$CurrentPageFile = Get-WmiObject -Class Win32_PageFileSetting",
                            @"$CurrentPageFile.Delete()",
                            @"Start-Sleep -s 2",
                            "Set-WmiInstance -Class Win32_PageFileSetting -Arguments @{Name=\"" +
                            vmc.PageFileConfig.DiskName +
                            ":\\pagefile.sys\"; InitialSize = 0; MaximumSize = 0}"
                        };

                        RR = psRem.Execute(null, commandList);

                        if (RR.HasErrors)
                            remoteErrorDescriptionList.AddRange(RR.ErrorDecsriptionList.Select(
                                ed => string.Format("Moving pagefile to {0}: - {1}",
                                    vmc.PageFileConfig.DiskName, ed)));
                    }

                    //*** Provision disks ***

                    if (ProvisionDisks && (null != vmc.DiskSpecList && 0 < vmc.DiskSpecList.Count))
                    {
                        //*** Initialize Disks ***

                        var diskIndex = 2;

                        commandList = new List<string>(2)
                            {
                                "NEW-ITEM -Path 'diskpartscript.txt' -ItemType file -force | OUT-NULL",
                                "ADD-CONTENT -Path 'diskpartscript.txt' -Value 'san policy=OnlineAll'",
                                "diskpart /s 'diskpartscript.txt'"
                            };

                        var rr = psRem.Execute(null, commandList);

                        if (rr.HasErrors)
                        {
                            remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(
                                ed => "Problem setting diskpart san policy=OnlineAll : " + ed));
                        }

                        foreach (var ds in vmc.DiskSpecList)
                        {
                            try
                            {
                                if (!ds.IsOS)
                                {
                                    commandList = new List<string>(5)
                                    {
                                        "NEW-ITEM -Path 'diskpartscript.txt' -ItemType file -force | OUT-NULL",
                                        "ADD-CONTENT -Path 'diskpartscript.txt' -Value 'select disk " + diskIndex++ + "'",
                                        "ADD-CONTENT -Path 'diskpartscript.txt' -Value 'attribute disk clear readonly'",
                                        "ADD-CONTENT -Path 'diskpartscript.txt' -Value 'online disk'"
                                    };

                                    if (vmReq.RequestType.Equals(CmpInterfaceModel.Constants.RequestTypeEnum.NewVM.ToString(),
                                    StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        commandList.Add("ADD-CONTENT -Path 'diskpartscript.txt' -Value 'CONVERT GPT'");
                                        commandList.Add("ADD-CONTENT -Path 'diskpartscript.txt' -Value 'CREATE PARTITION PRIMARY'");

                                        if (ds.BlockSizeK == 0)
                                            commandList.Add("ADD-CONTENT -Path 'diskpartscript.txt' -Value 'FORMAT FS=ntfs QUICK'");
                                        else
                                            commandList.Add("ADD-CONTENT -Path 'diskpartscript.txt' -Value 'FORMAT FS=ntfs unit=" + ds.BlockSizeK + "K QUICK'");

                                        commandList.Add("ADD-CONTENT -Path 'diskpartscript.txt' -Value 'ASSIGN LETTER= " + ds.DriveLetter + "'");
                                    }

                                    commandList.Add("diskpart /s 'diskpartscript.txt'");

                                    rr = psRem.Execute(null, commandList);

                                    if (rr.HasErrors)
                                    {
                                        remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(
                                            ed => "Problem setting diskpart online disk : " + ed));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                remoteErrorDescriptionList.Add("Exception while assigning disk(s) : " +
                                                               Utilities.UnwindExceptionMessages(ex));
                            }
                        }
                    }

                    //*** Add entities to local groups ***

                    if (AddUsersToGroups && (null != vmc.UserSpecList && 0 < vmc.UserSpecList.Count))
                    {
                        //*** Add entities to group(s) ***
                        commandList = new List<string>(1);

                        foreach (var us in vmc.UserSpecList)
                        {
                            if (null == us)
                                remoteErrorDescriptionList.Add("Null UserSpec");
                            else if (null == us.GroupToJoinName)
                                remoteErrorDescriptionList.Add("Null GroupToJoin name in UserSpec");
                            else if (null == us.DomainName)
                                remoteErrorDescriptionList.Add("Null DomainName in UserSpec");
                            else if (null == us.EntityName)
                                remoteErrorDescriptionList.Add("Null EntityName in UserSpec");
                            else if (0 == us.GroupToJoinName.Length & 0 == us.DomainName.Length &
                                     0 == us.EntityName.Length)
                                continue;
                            else
                            {
                                commandList.Add(string.Format(ADDUSERTOGROUPTEMPLATE,
                                    us.GroupToJoinName, us.DomainName, us.EntityName));
                                RR = psRem.Execute(null, commandList);

                                if (RR.HasErrors)
                                    foreach (var ED in RR.ErrorDecsriptionList)
                                        if (!(ED.Contains("already a member")))
                                            remoteErrorDescriptionList.Add(string.Format(
                                                "Adding entity '{0}/{1}' to '{2}' group : {3}",
                                                us.DomainName, us.EntityName, us.GroupToJoinName, ED));
                            }
                        }
                    }

                    //*** Remove temporary local admin user profile ***

                    //*** TODO : Fix this. Got the following error:
                    /*
                    Exception in Remoting.Execute() : At line:1 char:51
                    + ([ADSI]"WinNT://$env:computername").delete("user",U2b7384407)
                    +                                                   ~
                    Missing expression after ','.

                    At line:1 char:51
                    + ([ADSI]"WinNT://$env:computername").delete("user",U2b7384407)
                    +                                                   ~~~~~~~~~~
                    Unexpected token 'U2b7384407' in expression or statement.

                    At line:1 char:61
                    + ([ADSI]"WinNT://$env:computername").delete("user",U2b7384407)
                    +                                                             ~
                    Unexpected token ')' in expression or statement.
                    */

                    /*string tempAccountName = "";

                    try
                    {
                        tempAccountName = Utilities.GetXmlInnerText(vmReq.Config, "AdminUsername");

                        if (null != tempAccountName)
                        {
                            commandList = new List<string>(1);
                            //CommandList.Add(@"([ADSI]""WinNT://$env:computername"").delete(""user"",TheTempUserName)");
                            commandList.Add(string.Format(_DELETELOCALUSERTEMPLATE, tempAccountName));
                            RR = psRem.Execute(null, commandList);

                            if (RR.HasErrors)
                                foreach (string ED in RR.ErrorDecsriptionList)
                                    remoteErrorDescriptionList.Add(string.Format("Removing temp local admin '{0}' : {1}",
                                        tempAccountName, ED));
                        }
                    }
                    catch (Exception ex)
                    {
                        remoteErrorDescriptionList.Add(string.Format("Removing temp local admin '{0}' : {1}",
                            tempAccountName, Utilities.UnwindExceptionMessages(ex)));
                    }*/

                    if (!HaltSequence)
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.ExceptionMessage = "";
                        vmReq.StatusMessage = NextStatusMessage;
                        vmReq.StatusCode = NextStatusCode.ToString();
                        cdb.SetVmDepRequestStatus(vmReq, remoteErrorDescriptionList);
                    }
                }
                catch (Exception ex)
                {

                    if (null == vmReq)
                    {
                        LogThis(ex, EventLogEntryType.Error, "Exception in ProcessPostProcess()", 10, 10);
                    }
                    else
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);
                        Utilities.SetVmReqExceptionType(vmReq,
                            CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                        cdb.SetVmDepRequestStatus(vmReq, remoteErrorDescriptionList);
                    }
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
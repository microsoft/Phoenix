using System;
using System.Collections.Generic;
using System.Threading;

namespace CmpServiceLib
{
    class PostProvDisk : IDisposable
    {
        const string DISK_INITALL   = "Get-Disk | Where-Object PartitionStyle –Eq \"RAW\" | Initialize-Disk -confirm:$false";
        const string DISK_PARTITION = "New-Partition -DiskNumber {0} -UseMaximumSize -AssignDriveLetter";
        const string DISK_FORMAT    = "Format-Volume -FileSystem NTFS -DriveLetter {0} -confirm:$false";
        const int DwellTime = 20000;

        string ImpersonateName = null;
        string ImpersonateDomain = null;
        string ImpersonatePassword = null;

        string TargetURL = null;
        string VmProcessorDirectory = null;

        PowershellLib.Remoting _PsRem = null;

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="targetUrl"></param>
        /// <param name="impersonateName"></param>
        /// <param name="impersonateDomain"></param>
        /// <param name="impersonatePassword"></param>
        ///  
        //*********************************************************************

        public PostProvDisk(string targetUrl, string impersonateName, 
            string impersonateDomain, string impersonatePassword)
        {
            TargetURL = targetUrl;
            ImpersonateName = impersonateName; 
            ImpersonateDomain = impersonateDomain;
            ImpersonatePassword = impersonatePassword;

            _PsRem = new PowershellLib.Remoting(TargetURL,
                ImpersonateDomain + "\\" + ImpersonateName, ImpersonatePassword);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public PowershellLib.RemotingResult ExecuteCommands( List<string> commandList )
        {
            try
            {
                var psResult = _PsRem.Execute( VmProcessorDirectory, commandList );
                return psResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in PostProvDisk.Execute() : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public PowershellLib.RemotingResult ProvisionDisk(string lun, string driveLetter)
        {
            try
            {
                var commandList = new List<string>(1) 
                    {string.Format(DISK_PARTITION, lun)};
                var psResult = ExecuteCommands(commandList);

                if (psResult.HasErrors)
                    return psResult;

                Thread.Sleep(DwellTime);

                commandList = new List<string>(1) 
                    {string.Format(DISK_FORMAT, driveLetter)};
                psResult = ExecuteCommands(commandList);

                return psResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in PostProvDisk.ProvisionDisk() : " 
                    + Utilities.UnwindExceptionMessages(ex));
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

        public PowershellLib.RemotingResult Initialize()
        {
            try
            {
                var commandList = new List<string>(1);
                commandList.Add(DISK_INITALL);
                var psResult = ExecuteCommands(commandList);

                return psResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in PostProvDisk.Initialize() : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        {
            if (null != _PsRem)
            {
                _PsRem.Dispose();
                _PsRem = null;
            }
        }
    }
}

/*
--- Clear Disk ---
Get-Disk 2 | Clear-Disk -RemoveData

--- Build Disk ---
Get-Disk | Where-Object PartitionStyle –Eq "RAW" | Initialize-Disk -confirm:$false
New-Partition -DiskNumber 2 -UseMaximumSize -AssignDriveLetter
Format-Volume -FileSystem NTFS -DriveLetter F -confirm:$false
**/ 

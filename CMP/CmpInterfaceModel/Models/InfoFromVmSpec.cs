using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    public class InfoFromVmSpec
    {
        /// <summary> </summary>
        public string BiosAssetTag { set; get; }
        /// <summary> </summary>
        public string VmAddress { set; get; }
        /// <summary> </summary>
        public string ComputerName { get; set; }
        /// <summary> </summary>
        public string OperatingSystemVersion { get; set; }
        /// <summary> </summary>
        public int NumberOfCores { get; set; }
        /// <summary> </summary>
        public float InstalledMemoryGb { get; set; }
        /// <summary> </summary>
        public int DriveCount { get; set; }
        /// <summary> </summary>
        public int MaxDriveSizeGB { get; set; }
        /// <summary> </summary>
        public int TotalDriveSpaceGB { get; set; }

        public string DriveLetters { get; set; }        
        
        public List<Disk> GetDiskInfo()
        {
            if (this.DriveLetters == null) { return null; }
            
            //if the DriveLetters string contains delimiters; split into the array
            if (this.DriveLetters.Contains(",") == true)
            {
                var Disks = new List<Disk>();

                foreach (var d in (this.DriveLetters.Split(',')))
                {
                    if (d.Contains(";") == true)
                    {
                        var dSplit = d.Split(';');
                        var dr = new Disk();
                        dr.DriveLetter = dSplit[0].ToString().ToUpper();
                        dr.UsedSpaceGB = int.Parse(dSplit[1].ToString());
                        dr.TotalSpaceGB = int.Parse(dSplit[2].ToString());
                        dr.FreeSpaceGB = dr.TotalSpaceGB - dr.UsedSpaceGB;
                        Disks.Add(dr);
                    }
                }

                return Disks;
            }
            else { return null; }            
        }

        /// <summary> </summary>
        public string ComputerModel { get; set; }
        /// <summary> </summary>
        public string MachineDomain { get; set; }
        /// <summary> </summary>
        public string ClusteringStatus { get; set; }
        /// <summary> </summary>
        public int NetworkAdapterCount { get; set; }
        /// <summary> </summary>
        public int StaticIpAddressCount { get; set; }
        /// <summary> </summary>
        public string DataCenter { get; set; }
        public string LocalAdmins { get; set; }
        public bool? IsWindowsActivated { get; set; }   
    }

    public class PostInfoFromVmSpec
    {
        /// <summary> </summary>
        public string BiosAssetTag { set; get; }
        /// <summary> </summary>
        public string VmAddress { set; get; }
        public string DriveLetters { get; set; }
        public List<Disk> GetDiskInfo()
        {
            if (this.DriveLetters == null) { return null; }

            //if the DriveLetters string contains delimiters; split into the array
            if (this.DriveLetters.Contains(",") == true)
            {
                var Disks = new List<Disk>();

                foreach (var d in (this.DriveLetters.Split(',')))
                {
                    if (d.Contains(";") == true)
                    {
                        var dSplit = d.Split(';');
                        var dr = new Disk();
                        dr.DriveLetter = dSplit[0].ToString().ToUpper();
                        dr.UsedSpaceGB = int.Parse(dSplit[1].ToString());
                        dr.TotalSpaceGB = int.Parse(dSplit[2].ToString());
                        dr.FreeSpaceGB = dr.TotalSpaceGB - dr.UsedSpaceGB;
                        Disks.Add(dr);
                    }
                }

                return Disks;
            }
            else { return null; }
        }
        /// <summary> </summary>
        public string MachineDomain { get; set; }
        /// <summary> </summary>
        public string LocalAdmins { get; set; }
        public bool? IsWindowsActivated { get; set; }
        public string PageFiles { get; set; }
        public string TempFileDrive { get; set; }
        public bool? IsAzureVmAgentInstalled { get; set; }
        public string MachineOU { get; set; }
    }

    public class Disk
    {
        public string DriveLetter { get; set; }
        public int TotalSpaceGB { get; set; }
        public int UsedSpaceGB { get; set; }
        public int FreeSpaceGB { get; set; }
    }
}

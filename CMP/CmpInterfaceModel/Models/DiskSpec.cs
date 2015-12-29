namespace CmpInterfaceModel.Models
{
    public class DiskSpec
    {
        /// <summary>LUN</summary>
        public string Lun { set; get; }
        /// <summary>Drive Letter</summary>
        public string DriveLetter { set; get; }
        /// <summary>Logical Disk Size In GB</summary>
        public string LogicalDiskSizeInGB { set; get; }
        /// <summary>Config</summary>
        public string Config { set; get; }
        /// <summary>TagData</summary>
        public string TagData { set; get; }
        /// <summary>Source Vhd File (if migrated)</summary>
        public string SourceVhdFile { set; get; }
        /// <summary>MediaLink (if migrated)</summary>
        public string MediaLink { set; get; }
        /// <summary>Is this the VM OS disk?</summary>
        public bool IsOS { set; get; }
        public int BlockSizeK { set; get; }
    }
}


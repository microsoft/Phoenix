namespace AzureAdminClientLib
{
    public class RoleSize
    {
        private readonly string _name;
        private readonly int _coreCount;
        private readonly int _diskCount;
        private readonly double _ramMb;
        private readonly int _diskSizeRoleOs;
        private readonly double _diskSizeRoleApps;
        private readonly int _diskSizeVmOs;
        private readonly int _diskSizeVmTemp;
        private readonly bool _canBeService;
        private readonly bool _canBeVm;

        /// <summary> </summary>
        public string Name { get { return _name; } }
        /// <summary> </summary>
        public int CoreCount { get { return _coreCount; } }
        /// <summary> </summary>
        public int DiskCount { get { return _diskCount; } }
        /// <summary> </summary>
        public double RamMB { get { return _ramMb; } }
        /// <summary> </summary>
        public int DiskSizeRoleOS { get { return _diskSizeRoleOs; } }
        /// <summary> </summary>
        public double DiskSizeRoleApps { get { return _diskSizeRoleApps; } }
        /// <summary> </summary>
        public int DiskSizeVmOs { get { return _diskSizeVmOs; } }
        /// <summary> </summary>
        public int DiskSizeVmTemp { get { return _diskSizeVmTemp; } }
        /// <summary> </summary>
        public bool CanBeService { get { return _canBeService; } }
        /// <summary> </summary>
        public bool CanBeVm { get { return _canBeVm; } }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="coreCount"></param>
        /// <param name="diskCount"></param>
        /// <param name="ramMB"></param>
        /// <param name="diskSizeRoleOs"></param>
        /// <param name="diskSizeRoleApps"></param>
        /// <param name="diskSizeVmOs"></param>
        /// <param name="diskSizeVmTemp"></param>
        /// <param name="canBeService"></param>
        /// <param name="canBeVm"></param>
        /// 
        //*********************************************************************

        public RoleSize( string name, int coreCount, int diskCount, double ramMB, 
            int diskSizeRoleOs, double diskSizeRoleApps, int diskSizeVmOs, int diskSizeVmTemp,
            bool canBeService, bool canBeVm)
        {
            _name = name;
            _coreCount = coreCount;
            _diskCount = diskCount;
            _ramMb = ramMB;
            _diskSizeRoleOs = diskSizeRoleOs;
            _diskSizeRoleApps = diskSizeRoleApps;
            _diskSizeVmOs = diskSizeVmOs;
            _diskSizeVmTemp = diskSizeVmTemp;
            _canBeService = canBeService;
            _canBeVm = canBeVm;
        }
    }
}

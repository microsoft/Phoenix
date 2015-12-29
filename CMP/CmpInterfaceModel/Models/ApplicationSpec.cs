using System;
using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    public class SqlSpec
    {
        public Nullable<bool> InstallSql { get; set; }
        public Nullable<bool> InstallAnalysisServices { get; set; }
        public Nullable<bool> InstallReplicationServices { get; set; }
        public Nullable<bool> InstallIntegrationServicesallSql { get; set; }
        public string SqlInstancneName { get; set; }
        public string Collation { get; set; }
        public string Version { get; set; }
        public List<string> AdminGroupList { get; set; }
        public string AnalysisServicesMode { get; set; }
    }

    public class IisSpec
    {
        public Nullable<bool> InstallIis { get; set; }
        public string RoleServices { get; set; }
    }

    public class ApplicationSpec
    {
        public SqlSpec SqlConfig { get; set; }
        public IisSpec IisConfig { get; set; }
    }
}

using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    public class OpSpec
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Opcode { get; set; }
        public int TargetId { get; set; }
        public string TargetName { get; set; }
        public string TargetType { get; set; }
        public string sData { get; set; }
        public int iData { get; set; }
        public string Config { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Vmsize { get; set; }
        public List<CmpCommon.VhdInfo> Disks { get; set; }
        public string Requestor { get; set; }
    }
}

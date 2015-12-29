using System;

namespace CmpAzureServiceWebRole.Models
{
    public partial class CmpServiceUserAccount
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public string Domain { get; set; }
        public string AssociatedCorpAccountName { get; set; }
        public string StatusCode { get; set; }
        public string AccountTypeCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

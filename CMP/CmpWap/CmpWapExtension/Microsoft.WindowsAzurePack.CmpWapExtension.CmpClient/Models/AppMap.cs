using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AppMap
    {
        public int Id { get; set; }
        public Nullable<int> AppIdCode { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagId { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

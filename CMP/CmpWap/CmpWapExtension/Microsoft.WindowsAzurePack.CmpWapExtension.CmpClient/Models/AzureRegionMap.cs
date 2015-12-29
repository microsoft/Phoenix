using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
   public partial class AzureRegionMap
    {
        public int Id { get; set; }
        public string AzureReqionName { get; set; }
        public string WapRegionName { get; set; }
        public string OsImageContainer { get; set; }
        public string Config { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

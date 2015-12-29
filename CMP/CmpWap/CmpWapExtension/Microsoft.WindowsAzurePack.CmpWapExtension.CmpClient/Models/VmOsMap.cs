using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class VmOsMap
    {
        public int VMOsId { get; set; }

        public string Name { get; set; }

       
        public string Description { get; set; }

     
        public string AzureImageName { get; set; }

        public bool IsActive { get; set; }
    }
}

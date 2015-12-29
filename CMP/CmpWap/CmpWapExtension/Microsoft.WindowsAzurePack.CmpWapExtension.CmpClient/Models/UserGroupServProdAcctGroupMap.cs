using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class UserGroupServProdAcctGroupMap
    {
        public int Id { get; set; }
        public string UserGroupName { get; set; }
        public string ServProvAcctGroupName { get; set; }
        public string Config { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
    }
}

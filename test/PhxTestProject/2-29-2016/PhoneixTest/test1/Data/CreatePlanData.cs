using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Test.Data
{
    public class CreatePlanData
    {
        public string planName { get; set; }

        public string groupName { get; set; }

        public string region { get; set; }

        // step 2
        public string serverName { get; set; }

        public string azureOsVersion { get; set; }

        public string userName { get; set; }

        public string localAdminPassword { get; set; }

        // step 3
        public bool needAdditionalDrives { get; set; }

    }
}

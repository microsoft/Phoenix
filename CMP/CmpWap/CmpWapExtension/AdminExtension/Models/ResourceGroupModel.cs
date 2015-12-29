using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    class ResourceGroupModel
    {
        public ResourceGroupModel()
        {
            
        }

        public ResourceGroupModel(ResourceGroup rGroup)
        {
            this.ResourceProviderAcctGroupId = rGroup.ResourceProviderAcctGroupId;
            this.Name = rGroup.Name;
            this.DomainId = rGroup.DomainId;
            this.EnvironmentTypeId = rGroup.EnvironmentTypeId;
            this.NetworkNICId = rGroup.NetworkNICId;
        }

        public ResourceGroup ToApiObject()
        {
            return new ResourceGroup()
            {
                ResourceProviderAcctGroupId = this.ResourceProviderAcctGroupId,
                Name = this.Name,
                DomainId = this.DomainId,
                NetworkNICId = this.NetworkNICId,
                EnvironmentTypeId = this.EnvironmentTypeId                
            };
        }

        public int ResourceProviderAcctGroupId { get; set; }
        public string Name { get; set; }
        public int DomainId { get; set; }

        public int NetworkNICId { get; set; }

        public int EnvironmentTypeId { get; set; }
    }
}

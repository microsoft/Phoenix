using System;
using System.Collections.Generic;

namespace AzureAdminClientLib
{
    public class AzureVmOsArmData
    {
        public string Publisher { get; set; }
        public string Offer { get; set; }
        public string SKU { get; set; }

        public class AzureVmOsArmDataComparer : IEqualityComparer<AzureVmOsArmData>
        {
            public bool Equals(AzureVmOsArmData x, AzureVmOsArmData y)
            {
                //Check whether the compared objects reference the same data. 
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null. 
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.Publisher == y.Publisher
                    && x.Offer == y.Offer
                    && x.SKU == y.SKU;
            }

            public int GetHashCode(AzureVmOsArmData data) {
                //Check whether the object is null 
                if (Object.ReferenceEquals(data, null)) return 0;

                //Get hash code for each field if it is not null. 
                int hashPub = data.Publisher == null ? 0 : data.Publisher.GetHashCode();
                int hashOffer = data.Offer == null ? 0 : data.Offer.GetHashCode();
                int hashSKU = data.SKU == null ? 0 : data.SKU.GetHashCode();

                return hashPub ^ hashOffer ^ hashSKU;
            }
        }
    }
}



using System;
using System.Linq;

//using Microsoft.Data.OData.Query.SemanticAst;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public class ECSPortalDB
    {
        public Models.ECS.Subscription FetchSubscription(Guid subscriptionId)
        {
            try
            {
                using (var db = new Models.ECS.ECSPortalDBContext())
                {
                    var subList = from s in db.Subscriptions
                        where s.WAPSubscriptionID == subscriptionId
                        select s;

                    return !subList.Any() ? null : subList.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}

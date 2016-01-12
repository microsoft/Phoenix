using System;
using System.Collections.Generic;
using System.Linq;
using CmpInterfaceModel;

namespace AzureAdminClientLib
{
    public class SubscriptionOps
    {
        const string URLTEMPLATE_FETCHREGIONRESOURCEUSAGE_ARM =
            "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/usages?api-version={2}";

        private readonly IConnection _Connection = null;

        public class ResourceUsageStruct
        {
            public string Unit;
            public int Value;
            public int Limit;
            public string Name;
            public string LocalName;
            public string Region;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public SubscriptionOps(IConnection connection)
        {
            _Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public List<ResourceUsageStruct> FetchResourceUsage()
        {
            var aro = new AzureRegionOps(_Connection);
            var regionlist = aro.GetAzureLocationsList();
            var usageList = new List<ResourceUsageStruct>();

            foreach (var region in regionlist)
                usageList.AddRange(FetchResourceUsage(region.Name));

            return usageList;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<ResourceUsageStruct> FetchResourceUsage(string region)
        {
            try
            {
                var url = string.Format(URLTEMPLATE_FETCHREGIONRESOURCEUSAGE_ARM,
                    _Connection.SubcriptionID, "westus", "2015-06-15");

                var hi = new HttpInterface(_Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (resp.HadError)
                    throw new Exception("Error in Azure response : " + resp.Body );

                var ul = Utilities.FetchJsonValue(resp.Body, "value") as Newtonsoft.Json.Linq.JArray;
                var usageList = new List<ResourceUsageStruct>();

                if (null != ul)
                    foreach (var usage in ul)
                    {
                        usageList.Add(new ResourceUsageStruct()
                        {
                            Region = region,
                            Unit  = Utilities.FetchJsonValue(usage.ToString(),"unit") as string, 
                            Value = Convert.ToInt32(Utilities.FetchJsonValue(usage.ToString(),"currentValue")), 
                            Limit = Convert.ToInt32(Utilities.FetchJsonValue(usage.ToString(),"limit")), 
                            Name = Utilities.FetchJsonValue(usage.ToString(),new[] {"name", "value"}) as string, 
                            LocalName = Utilities.FetchJsonValue(usage.ToString(),new[] {"name", "localizedValue"}) as string
                        });
                    }

                return usageList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SubscriptionOps.FetchResourceUsage() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="metric"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ResourceUsageStruct FetchResourceUsage(string region, string metric)
        {
            var ruList = FetchResourceUsage(region);

            return ruList.FirstOrDefault(ru => ru.Name.Equals(metric));
        }
    }
}


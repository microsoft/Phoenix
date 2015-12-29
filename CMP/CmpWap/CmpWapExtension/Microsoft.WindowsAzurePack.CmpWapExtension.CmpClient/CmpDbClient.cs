using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    //*********************************************************************
    ///
    /// <summary>
    /// Reads and writes CMP VM deployment requests to the CMP database
    /// </summary>
    /// 
    //*********************************************************************

    class CmpDbClient
    {
        string _CmpDbConnectionString = null;

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Initializes a CmpDbClient connection with a CMP database connection string 
        ///  </summary>
        ///  <param name="cmpDbConnectionString">Connection string for the CMP database</param>
        ///  
        //*********************************************************************

        public CmpDbClient(string cmpDbConnectionString)
        {
            if(null == cmpDbConnectionString)
                _CmpDbConnectionString = CmpCommon.Utilities.GetConnectionString("CMPContext");
            else
                _CmpDbConnectionString = cmpDbConnectionString;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets a list of CMP VM deployment requests
        /// </summary>
        /// <returns>A list of CMP VM deployment requests</returns>
        /// 
        //*********************************************************************

        public List<CmpServiceLib.Models.VmDeploymentRequest> FetchVmDepRequests()
        {
            var cdb = new CmpServiceLib.CmpDb(_CmpDbConnectionString);
            return cdb.FetchVmDepRequests("", true);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets a single CMP VM deployment request with the specified 
        /// Deployment Request ID
        /// </summary>
        /// <param name="deploymentRequestId">The Deployment Request ID for the
        /// CMP VM deployment request to get</param>
        /// <returns>CMP VM deployment request with the specified 
        /// Deployment Request ID</returns>
        /// 
        //*********************************************************************
        public CmpServiceLib.Models.VmDeploymentRequest FetchVmDepRequest(int deploymentRequestId)
        {
            var cdb = new CmpServiceLib.CmpDb(_CmpDbConnectionString);
            return cdb.FetchVmDepRequest(deploymentRequestId);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Updates a CMP VM deployment request in the CMP database
        ///  </summary>
        ///  <param name="vmRequest">The CMP VM deployment request to update
        /// </param>
        /// <param name="warningList">A list of any warnings associated with 
        /// the CMP VM deployment request</param>
        ///  
        //*********************************************************************

        public void UpdateVmDepRequest(CmpServiceLib.Models.VmDeploymentRequest vmRequest, List<string> warningList)
        {
            var cdb = new CmpServiceLib.CmpDb(_CmpDbConnectionString);
            cdb.SetVmDepRequestStatus(vmRequest, warningList);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Creates a new CMP VM deployment request in the CMP databse
        ///  </summary>
        /// <param name="vmDepRequest">The CMP VM deployment request to add to
        /// the CMP database</param>
        /// <returns>The CMP VM deployment request to added to the CMP database
        /// </returns>
        ///  
        //*********************************************************************

        public CmpServiceLib.Models.VmDeploymentRequest InsertVmDepRequest(CmpServiceLib.Models.VmDeploymentRequest vmDepRequest)
        {
            var cdb = new CmpServiceLib.CmpDb(_CmpDbConnectionString);
            return cdb.InsertVmDepRequest(vmDepRequest);
        }
    }
}

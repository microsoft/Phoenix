using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using CmpServiceLib;
using AzureAdminClientLib;

namespace CMPWapFuncTest
{
    [TestClass]
    public class UnitTest1
    {
        private static string _cmpDbConnectionString;
        private static EventLog _eventLog;

        [TestMethod]
        public void DeleteVm()
        {
            _eventLog = new EventLog();
            _cmpDbConnectionString = GetCmpContextConnectionStringFromConfig();
            CmpService service = new CmpService(_eventLog,_cmpDbConnectionString,"");
            service.VmStop(25);


        }

        [TestMethod]
        public void ResizeVm()
        {
            _eventLog = new EventLog();
            _cmpDbConnectionString = GetCmpContextConnectionStringFromConfig();
            CmpService service = new CmpService(_eventLog, _cmpDbConnectionString, "");
            service.VmResize(20,"D12");
           
           // VmOps ops = new VmOps();



        }

        private string GetCmpContextConnectionStringFromConfig()
        {
            try
            {
                var xk = new KryptoLib.X509Krypto(null);
                return (xk.GetKTextConnectionString("CMPContext", "CMPContextPassword"));
            }
            catch (Exception ex)
            {
                 return null;
            }
        }

        [TestMethod]
        public void FetchAzureRegionsListFromAzure()
        {
            IEnumerable<AzureLocationArmData> result = null;
            Exception error = null;
            try
            {
                AzureRefreshService ars = new AzureRefreshService(null, null);
                //result = ars.FetchAzureRegionsList();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.IsNotNull(result);
            Assert.IsNull(error);
        }
    }
}

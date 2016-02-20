using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AzureAdminClientLib;
using CmpInterfaceModel.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using CmpServiceLib;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Data.Entity;
using System.Diagnostics;
using Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Controllers;
using Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient;
using Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Controllers;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.UnitTestUi
{
    public partial class Form1 : Form
    {
        EventLog eLog = new EventLog();
        public Form1()
        {
            InitializeComponent();
        }

        private void SubmitTest()
        {
            var subId = "";

            var paramList = new List<ScriptJobPoshParamSpec>
            {
                new ScriptJobPoshParamSpec() {Name = "scriptblock", Value = "write-output get-process"},
                new ScriptJobPoshParamSpec() {Name = "Computername", Value = "localhost"}
            };

            var sj = new ScriptJob
            {
                BreakOn = CmpInterfaceModel.Models.SequenceSpec.BreakOnEnum.Exception.ToString(),
                Config = null,
                Engine = CmpInterfaceModel.Models.SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                ExecuteInState = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                Waitmode = CmpInterfaceModel.Models.SequenceSpec.WaitmodeEnum.Synchronous.ToString(),
                ID = 1,
                Locale = CmpInterfaceModel.Models.SequenceSpec.SequenceLocaleEnum.Remote.ToString(),
                Name = "The Sequence Name",
                TagData = "<Tag Data>",
                ScriptList = null,
                SmaConfig = new ScriptJobSmaConfigSpec()
                {
                    SmaServerUrl = "",
                    RunbookId = "",
                    RunbookName = "execute-scriptblock",
                    ParamList = paramList
                }
            };

            var sjc = new ScriptJobsController();
            var sjOut = sjc.SubmitScriptJob(subId, sj);
        }

        private void FetchTest()
        {
            string subId = "";
            string jobId = "";

            var sjc = new ScriptJobsController();
            var sjOut = sjc.GetScriptJob(subId, jobId);
        }

        private void OpsQueueTest()
        {
            var vmOp = new VmOp()
            {
                Config = "",
                Id = 0,
                Name = null,
                Opcode = CmpInterfaceModel.Constants.VmOpcodeEnum.STOP.ToString(),
                StatusCode = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                StatusMessage = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                VmId = 3,
                Vmsize = "",
                disks = null,
                iData = 0,
                sData = null
            };

            var opSpec = VmOpsController.Translate(vmOp);
            opSpec.Requestor = "";
            //opSpec = new CmpInterface(null).SubmitOpToQueue(opSpec);


            var cmp = new CmpServiceLib.CmpService(eLog, ConfigurationManager.ConnectionStrings["CMPContext"].ConnectionString, null);
            opSpec = cmp.SubmitOpToQueue(opSpec);

        }

        private void GroupFetchTest()
        {
            var rgc = new ResourceGroupController();
            var rgList = rgc.ListResourceGroups("");
        }

        private void AzureCall_FetchRegionsTest()
        {
            IEnumerable<AzureLocationArmData> locationResult = null;
            IEnumerable<AzureVmSizeArmData> sizeResult = null;
            IEnumerable<AzureVmOsArmData> dataResult = null;
            AzureRefreshService ars = new AzureRefreshService(null, ConfigurationManager.ConnectionStrings["CMPContext"].ConnectionString);
            //AzureRefreshService ars = new AzureRefreshService(null, "Data Source=thezephyr;Initial Catalog=CMP;Persist Security Info=True;User ID=sa;Password=123Mainau!;MultipleActiveResultSets=True");
            ars.FetchAzureInformationWithArm(out locationResult, out sizeResult, out dataResult);
            //UpdateAzureRegions(locationResult.ToList());
            //UpdateVmSizes(sizeResult.ToList());
            //osResult = ars.FetchAzureOsList();
            //UpdateVmOs(osResult);
            //UpdateVmOsArm(dataResult);
        }

        private void TestSyncWorkerWithAzure()
        {
            /* You'll have to make the method static to run this unit test*/
            //SyncWorker.SyncWithAzure();
        }


        private Connection GetTestConnection()
        {
            var subId = "";
            var tenantId = "";
            var clientId = "";
            var clientKey = "";

            return new Connection(subId, null, tenantId, clientId, clientKey);
        }

        private void FetchArmResourceGroupList()
        {
            var hso = new HostedServiceOps(GetTestConnection());
            var res = hso.GetResourceGroupList();
        }

        private void FetchArmResourceGroup()
        {
            var hso = new HostedServiceOps(GetTestConnection());
            var res = hso.GetResourceGroup("");
        }

        private void CreateArmResourceGroup()
        {
            var hso = new HostedServiceOps(GetTestConnection());
            var res = hso.CreateResourceGroup("", "westus", false);
        }

        private void CheckResourceGroupAvailability()
        {
            string agName;
            var hso = new HostedServiceOps(GetTestConnection());
            var res = hso.CheckResourceGroupAvailability("", 1, out agName);
        }

        private void FetchVnetInfo()
        {
            string agName;
            var hso = new VnetOps(GetTestConnection());
            var res = hso.FetchVnetInfoArm("", "");
        }

        private void StopVm()
        {
            var vmo = new VmOpsController();
            var soRet = vmo.SubmitOp("",
                new VmOp()
                {
                    Opcode = CmpInterfaceModel.Constants.VmOpcodeEnum.STOP.ToString(),
                    VmId = 1029
                });
        }

        private void FetchRegionsTest()
        {
            var arc = new RegionsController();
            var regionlist = arc.ListRegions("");
        }

        private void FetchResourceGroupNameTest()
        {
            var controller = new ResourceGroupController();
            var resourceGroupName = controller.ListResourceGroups("");
            var resourceGroupNameAll = controller.ListResourceGroups();
        }

        private void FetchVmSizesFromAzureForCmp()
        {
            var psai = new ProcessorSyncAzureInfo(new EventLog(), "");
            psai.SyncWithAzure();
        }

        private void FetchRoleSizes()
        {
            var cmpdb = new CmpDb("");
            var role1 = cmpdb.FetchAzureRoleSizeList();
            var role2 = cmpdb.GetAzureRoleSizeFromName("Standard_A5");
        }

        private void FetchEnvironmentTypesTest()
        {
            var ctr = new EnvironmentTypesController();
            var result = ctr.ListEnvironmentTypes("");
        }

        private void TestSyncWorker()
        {
            SyncWorker.StartAsync(null);
        }

        private void FetchTenantInfo()
        {
            var ctr = new OSsController();
            var ctr2 = new VmSizesController();
            //var result = ctr.ListOSs("");
            var result = ctr.ListOSs("");
            var resultAll = ctr.ListOSs();
            var result3 = ctr2.ListVmSizes("");
        }

        private void FetchPlanConfigInfo()
        {
            var ctr = new PlansController();
            var result = ctr.GetPlanConfiguration("Novihmnodnl");
        }

        private void SetVmOsByBatch()
        {
            var ctr = new CmpWapDb();
            var list = new List<VmOs>
            {
                new VmOs()
                {
                    VmOsId = 7,
                    IsActive = true,
                },
                new VmOs()
                {
                    VmOsId = 420,
                    IsActive = false,
                }
            };
            ctr.SetVmOsByBatch(list, "Novihmnodnl");
        }

        private void FetchOsInfoTest()
        {
            var ctr = new CmpWapDb();
            var result = ctr.FetchOsInfoList("Novihmnodnl");
        }

        private void FetchTenantVmSizes()
        {
            ICmpWapDbTenantRepository ctr = new CmpWapDb();
            var result = ctr.FetchVmSizeInfoList("");
        }

        private void FetchDefaultResourceGroupTest()
        {
            var ctr = new CmpWapDb();
            var result = ctr.FetchDefaultResourceProviderGroupName("");
        }

        private void ServiceProviderFetchTest()
        {
            var sp = new ServiceProviderAccountsController();
            string subID = "";
            var result = sp.ListServiceProviderAccounts();

        }

        private void ServiceProviderInsertTest()
        {
            var sp = new ServiceProviderAccountsController();
            ServiceProviderAccount spA = new ServiceProviderAccount();
            //spA.ID = 345;
            spA.Name = "Test";
            spA.ClientKey = "123abc!!!";
            string subID = "";
            var result = sp.UpdateServiceProviderAccount(spA);

        }

        private async void GetVmTest()
        {
            var subID = "c438fe96-7cc3-43a1-ac87-c2795a6eea79";
            var vmID = 3;
            //var subID = "93198a73-7ca2-4218-b694-02b43aeb4b25";
            //var vmID = 3;

            //var tc = new CmpWapExtensionTenantController();
            //var resp = await tc.GetVm(subID, vmID);

            var vmc = new VmsController();
            var resp = vmc.GetVm(subID, vmID);
        }

        private void PostOpsProcessingTest()
        {
            var p = new ProcessorOps(null, ConfigurationManager.ConnectionStrings["CMPContext"].ConnectionString);
            p.ProcessOpsProcessing();
            //p.ProcessOpsSubmissions();
        }

        private void VmOpsTest()
        {
            var ctr = new VmOpsController();
            ctr.GetVmOpsQueueStatus("", 10);
        }

        private void VmOpsStopTest()
        {
            var vmops = new VmOps(GetTestConnection());
            vmops.Stop("raldabadec10", "RobertoTest-01");
        }

        private void VmOpsResizeTest()
        {
            var vmops = new VmOps(GetTestConnection());

            try
            {
                vmops.Resize("aaa", "bbb", "bad");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            //vmops.Resize("aaa", "bbb", "Standard_A6");
            vmops.Resize("aaa", "bbb", "Standard_A7");
        }

        private async void TenantGetSubMappingsTest()
        {
            string [] subId = {"c438fe96-7cc3-43a1-ac87-c2795a6eea79"};
            var tc = new SubscriptionsController();
            var resp = tc.GetSubscriptionList(subId);
        }

        private void CmpSyncTest()
        {
            var sw = new SyncWorker();
            sw.SynchWithCmp();
        }

        private void SpaAadValidationTest()
        {
            var tenantId = "";
            var clientId = "";
            var clientKey = "";
            bool result = Microsoft.WindowsAzurePack.CmpWapExtension.Api.Utilities.ValidateAadCredentials(clientId, tenantId, clientKey);
        }

        private async void button_Test_Click(object sender, EventArgs e)
        {
            try
            {
                //StopVm();
                //FetchTest();
                //SubmitTest();
                //OpsQueueTest();
                //GroupFetchTest();
                //FetchArmResourceGroupList();
                //FetchArmResourceGroup();
                //CreateArmResourceGroup();
                //CheckResourceGroupAvailability();
                //ApiTest_VmSizes();
                //ApiTest_VmOs();
                //AzureCall_FetchRegionsTest();
                //vt = new VmTests();
                //vt.BuildVerificationTest_01();
                //FetchVnetInfo();


                //FetchVmSizesFromAzureForCmp();
                //AzureCall_FetchRegionsTest();
                //FetchRegionsTest();
                //FetchResourceGroupNameTest();
                //FetchRoleSizes();
                //TestSyncWorker();
                //FetchTenantInfo();
                //SetVmOsByBatch();
                //FetchPlanConfigInfo();
                //AzureCall_FetchRegionsTest();
                //TestSyncWorkerWithAzure();
                //FetchOsInfoTest();
                //FetchTenantVmSizes();
                //FetchDefaultResourceGroupTest();

                GetVmTest();
                //PostOpsProcessingTest();
                //VmOpsTest();
                //VmOpsStopTest();
                //VmOpsResizeTest();
                //TenantGetSubMappingsTest();

                //FetchEnvironmentTypesTest();
                //SpaAadValidationTest();

                //CmpSyncTest();
                //ServiceProviderInsertTest();
                //ServiceProviderFetchTest();

                /*
                vt = new VmTests();
                vt.BuildVerificationTest_01();*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //private async void button_Test_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var tenantId = "";
        //        var clientId = "";
        //        var clientKey = "";

        //        var ret = await AzureActiveDir.GetAdUserToken(tenantId, clientId, clientKey);

        //        //var ret = AzureActiveDir.GetAdUserToken(tenantId, clientId, clientKey);
        //        //ret.Wait();
        //        //var AdToken = ret.Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        //throw;
        //    }
        //}
    }
}

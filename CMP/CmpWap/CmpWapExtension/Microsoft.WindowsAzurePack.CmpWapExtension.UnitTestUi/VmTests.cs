using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.UnitTestUi
{
    class VmTests
    {
        private EventLog _eventLog;
        private IVMServiceRepository _vmRepo;

        public VmTests()
        {
            _eventLog = new EventLog("Application");
        }

        private List<CreateVm> VmDeploymentSetup()
        {
            var vmDepList = new List<CreateVm>
            {
                new CreateVm
                {
                    //Changeable fields
                    SubscriptionId = "", // put your wap subscription here
                    EnvResourceGroupName = "DEFAULT", // Change this to resource group pointing to your Azure Sub
                    VmRegion = "West US", // Change this to the target region

                    // Constant fields
                    VmAppName = "BeanBVT01",
                    VmAppId = "ICTO-BeanBVT01",
                    Name = "TSTBNBVT" + new Random().Next(10000),
                    VmDomain = null,
                    VmAdminName = "bvtdev",
                    VmAdminPassword = "",
                    VmSourceImage = "Windows Server Essentials Experience on Windows Server 2012 R2",
                    VmSize = "Standard_D3",
                    VmRole = "3",
                    VmDiskSpec ="<Drives><Drive><Letter>C</Letter><Role>Data</Role><SizeInGB>100</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive><Drive><Letter>D</Letter><Role>Data</Role><SizeInGB>21</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive></Drives>",
                    VmConfig = "",
                    VmTagData = "",
                    ServiceCategory = "Basic",
                    Nic1 = "West Europe",
                    Msitmonitored = "false",
                    sqlconfig = null,
                    IIsconfig = null,
                    EnvironmentClass = "Dev",
                    AccountAdminLiveEmailId = "abcd@contoso.com", 
                    OsCode = "Windows", 
                    AzureApiName = "ARM", 
                    CmpRequestId = 0, 
                    CreatedBy = "markw", 
                    AddressFromVm = null, 
                    Id = 0, 
                    ExceptionMessage = null, 
                    StatusCode = null, 
                    StatusMessage = null, 
                    Type = null
                }
            };
            return vmDepList;
        }

        public void BuildVerificationTest_01()
        {
            //Initialize values
            var vmList = VmDeploymentSetup();

            // starting CMP service
            //var cmpService = StartCmpServiceTest();

            int depId = CreateVmTest(_eventLog, vmList[0]);

            // Create VM as Async Tasks
            /*Task[] taskArray = new Task[vmList.Count];
            for (int i = 0; i< vmList.Count; i++)
            {
                // Start VM Provisioning
                int depId = CreateVmTest(_eventLog, vmList[i]);

                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    VmDeploymentRequest vmd = obj as VmDeploymentRequest;
                    // Check VM Provisioning Status
                    var cmpi = new VMServiceRepository(_eventLog);
                    var vmDepReq = cmpi.FetchCmpRequest(depId);

                    string statusCode = null;
                    while (!vmDepReq.Status.Equals("Complete") && !vmDepReq.Status.Equals("Exception"))
                    {
                        vmDepReq = cmpi.FetchCmpRequest(depId);
                        statusCode = vmDepReq.Status;
                    }
                    vmd.Status = statusCode;
                    vmd.StatusMessage = vmDepReq.StatusMessage;
                }, new VmDeploymentRequest(){});
            }
            Task.WaitAll(taskArray);
            
            //Assert

            foreach (var task in taskArray)
            {
                var data = task.AsyncState as VmDeploymentRequest;
                //Assert.AreNotEqual(data.Status, null);
                //Assert.AreNotEqual(data.Status, "Exception", data.StatusMessage);                
            }

            DeleteVmTest(_eventLog ,vmList );*/

            // Stop CMP Service
            //StopCmpServiceTest(cmpService);
        }

        private int CreateVmTest(EventLog _eventLog, CreateVm vM)
        {
            var cmpi = new VMServiceRepository(_eventLog);
            //Insert app data to the DB 


            var nvm = new CreateVm
            {
                VmAppName = vM.VmAppName,
                VmAppId = vM.VmAppId,
                SubscriptionId = vM.SubscriptionId,
                AccountAdminLiveEmailId = vM.AccountAdminLiveEmailId
            };

            //cmpi.PerformAppDataOps(nvm);
            //Submit VM information to the WAP DB
            vM = cmpi.SubmitVmRequest(vM);

            return vM.CmpRequestId;
        }

        private void DeleteVmTest(EventLog _eventLog, List<CreateVm> vmList)
        {

            VmOp vmOp = new VmOp()
            {
                Opcode = CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEFROMSTORAGE.ToString(),
                iData = 0,
                sData = null
            };

            Task[] taskArray = new Task[vmList.Count];
            var cmpi = new VMServiceRepository(_eventLog);
            for (int i = 0; i < vmList.Count; i++)
            {
                var opSpec = VmOpsController.Translate(new VmOp { Opcode = vmOp.Opcode, VmId = vmList[i].Id, sData = vmOp.sData, iData = vmOp.iData });

                taskArray[i] = Task.Factory.StartNew(() =>
                {
                    opSpec = cmpi.SubmitOperation(opSpec);

                    while (!opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString())
                            && !opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                    {
                        opSpec = cmpi.GetVmOpsRequestSpec(vmList[i].Name);
                    }

                });
            }
            Task.WaitAll(taskArray);

            for (int i = 0; i < vmList.Count; i++)
            {
                var opSpec = cmpi.GetVmOpsRequestSpec(vmList[i].Name);
                //Assert.AreNotEqual(opSpec, null);
                //Assert.AreEqual(opSpec.StatusCode, CmpInterfaceModel.Constants.StatusEnum.Complete);
            }

        }


        private CmpServiceLib.CmpService StartCmpServiceTest()
        {
            _eventLog.Source = CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName;
            _eventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
            CmpServiceLib.CmpService _CS = new CmpServiceLib.CmpService(_eventLog, null, null);
            //if service has not started it will throw an exception and the test will fail
            _CS.AsynchStart();

            return _CS;
        }

        private void StopCmpServiceTest(CmpServiceLib.CmpService cmpService)
        {
            cmpService.AsynchStop();
        }
    }
}

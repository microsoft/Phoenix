using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CmpInterfaceModel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.CmpService;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers;
using VmDeploymentRequest = CmpInterfaceModel.Models.VmDeploymentRequest;

namespace CmpBuildVerificationTests
{
    [TestClass]
    public class CmpBuildVerificationTests
    {
        private EventLog _eventLog;
        private IVMServiceRepository _vmRepo;

        public CmpBuildVerificationTests()
        {
            _eventLog = new EventLog("Application");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<CreateVm> VmDeploymentSetup()
        {
            var vmDepList = new List<CreateVm>
            {
                new CreateVm
                {
                    //Changeable fields
                    SubscriptionId = "xxx", // put your wap subscription here
                    EnvResourceGroupName = "DEFAULT", // Change this to resource group pointing to your Azure Sub
                    VmRegion = "East US", // Change this to the target region

                    // Constant fields
                    VmAppName = "App1",
                    VmAppId = "App1",
                    Name = "App1Server" + new Random().Next(10000),
                    VmDomain = null,
                    VmAdminName = "myadminname",
                    VmAdminPassword = "mypassword",
                    VmSourceImage = "Windows Server Essentials Experience on Windows Server 2012 R2",
                    VmSize = "A5",
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
                    AzureApiName = "ARM"
                },
                new CreateVm
                {
                    //Changeable fields
                    SubscriptionId = "xxx", // put your wap subscription here
                    EnvResourceGroupName = "DEFAULT", // Change this to resource group pointing to your Azure Sub
                    VmRegion = "East US", // Change this to the target region

                    // Constant fields
                    VmAppName = "App2",
                    VmAppId = "App2",
                    Name = "App2Server" + new Random().Next(10000),
                   //Name = "TSTBNBVTWE925",
                    VmDomain = null,
                    VmAdminName = "myadminname",
                    VmAdminPassword = "mypassword",
                    VmSourceImage = "Windows Server 2012 R2 Datacenter, May 2015",
                    VmSize = "Standard_D12",
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
                    AzureApiName = "ARM"
                },
                new CreateVm
                {
                    //Changeable fields
                    SubscriptionId = "xxx", // put your wap subscription here
                    EnvResourceGroupName = "DEFAULT", // Change this to resource group pointing to your Azure Sub
                    VmRegion = "East US", // Change this to the target region

                    // Constant fields
                    VmAppName = "App3",
                    VmAppId = "App3",
                    Name = "App3Server" + new Random().Next(10000),
                   //Name = "TSTBNBVTWE925",
                    VmDomain = null,
                    VmAdminName = "myadminname",
                    VmAdminPassword = "mypassword",
                    VmSourceImage = "Windows Server 2012 R2 Datacenter, May 2015",
                    VmSize = "Standard_D12",
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
                    AzureApiName = "ARM"                }
            };
            return vmDepList;
        }

        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<CreateVm> Setup2()
        {
            return new List<CreateVm>
            {
                new CreateVm
                {
                    //Changeable fields
                    SubscriptionId = "xxx", // put your wap subscription here
                    EnvResourceGroupName = "DEFAULT", // Change this to resource group pointing to your Azure Sub
                    VmRegion = "East US", // Change this to the target region

                    // Constant fields
                    VmAppName = "App4",
                    VmAppId = "App4",
                    Name = "App4Server" + new Random().Next(10000),
                   //Name = "TSTBNBVTWE925",
                    VmDomain = null,
                    VmAdminName = "myadminname",
                    VmAdminPassword = "mypassword",
                    VmSourceImage = "Windows Server 2012 R2 Datacenter, May 2015",
                    VmSize = "Standard_D12",
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
                    AzureApiName = "ARM"                }
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        [TestMethod]
        public void BuildVerificationTest_01()
        {
            //Initialize values
            var vmList = VmDeploymentSetup();
            // starting CMP service
            var cmpService = StartCmpServiceTest();
            
            // Create VM as Async Tasks
            Task[] taskArray = new Task[vmList.Count];
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
                    if (vmd != null)
                    {
                        vmd.Status = statusCode;
                        vmd.StatusMessage = vmDepReq.StatusMessage;
                    }
                }, new VmDeploymentRequest(){});
            }
            Task.WaitAll(taskArray);
            
            //Assert

            foreach (var task in taskArray)
            {
                var data = task.AsyncState as VmDeploymentRequest;
                Assert.AreNotEqual(data.Status, null);
                Assert.AreNotEqual(data.Status, "Exception", data.StatusMessage);                
                Assert.AreEqual(data.Status, "Complete", data.StatusMessage);
            }


            DeleteVmTest(_eventLog ,vmList );

            // Stop CMP Service
            StopCmpServiceTest(cmpService);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_eventLog"></param>
        /// <param name="vM"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int CreateVmTest(EventLog _eventLog, CreateVm vM)
        {
            var cmpi = new VMServiceRepository(_eventLog);
            //Insert app data to the DB 
            cmpi.PerformAppDataOps(new CreateVm
            {
                VmAppName = vM.VmAppName,
                VmAppId = vM.VmAppId,
                SubscriptionId = vM.SubscriptionId,
                AccountAdminLiveEmailId = vM.AccountAdminLiveEmailId
            });
            //Submit VM information to the WAP DB
            vM = cmpi.SubmitVmRequest(vM);

            return vM.CmpRequestId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_eventLog"></param>
        /// <param name="vmList"></param>
        /// 
        //*********************************************************************

        private void DeleteVmTest(EventLog _eventLog , List<CreateVm> vmList)
        {

            VmOp vmOp = new VmOp()
            {
                Opcode = CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEFROMSTORAGE.ToString(),
                iData = 0,
                sData = null
            };

            List<OpSpec> opsList = new List<OpSpec>();
            var cmpi = new VMServiceRepository(_eventLog);

            for (int i = 0; i < vmList.Count; i++)
            {
                var opSpec = VmOpsController.Translate(new VmOp { Opcode = vmOp.Opcode, VmId = vmList[i].Id, sData = vmOp.sData, iData = vmOp.iData });

                opSpec = cmpi.SubmitOperation(opSpec);

                opSpec = cmpi.GetVmOpsRequestSpec(vmList[i].Name);
                while (!opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString())
                        && !opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                {
                    opSpec = cmpi.GetVmOpsRequestSpec(vmList[i].Name);
                }

                opsList.Add(opSpec);

                if (opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                {
                    break;
                }
            }

            foreach (var ops in opsList)
            {
                Assert.AreNotEqual(ops.StatusCode, null);
                Assert.AreNotEqual(ops.StatusCode, "Exception", ops.StatusMessage);
                Assert.AreEqual(ops.StatusCode, "Complete", ops.StatusMessage);
            }
            /*
            Task[] taskArray = new Task[vmList.Count];
            for (int i = 0; i < vmList.Count; i++)
            {
                var i1 = i;
                taskArray[i] = Task.Factory.StartNew((Object ob) =>
                {
                    OpRequest op = ob as OpRequest;
                    var opSpec = cmpi.GetVmOpsRequestSpec(vmList[i1].Name);
                    while (!opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString())
                            && !opSpec.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                    {
                        opSpec = cmpi.GetVmOpsRequestSpec(vmList[i1].Name);
                    }

                    op.StatusCode = opSpec.StatusCode;
                    op.StatusMessage = opSpec.StatusMessage;

                }, new OpRequest(){});
            }
            Task.WaitAll(taskArray);

            foreach (var task in taskArray)
            {
                var data = task.AsyncState as OpRequest;
                Assert.AreNotEqual(data.StatusCode, null);
                Assert.AreNotEqual(data.StatusCode, "Exception", data.StatusMessage);
                Assert.AreEqual(data.StatusCode, "Complete", data.StatusMessage);
            }

            for (int i = 0; i < vmList.Count; i++ )
            {
                var opSpec = cmpi.GetVmOpsRequestSpec(vmList[i].Name);
                Assert.AreNotEqual(opSpec , null);
                Assert.AreEqual(opSpec.StatusCode, CmpInterfaceModel.Constants.StatusEnum.Complete.ToString());
            }*/
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

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

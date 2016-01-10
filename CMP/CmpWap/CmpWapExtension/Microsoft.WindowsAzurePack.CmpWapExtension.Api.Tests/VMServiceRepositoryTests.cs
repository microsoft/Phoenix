using System;
using System.Collections.Generic;
using System.Linq;
using CmpCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.CmpService;
using Moq;
using ServiceProviderAccount = Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts.ServiceProviderAccount;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Tests
{
    [TestClass]
    public class VmServiceRepositoryTests
    {
        private Mock<ICmpApiClient> _cmpApiServiceMock;
        private Mock<ICmpWapDb> _cmpWapDbMock;
        private Mock<ICmpWapDbAdminRepository> _cmpWapDbAdminMock;

        [TestInitialize]
        public void Setup()
        {
            _cmpApiServiceMock = new Mock<ICmpApiClient>();
            _cmpWapDbMock = new Mock<ICmpWapDb>();

            var vmDepRequest = new List<VmDeploymentRequest>()
            {
                new VmDeploymentRequest()
                {
                    ID = 345,
                    RequestName = "CMPWAP: MockVM1",
                    RequestDescription = "Mocking VM for test",
                    ParentAppName = "A&O Stuff",
                    RequestType = "NewVM",
                    Active = true,
                    AftsID = 123,
                    ParentAppID = "ICTO-1234",
                    TargetServiceProviderType = "Azure",
                    TargetLocation = "CentralUS",
                    TargetLocationType = "Region",
                    TargetAccount = "ecd32150-1e35-4a03-af0f-163da2eff99a",
                    TargetAccountType = "AzureSubscription",
                    TargetAccountCreds = null,
                    VmSize = null,
                    TagData = "XML Mock",
                    Config = "Xml Mock",
                    TargetVmName = "MockVM1",
                    WhoRequested = "redmond\abcd",
                    WhenRequested = DateTime.Now,
                    ExceptionMessage = "exception xyz",
                    LastStatusUpdate = DateTime.Today,
                    StatusMessage = "Exception in xyz"
                },
                new VmDeploymentRequest()
                {
                    ID = 346,
                    RequestName = "CMPWAP: MockVM2",
                    RequestDescription = "Mocking VM for test",
                    ParentAppName = "A&O Stuff",
                    RequestType = "NewVM",
                    Active = true,
                    AftsID = 123,
                    ParentAppID = "ICTO-1234",
                    TargetServiceProviderType = "Azure",
                    TargetLocation = "CentralUS",
                    TargetLocationType = "Region",
                    TargetAccount = "ecd32150-1e35-4a03-af0f-163da2eff99a",
                    TargetAccountType = "AzureSubscription",
                    TargetAccountCreds = null,
                    VmSize = null,
                    TagData = "XML Mock",
                    Config = "Xml Mock",
                    TargetVmName = "MockVM2",
                    WhoRequested = "redmond\abcd",
                    WhenRequested = DateTime.Now,
                    ExceptionMessage = null,
                    LastStatusUpdate = DateTime.Today,
                    StatusMessage = "null"
                }
            };

            var vmInfo = new CmpApiClient.VmInfo()
            {
                DeploymentID = "123",
                DNSName = "mockvmdns",
                InternalIP = "1.2.3.4",
                RDPCertificateThumbprint = "adwsaeqe45354etfg5645etfdg",
                RoleName = "mockvm",
                RoleSize = "A7",
                Status = "ReadyRole",
                QueueStatus = "Complete"
            };

            var svcProviderAcc = new List<ServiceProviderAccount>()
            {
                new ServiceProviderAccount()
                {
                    ID = 123,
                    Name = "Serv Prov 1",
                    Description = "Description for Serv Prov Account",
                    OwnerNamesCSV = "Owner Names",
                    Config = "<Config></Config>",
                    TagData = "Tag Data",
                    TagID = 1,
                    ResourceGroup = "RGroup1"
                },
                new ServiceProviderAccount()
                {
                    ID = 456,
                    Name = "Serv Prov 1",
                    Description = "Description for Serv Prov Account",
                    OwnerNamesCSV = "Owner Names",
                    Config = "<Config></Config>",
                    TagData = "Tag Data",
                    TagID = 2,
                    ResourceGroup = "RGroup2"
                }
            };

            var detachedDisks = new List<VhdInfo>()
            {
                new VhdInfo()
                {
                    DiskName = "VM1_C",
                },
                new VhdInfo()
                {
                    DiskName = "VM1_D"
                },
                new VhdInfo()
                {
                    DiskName = "VM1_E"
                }
            };

            var apps = new List<Models.Application>
            {
                new Models.Application
                {
                    ApplicationId = 1,
                    IsActive = true,
                    HasService = true,
                    CIOwner = "user1@contoso.com",
                    Code = "App1",
                    CreatedBy = "user1@contoso.com",
                    LastUpdatedBy = "user1@contoso.com",
                    CreatedOn = DateTime.UtcNow,
                    LastUpdatedOn = DateTime.UtcNow,
                    Name = "App1",
                    SubscriptionId = "abcd-1234",
                    Region = "USWest"
                },
                new Models.Application
                {
                    ApplicationId = 2,
                    IsActive = true,
                    HasService = true,
                    CIOwner = "user1@contoso.com",
                    Code = "Ap2",
                    CreatedBy = "user1@contoso.com",
                    LastUpdatedBy = "user1@contoso.com",
                    CreatedOn = DateTime.UtcNow,
                    LastUpdatedOn = DateTime.UtcNow,
                    Name = "App2",
                    SubscriptionId = "abcd-1234",
                    Region = "USWest"
                }
            };

            var vmSizeInfoListToFetch = new List<Models.VmSize>
            {
                new Models.VmSize{
                    VmSizeId = 123,
                    Name = "MockVmSize 1",
                    Description = "Mock VM Size for testing purposes",
                    Cores = 1,
                    Memory = 2048,
                    MaxDataDiskCount = 2,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "user1@contoso.com",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "user1@contoso.com"
                },
                new Models.VmSize{
                    VmSizeId = 456,
                    Name = "MockVmSize 2",
                    Description = "Mock VM Size for testing purposes",
                    Cores = 1,
                    Memory = 2048,
                    MaxDataDiskCount = 2,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "user2@contoso.com",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "user2@contoso.com"
                },
                
            };

            var azureRegionListToFetch = new List<Models.AzureRegion>
            {
                new Models.AzureRegion
                {
                    AzureRegionId = 123,
                    Name = "Central US",
                    Description = "Central US",
                    OsImageContainer = "https://contoso.com",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "user2@contoso.com",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "user2@contoso.com"
                },
                new Models.AzureRegion
                {
                    AzureRegionId = 456,
                    Name = "West US",
                    Description = "West US",
                    OsImageContainer = "https://contosoAlternative.com",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "admin@contoso.com",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "admin@contoso.com"
                }
            };

            var vmOsInfoListToFetch = new List<Models.VmOs>
            {
                new Models.VmOs
                {
                    VmOsId = 1234,
                    Name = "Windows Server 2008 R2 (SP1)",
                    Description = "Windows Server 2008 R2 (SP1)",
                    AzureImageName = "SDO_2014_09_2008R2SP1_FS.vhd",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "REDMOND\ramankum",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "REDMOND\ramankum",
                    AzureImageOffer = "WindowsServer",
                    AzureImagePublisher = "MicrosoftWindowsServe",
                    AzureWindowsOSVersion = "2012-R2-Datacenter"
                },
                new Models.VmOs
                {
                    VmOsId = 5678,
                    Name = "Windows Server 2012 (SP0)",
                    Description = "Windows Server 2012 (SP0)",
                    AzureImageName = "SDO_2014_09_2012SP0_FS.vhd",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "REDMOND\ramankum",
                    LastUpdatedOn = DateTime.UtcNow,
                    LastUpdatedBy = "REDMOND\ramankum",
                    AzureImageOffer = "WindowsServer",
                    AzureImagePublisher = "MicrosoftWindowsServe",
                    AzureWindowsOSVersion = "2012-R2-Datacenter"
                }
            };
                

            //Mocks for CmpApiClient
            _cmpApiServiceMock.Setup(r => r.FetchCmpRequests())
                .Returns(vmDepRequest);

            _cmpApiServiceMock.Setup(r =>r.FetchCmpRequest(It.IsAny<int>()))
                .Returns((int n) => vmDepRequest.FirstOrDefault(req => req.ID == n));

            _cmpApiServiceMock.Setup(r => r.FetchServProvAcctList(It.IsAny<string>()))
                .Returns((string n) => Translate(svcProviderAcc.Where(svc => svc.ResourceGroup == n)));

            _cmpApiServiceMock.Setup(r => r.GetDetachedDisks(It.IsAny<int>()))
                .Returns((int n) => n == 123 ? detachedDisks : null);

            _cmpApiServiceMock.Setup(r => r.GetVm(It.IsAny<int>()))
                .Returns((int n) => n == 123 ? vmInfo : null);

            // Mocks for WAP DB
            _cmpWapDbMock.Setup(r => r.CheckAppDataRecord(It.IsAny<CreateVm>()))
                .Returns((CreateVm vm) => apps.Any(a => a.Name == vm.VmAppName || a.Code == vm.VmAppId));

            _cmpWapDbMock.Setup(r => r.InsertAppDataRecord(It.IsAny<CreateVm>()));

            _cmpWapDbMock.Setup(r => r.FetchVmSizeInfoList(true))
                .Returns(vmSizeInfoListToFetch);

            _cmpWapDbMock.Setup(r => r.DeleteVmSize(It.IsAny<int>()));

            //_cmpWapDbMock.Setup(r => r.FetchAzureRegionList())
            //    .Returns(azureRegionListToFetch);

            _cmpWapDbMock.Setup(r => r.FetchOsInfoList(true))
                .Returns(vmOsInfoListToFetch);

        }

        private static List<CmpApiClient.ServiceProviderAccount> Translate(IEnumerable<ServiceProviderAccount> enumerable)
        {

            return enumerable.Select(spa => new CmpApiClient.ServiceProviderAccount()
            {
                ID = spa.ID,
                AccountID = spa.AccountID,
                AccountPassword = spa.AccountPassword,
                AccountType = spa.AccountType,
                Active = spa.Active,
                AzAffinityGroup = spa.AzAffinityGroup,
                AzRegion = spa.AzRegion,
                AzStorageContainerUrl = spa.AzStorageContainerUrl,
                AzSubnet = spa.AzSubnet,
                AzVNet = spa.AzVNet,
                CertificateBlob = spa.CertificateBlob,
                CertificateThumbprint = spa.CertificateThumbprint,
                Config = spa.Config,
                CoreCountCurrent = spa.CoreCountCurrent,
                CoreCountMax = spa.CoreCountMax,
                Description = spa.Description,
                ExpirationDate = spa.ExpirationDate,
                Name = spa.Name,
                OwnerNamesCSV = spa.OwnerNamesCSV,
                ResourceGroup = spa.ResourceGroup,
                TagData = spa.TagData
            }).ToList();
        }

        [TestMethod]
        public void FetchCmpRequest_OnExecuteWithValidValue_ReturnsDepRequests()
        {
            //Arrange
            var repo = new VMServiceRepository() {CmpSvProxy = _cmpApiServiceMock.Object};

            //Act
            var output = repo.FetchCmpRequest(345);

            //Assert
            Assert.IsNotNull(output);
            Assert.AreEqual("MockVM1", output.TargetVmName);
        }

        [TestMethod]
        public void FetchCmpRequest_OnExecuteWithInvalidValue_ReturnsNull()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.FetchCmpRequest(34565);

            //Assert
            Assert.IsNull(output);
        }

        [TestMethod]
        public void FetchServiceProviderAccountList_OnExecuteWithValidValue_ServiceProviderAccountList()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.FetchServiceProviderAccountList("RGroup1");

            //Assert
            Assert.AreNotEqual(output.Count, 0);
        }

        [TestMethod]
        public void GetDetachedDisks_OnExecuteWithValidValue_VhdInfoList()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.GetDetachedDisks(123);

            //Assert
            Assert.AreNotEqual(output.ToList().Count, 0);
        }

        [TestMethod]
        public void GetDetachedDisks_OnExecuteWithInValidValue_ReturnsNull()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.GetDetachedDisks(124);

            //Assert
            Assert.IsNull(output);
        }

        [TestMethod]
        public void GetVm_OnExecuteWithValidValue_VmInfo()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.GetVm(123);

            //Assert
            Assert.IsNotNull(output);
        }

        [TestMethod]
        public void GetVm_OnExecuteWithInValidValue_ReturnsNull()
        {
            //Arrange
            var repo = new VMServiceRepository() { CmpSvProxy = _cmpApiServiceMock.Object };

            //Act
            var output = repo.GetDetachedDisks(124);

            //Assert
            Assert.IsNull(output);
        }

        [TestMethod]
        public void PerformAppDataOps_OnExecutionWithPresentValue_ReturnsVoid()
        {
            //Arrange
            var repo = new VMServiceRepository { WapDbContract = _cmpWapDbMock.Object };
            Exception exObject = null;

            //Act
            try
            {
                repo.PerformAppDataOps(new CreateVm
                {
                    VmAppName = "App1",
                    VmAppId = "App1"
                });
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            Assert.IsNull(exObject);
        }

        [TestMethod]
        public void PerformAppDataOps_OnExecutionWithAbsentValue_ReturnsVoid()
        {
            //Arrange
            var repo = new VMServiceRepository { WapDbContract = _cmpWapDbMock.Object };
            Exception exObject = null;

            //Act
            try
            {
                repo.PerformAppDataOps(new CreateVm
                {
                    VmAppName = "App231",
                    VmAppId = "App231"
                });
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            Assert.IsNull(exObject);
        }

        #region VmSizeInfo CRUD tests

        [TestMethod]
        public void FetchVmSizeInfoList_OnExecute_ReturnsVmSizeList()
        {
            //Arrange
            var repo = _cmpWapDbMock.Object;
            IEnumerable<Models.VmSize> result = null;

            //Act
            try
            {
                result = repo.FetchVmSizeInfoList(onlyActiveOnes: true);
            }
            catch (Exception ex)
            {
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InsertVmSizeInfo_OnExecute_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var vmsThatWereInserted = new List<Models.VmSize>();
            _cmpWapDbMock.Setup(r => r.InsertVmSizeByBatch(It.IsAny<List<Models.VmSize>>())).Callback((Models.VmSize vms) => vmsThatWereInserted.Add(vms));

            var repo = _cmpWapDbMock.Object;
            var vmSizeInfoToInsert = new List<Models.VmSize>();
            
            vmSizeInfoToInsert.Add(new Models.VmSize
            {
                VmSizeId = 0,
                Name = "MockVmSize Insertion",
                Description = "Mock VM Size for testing purposes",
                Cores = 1,
                Memory = 2048,
                MaxDataDiskCount = 2,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user1@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user1@contoso.com"
            });

            //Act
            try
            {
                repo.InsertVmSizeByBatch(vmSizeInfoToInsert);
                repo.FetchVmSizeInfo("MockVmSize Insertion");
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, vmsThatWereInserted.Count());
            Assert.AreEqual(vmSizeInfoToInsert.First().Name, vmsThatWereInserted.First().Name);
        }

        [TestMethod]
        public void UpdateVmSizeInfo_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var vmsThatWereUpdated = new List<Models.VmSize>();
            _cmpWapDbMock.Setup(r => r.UpdateVmSizeInfo(It.IsAny<Models.VmSize>())).Callback((Models.VmSize vms) => vmsThatWereUpdated.Add(vms));

            var repo = _cmpWapDbMock.Object;
            var vmSizeInfoToUpdate = new Models.VmSize
            {
                VmSizeId = 123,
                Name = "Updated: Mock VM Size",
                Description = "Updated: Mock VM Size for testing purposes",
                Cores = 2,
                Memory = 4096,
                MaxDataDiskCount = 4,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user2@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user2@contoso.com"
            };

            //Act
            try
            {
                repo.UpdateVmSizeInfo(vmSizeInfoToUpdate);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, vmsThatWereUpdated.Count());
            Assert.AreEqual(vmSizeInfoToUpdate.Name, vmsThatWereUpdated.First().Name);
        }

        [TestMethod]
        public void DeleteVmSizeInfo_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            var repo = _cmpWapDbMock.Object;
            Exception exObject = null;
            var vmSizeId = 123;

            //Act
            try
            {
                repo.DeleteVmSize(vmSizeId);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
        }

        #endregion

        #region VM OS Info CRUD Tests

        [TestMethod]
        public void FetchVmOsInfoList_OnExecute_ReturnsVmOsInfoList()
        {
            //Arrange
            var repo = _cmpWapDbMock.Object;
            IEnumerable<Models.VmOs> result = null;

            //Act
            try
            {
                result = repo.FetchOsInfoList(onlyActiveOnes: true);
            }
            catch (Exception ex)
            {
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InsertOsInfo_OnExecute_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var vmOsInfosThatWereInserted = new List<Models.VmOs>();
            _cmpWapDbMock.Setup(r => r.InsertOsInfoByBatch(It.IsAny<List<Models.VmOs>>())).Callback((Models.VmOs vmo) => vmOsInfosThatWereInserted.Add(vmo));

            var repo = _cmpWapDbMock.Object;
            var vmOsInfoToInsert = new List<Models.VmOs>();
            vmOsInfoToInsert.Add(new Models.VmOs
            {
                VmOsId = 987,
                Name = "Windows Server Test (SP0)",
                Description = "Test information",
                AzureImageName = "SDO_2015_08_05_TEST_P0_FS.vhd",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user1@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user1@contoso.com",
                AzureImageOffer = "WindowsServer",
                AzureImagePublisher = "MicrosoftWindowsServe",
                AzureWindowsOSVersion = "2012-R2-Datacenter"
            });

            //Act
            try
            {
                repo.InsertOsInfoByBatch(vmOsInfoToInsert);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, vmOsInfosThatWereInserted.Count());
            Assert.AreEqual(vmOsInfoToInsert.First().Name, vmOsInfosThatWereInserted.First().Name);
        }

        [TestMethod]
        public void UpdateOsInfo_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var vmOsThatWereUpdated = new List<Models.VmOs>();
            _cmpWapDbMock.Setup(r => r.UpdateOsInfo(It.IsAny<Models.VmOs>())).Callback((Models.VmOs vmo) => vmOsThatWereUpdated.Add(vmo));

            var repo = _cmpWapDbMock.Object;
            var vmOsInfoToUpdate = new Models.VmOs
            {
                Name = "Updated Windows Server test",
                Description = "New updated description",
                AzureImageName = "newTestImage.vhd",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user2@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user2@contoso.com",
                AzureImageOffer = "WindowsServer",
                AzureImagePublisher = "MicrosoftWindowsServe",
                AzureWindowsOSVersion = "2012-R2-Datacenter"
            };

            //Act
            try
            {
                repo.UpdateOsInfo(vmOsInfoToUpdate);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, vmOsThatWereUpdated.Count());
            Assert.AreEqual(vmOsInfoToUpdate.Name, vmOsThatWereUpdated.First().Name);
        }

        [TestMethod]
        public void DeleteOsInfo_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            var repo = _cmpWapDbMock.Object;
            Exception exObject = null;
            var vmOsInfoId = 1234;

            //Act
            try
            {
                repo.DeleteOsInfo(vmOsInfoId);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
        }

        #endregion

        #region Azure Subscription CRUD tests

        //[TestMethod]
        //public void FetchAzureRegionList_OnExecute_ReturnsAzureRegionList()
        //{
        //    //Arrange
        //    var repo = _cmpWapDbMock.Object;
        //    Exception exObject = null;
        //    List<Models.AzureRegion> result = null;

        //    //Act
        //    try
        //    {
        //        result = repo.FetchAzureRegionList();
        //    }
        //    catch (Exception ex)
        //    {
        //        exObject = ex;
        //    }

        //    Assert.IsNotNull(result);
        //}

        [TestMethod]
        public void InsertAzureRegion_OnExecute_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var regionsThatWereInserted = new List<Models.AzureRegion>();
            _cmpWapDbMock.Setup(r => r.InsertAzureRegionByBatch(It.IsAny<List<Models.AzureRegion>>())).Callback((Models.AzureRegion ar) => regionsThatWereInserted.Add(ar));
            
            var repo = _cmpWapDbMock.Object;
            var azureRegionToInsert = new List<Models.AzureRegion>(); 
            azureRegionToInsert.Add(new Models.AzureRegion
            {
                AzureRegionId = 0,
                Name = "Mock Azure Region",
                Description = "Mock Azure Region for testing purposes",
                OsImageContainer = "https://contoso.com",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user1@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user1@contoso.com"
            });

            //Act
            try
            {
                repo.InsertAzureRegionByBatch(azureRegionToInsert);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, regionsThatWereInserted.Count());
            Assert.AreEqual(azureRegionToInsert.First().Name, regionsThatWereInserted.First().Name);
        }

        [TestMethod]
        public void UpdateAzureRegion_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            Exception exObject = null;
            var regionsThatWereUpdated = new List<Models.AzureRegion>();
            _cmpWapDbMock.Setup(r => r.UpdateAzureRegion(It.IsAny<Models.AzureRegion>())).Callback((Models.AzureRegion ar) => regionsThatWereUpdated.Add(ar));

            var repo = _cmpWapDbMock.Object;
            var azureRegionToUpdate = new Models.AzureRegion
            {
                AzureRegionId = 123,
                Name = "Updated: Mock Azure Region",
                Description = "Updated: Mock Azure Region for testing purposes",
                OsImageContainer = "https://contosoUpdated.com",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "user2@contoso.com",
                LastUpdatedOn = DateTime.UtcNow,
                LastUpdatedBy = "user2@contoso.com"
            };

            //Act
            try
            {
                repo.UpdateAzureRegion(azureRegionToUpdate);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
            Assert.AreEqual(1, regionsThatWereUpdated.Count());
            Assert.AreEqual(azureRegionToUpdate.Name, regionsThatWereUpdated.First().Name);
        }

        //[TestMethod]
        //public void UpdateAzureRegion_OnExecuteWithInvalidValue_ReturnsVoid()
        //{
        //    //Arrange
        //    var repo = _cmpWapDbMock.Object;
        //    Exception exObject = null;
        //    var azureRegionToUpdate = new Models.AzureRegion
        //    {
        //        AzureRegionId = 0,
        //        Name = "Updated: Mock Azure Region",
        //        Description = "Updated: Mock Azure Region for testing purposes",
        //        OsImageContainer = "https://contosoUpdated.com",
        //        IsActive = true,
        //        CreatedOn = DateTime.UtcNow,
        //        CreatedBy = "user2@contoso.com",
        //        LastUpdatedOn = DateTime.UtcNow,
        //        LastUpdatedBy = "user2@contoso.com"
        //    };

        //    //Act
        //    try
        //    {
        //        repo.UpdateAzureRegion(azureRegionToUpdate);
        //    }
        //    catch (Exception ex)
        //    {
        //        exObject = ex;
        //    }

        //    //Assert
        //    Assert.IsNotNull(exObject);
        //}

        [TestMethod]
        public void DeleteAzureRegion_OnExecuteWithValidValue_ReturnsVoid()
        {
            //Arrange
            var repo = _cmpWapDbMock.Object;
            Exception exObject = null;
            var azureRegionId = 123;

            //Act
            try
            {
                repo.DeleteAzureRegion(azureRegionId);
            }
            catch (Exception ex)
            {
                exObject = ex;
            }

            //Assert
            Assert.IsNull(exObject);
        }

        #endregion
    }
}

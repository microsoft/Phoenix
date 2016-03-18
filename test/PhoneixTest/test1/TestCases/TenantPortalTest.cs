
namespace Phoenix.Test.UI.TestCases
{
    using System;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.UI;
    
    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework;
    using Phoenix.Test.UI.Framework.Logging;
    //using Phoenix.Test.UI.Framework.Authentication;
    //using Phoenix.Test.UI.Framework.Configuration;
    using Phoenix.Test.UI.Framework.Controls;
    //using Phoenix.Test.UI.Framework.Shared;
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.Common;
    using Microsoft.VisualStudio.TestTools;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data;
    using System.IO;
    using System.Xml;

    [TestClass]
    //[SignInAs(EUserTypes.Regular)]
    public class TenantPortalTest : WAPTestBase
    {
        private LoginPage loginPage;


        private string userName;
        private string password;
        private string serverName;
        private string url;


        public TenantPortalTest(string userAccount, string password, string serverName)
        {
            this.userName = userAccount;
            this.password = password;
            this.serverName = serverName;
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ReadConfigTestUser(config);

            Log.Information("Start tenant portal test init...");
            driver.Url = "https://" + serverName + ":30081";
            
            this.loginPage = new LoginPage(driver);
            System.Threading.Thread.Sleep(15000);
            driver.Manage().Window.Maximize();
            System.Threading.Thread.Sleep(15000);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            System.Threading.Thread.Sleep(5000);
            base.TestCleanup();
        }

        [TestMethod(),
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify tenant user can create a VM from new button.")]
        public void TenantCreateVmFromNewButtonTest()
        {
            var smpPage = LoginTenantProtal(this.userName, this.password);
            var createVmData = GetCreateVmData();

            smpPage.CreateVmFromNewButton(createVmData);

            smpPage.VerifyVmCreated(createVmData);

            //Assert.IsTrue(smpPage.VerifyVmCreated(createVmData), "Failed to create VM from new button.");
        }

        public void TenantCreateVmFromMainMenuTest()
        {
            var smpPage = LoginTenantProtal(this.userName, this.password);
            var createVmData = GetCreateVmData();

            smpPage.CreateVmFromNewButton(createVmData);

            smpPage.VerifyVmCreated(createVmData);

            //Assert.IsTrue(smpPage.VerifyVmCreated(createVmData), "Failed to create VM from new button.");
        }

        public void TenantAddCoAdminFromNewButtonTest()
        {

        }




        public SmpPage LoginTenantProtal(string user, string psw)
        {
            return loginPage.Login(user, psw);
        }

        public CreateVmData GetCreateVmData()
        {
            string anyVmName = GetRandomVmName();
            return new CreateVmData() { groupName = "Group01", serverName = anyVmName, userName = "test01@microsoft.com", localAdminPassword = "Password0)" };
            //Do I need to replace this line using the non-hardcoded parameters that this class is getting from the application form? Yes.
            //return new CreateVmData() { groupName = "Group01", serverName = anyVmName, userName = this.userName, localAdminPassword = this.password };
        }

        public string GetRandomVmName()
        {
            return TestDataUtils.GetRandomString(8);
        }

        public void ReadConfigTestUser(DataSet config)
        {
            this.userName = config.Tables[6].Rows[0].ItemArray[0].ToString();
            this.password = config.Tables[6].Rows[0].ItemArray[1].ToString();
        }


    }
}

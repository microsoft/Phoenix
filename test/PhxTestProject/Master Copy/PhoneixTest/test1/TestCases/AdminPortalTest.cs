namespace Phoenix.Test.UI.TestCases
{
    using System;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.UI;

    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework;
    //using Phoenix.Test.UI.Framework.Authentication;
    //using Phoenix.Test.UI.Framework.Configuration;
    using Phoenix.Test.UI.Framework.Controls;
    //using Phoenix.Test.UI.Framework.Shared;
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.Common;
    using Microsoft.VisualStudio.TestTools;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    //[SignInAs(EUserTypes.Regular)]
    public class AdminPortalTest : WAPTestBase
    {
        private FirefoxDriver driver;
        private AdminLoginPage loginPage;

        private string userName;
        private string password;
        private string url;

        [TestInitialize]
        public override void TestInitialize()
        {
            this.driver = new FirefoxDriver();

            driver.Url = "https://khphoenixsql2.redmond.corp.microsoft.com:30091";
            this.userName = "v-willc@microsoft.com";
            this.password = GetPassword();

            this.loginPage = new AdminLoginPage(driver);
            loginPage.Login(userName, password);

            driver.Manage().Window.Maximize();
            System.Threading.Thread.Sleep(10000);
        }


        [TestMethod,
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify administrator can create a user account.")]
        public void AdminCreateUserAccountTest()
        {
            TestInitialize();
            var smpPage = new SmpPage(this.driver);

            var data = GetCreatePlanData();

            smpPage.CreatePlanFromNewButton(data);
        }


        public CreatePlanData GetCreatePlanData()
        {
            string anyPlanName = GetRandomPlanName();
            return new CreatePlanData() { planName = anyPlanName, groupName = "Group01", userName = "test01@microsoft.com", localAdminPassword = "Password0)" };
        }

        public string GetRandomPlanName()
        {
            return "Plan" + TestDataUtils.GetRandomString(4);
        }

















































        private string GetPassword()
        {
            return "XiaoGe1225";
        }
    }
}

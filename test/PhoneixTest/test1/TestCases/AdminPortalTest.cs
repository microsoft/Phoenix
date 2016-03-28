namespace Phoenix.Test.UI.TestCases
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Support.UI;
    using Phoenix.Test.Common;
    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;


    [TestClass]
    public class AdminPortalTest : WAPTestBase
    {
        private string userName;
        private string password;
        private string serverName;
        private string clientId;
        private string clientKey;
        private string tenantId;
        private string azureSubscription;

        public AdminPortalTest(string userAccount, string password, string serverName, string clientId, string clientKey, string tenantId, string azureSubscription)
        {
            this.userName = userAccount;
            this.password = password;
            this.serverName = serverName;

            this.clientId = clientKey;
            this.clientKey = clientKey;
            this.tenantId = tenantId;
            this.azureSubscription = azureSubscription;
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            if (driver == null)
                base.TestInitialize();
            Log.Information("Start test init.");
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://" + serverName + ":30091");

            Log.Information("Get user and password.");

            var authWindow = this.driver.Wait(ExpectedConditions.AlertIsPresent(), 5000);
            if (authWindow != null)
            {
                authWindow.SetAuthenticationCredentials(userName, password);
                authWindow.Accept();
            }

            // wait for redirect complete
            this.driver.Wait(ExpectedConditions.TitleContains("Azure"));
            this.driver.WaitForAjax(20*1000);
        }


        [TestMethod,
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify administrator can create a plan.")]
        public void AdminCreatePlanTest()
        {
            Log.Information("---Start AdminCreatePlanTest---");
            TestInitialize();
            var smpPage = new SmpPage(this.driver);

            var data = GetCreatePlanData();

            smpPage.CreatePlanFromNewButton(data);

        }

        [TestMethod,
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify administrator can create a add-on.")]
        public void AdminCreateAddonTest(CreateAddonData data)
        {
            Log.Information("---Start AdminCreateAddonTest---");
            TestInitialize();
            var smpPage = new SmpPage(this.driver);

            smpPage.CreateAddonFromNewButton(data);
        }

        public void AdminOnboardSubscriptionTest(CreatePlanData planData, CreateAddonData data, string subscriptionName)
        {
            Log.Information("---Open Add-ons List Page...---");
            TestInitialize();
            var smpPage = new SmpPage(this.driver);

            Log.Information("---Open Add-ons List Page...---");
            Log.Information("Find main menu ...");
            smpPage.GetMainMenu_AdminPortal();
            smpPage.OpenMenuPlans();
            
            var page = new AddonListPage(this.driver);
            Log.Information("---Select Add-ons tab...---");
            page.SelectAddonsTab();
            Log.Information("---Click Add-on " + data.addonName + " and check details...---");
            page.SelectAddonInTableAndCheckDatails(data.addonName);

            var configPage = new AddonConfigPage(this.driver); string name = "Cmp Wap Extension";
            Log.Information("---Click Add-on service " + name + " and check details...---");
            configPage.SelectAddonServiceInTableAndCheckDatails(name);

            var subscptPage = new SubscriptionPage(this.driver);
            Log.Information("---Onboarding subscripton...---");
            subscptPage.OnboardSubscription(planData, subscriptionName);
        }


        public void AdminConfigureAddon()
        {
            var addonConfigPage = new AddonConfigPage(this.driver);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        public CreatePlanData GetCreatePlanData()
        {
            string anyPlanName = GetRandomPlanName();
            return new CreatePlanData() { planName = anyPlanName, groupName = "Group01", userName = "test01@microsoft.com", localAdminPassword = "Password0)" };
        }

        public CreateAddonData GetCreateAddonData(string clientId, string clientKey, string tenantId, string azureSubscription)
        {
            string anyAddonName = GetRandomAddonName();
            return new CreateAddonData() { addonName = anyAddonName, clientId = clientId, clientKey = clientKey, tenantId = tenantId, azureSubscription = azureSubscription};
        }

        public string GetRandomPlanName()
        {
            return "Plan" + TestDataUtils.GetRandomString(4);
        }

        public string GetRandomAddonName()
        {
            return "Addon" + TestDataUtils.GetRandomString(4);
        }

        public string GetRandomSubscriptionName()
        {
            return "Subsc" + TestDataUtils.GetRandomString(4);
        }
    }
}

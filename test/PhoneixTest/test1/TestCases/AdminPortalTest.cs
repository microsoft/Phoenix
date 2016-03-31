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
        private string tenantUserAccount;
        private string tenantUserPassword;

        public AdminPortalTest(
            string userAccount, 
            string password, 
            string serverName, 
            string clientId, 
            string clientKey, 
            string tenantId, 
            string azureSubscription,
            string tenantUserAccount,
            string tenantUserPassword)
        {
            this.userName = userAccount;
            this.password = password;
            this.serverName = serverName;

            this.clientId = clientKey;
            this.clientKey = clientKey;
            this.tenantId = tenantId;
            this.azureSubscription = azureSubscription;
            this.tenantUserAccount = tenantUserAccount;
            this.tenantUserPassword = tenantUserPassword;
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
            this.driver.WaitForAjax(30*1000);
        }


        [TestMethod,
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify administrator can create a plan.")]
        public void AdminCreatePlanTest(CreatePlanData data)
        {
            Log.Information("---Start AdminCreatePlanTest---");
            TestInitialize();
            var smpPage = new SmpPage(this.driver);
            smpPage.CreatePlanFromNewButton(data);

        }

        public void AdminOnboardSubscriptionTest(CreatePlanData planData, string subscriptionName)
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
            page.SelectPlansTab();
            
            Log.Information("---Click Add-on " + planData.planName + " and check details...---");
            page.SelectPlanInTableAndCheckDatails(planData.planName);

            var configPage = new AddonConfigPage(this.driver); string name = "Cmp Wap Extension";
            Log.Information("---Click Add-on service " + name + " and check details...---");
            configPage.SelectAddonServiceInTableAndCheckDatails(name);

            var subscptPage = new SubscriptionPage(this.driver);
            Log.Information("---Onboarding subscripton...---");
            subscptPage.OnboardSubscription(planData, subscriptionName);

            Log.Information("Added subscription to plan");
        }

        public void AdminChangePlanAccess(CreatePlanData planData)
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
            page.SelectPlansTab();

            Log.Information("---Click Add-on " + planData.planName + " and check details...---");
            page.SelectPlanInTable(planData.planName);
            page.ChangePlanAccess();
        }


        public void AdminCreateUserTest(CreateTenantUserData data,string planName)
        {
            Log.Information("---Open Service Management Portal Page...---");
            TestInitialize();
            var smpPage = new SmpPage(this.driver);
            smpPage.CreateUserFromNewButton(data,planName);
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

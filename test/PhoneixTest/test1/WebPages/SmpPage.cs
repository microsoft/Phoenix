
namespace Phoenix.Test.UI.Framework.WebPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.UI.Framework;
    using Phoenix.Test.UI.Framework.Controls;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.Data;

    public class SmpPage : Page
    {
        public SmpPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.ClassName, Using = "wizard-button-cancel")]
        private HtmlButton btnCloseFirstTimeWizard { get; set; }

        [FindsBy(How = How.ClassName, Using = "fxs-drawertaskbar-newbutton")]
        protected HtmlButton btnNew { get; set; }

        [FindsBy(How = How.ClassName, Using = "fxs-drawer-drawermenu")]
        protected HtmlSection drawer { get; set; }

        //[FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        //[FindsBy(How = How.Id, Using = "__fx-grid13")] // Not use, random grid Id !!!
        private HtmlTable tableAzureVMs { get; set; }

        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableAllItems { get; set; }

        ////[FindsBy(How = How.Id, Using = "fxshell-nav1-items")]
        protected HtmlScrollBarMenu_Tenant mainMenuTenantPortal { get; set; }

        protected HtmlScrollBarMenu_Admin mainMenuAdminPortal { get; set; }

        [FindsBy(How = How.Id, Using = "fxshell-nav1-items")]
        protected HtmlScrollBarMenu_Admin mainMenu { get; set; }

        [FindsBy(How = How.Name, Using = "Service Management")]
        protected HtmlDiv smp { get; set; }


        public bool IsFirstTimeLogin
        {
            get { return this.btnCloseFirstTimeWizard != null; }
        }

        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void CreateVmFromNewButton(CreateVmData data)
        {
            Browser.WaitForAjax();

            Log.Information("---Click New button---");
            OpenDrawer(); 
            Log.Information("---Select Create VM---");
            this.drawer.SelectItem("AZURE VMS");
            this.drawer.SelectItem("CREATE AZURE VM");

            Log.Information("---Go through wizard to create VM---");
            var createVmWizard = new CreateVmWizard(this.Browser);
            createVmWizard.Step1(data); createVmWizard.GoNext();
            createVmWizard.Step2(data); createVmWizard.GoNext();
            createVmWizard.Step3(data); createVmWizard.Complete();
            Log.Information("---Create VM request send successfully---");
        }

        public void CreateVmFromMainMenu(CreateVmData data)
        {
            Browser.WaitForAjax();

            Log.Information("Find main menu ...");
            GetMainMenu_TenantPortal();
            Log.Information("---Select Create VM---");
            this.mainMenuTenantPortal.SelectAzureVms();

            //this.drawer.SelectItem("AZURE VMS");
            //this.drawer.SelectItem("CREATE AZURE VM");

            Log.Information("---Go through wizard to create VM---");
            var createVmWizard = new CreateVmWizard(this.Browser);
            createVmWizard.Step1(data); createVmWizard.GoNext();
            createVmWizard.Step2(data); createVmWizard.GoNext();
            createVmWizard.Step3(data); createVmWizard.Complete();
            Log.Information("---Create VM request send successfully---");
        }

        public void CreatePlanFromNewButton(CreatePlanData data)
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer();
            Log.Information("---Select Create Plan---");
            this.drawer.SelectItem("PLAN");
            this.drawer.SelectItem("CREATE PLAN");

            Log.Information("---Go through wizard to create plan---");
            var createPlanWizard = new CreatePlanWizard(this.Browser);
            createPlanWizard.Step1(data); createPlanWizard.GoNext();
            createPlanWizard.Step2(data); createPlanWizard.GoNext();
            createPlanWizard.Step3(data); createPlanWizard.Complete();
            Log.Information("---Create plan request send successfully---");
        }

        public void CreateAddonFromNewButton(CreateAddonData data)
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer();
            Log.Information("---Select Create Addon---");
            this.drawer.SelectItem("PLAN");
            this.drawer.SelectItem("CREATE ADD-ON");

            Log.Information("---Go through wizard to create Addon---");
            var createAddonWizard = new CreateAddonWizard(this.Browser);
            createAddonWizard.Step1(data); createAddonWizard.GoNext();
            createAddonWizard.Step2(data); createAddonWizard.Complete();            
            Log.Information("---Create add-on request send successfully---");


        }

        public void OpenDrawer()
        {
            Log.Information("Click New button to open drawer.");
            this.btnNew.Click();
        }

        public void OpenMenuPlans()
        {
            //if (this.mainMenuAdminPortal.Items.Count == 0)
            //    Log.Information("Count is 0.");
            //else
            //{
            //    foreach (var item in this.mainMenuAdminPortal.Items)
            //    {
            //        Log.Information("key: " + item.Key + " | " + "value: " + item.Value.ToString());
            //    }
            //}
            if (this.mainMenuAdminPortal == null)
                Log.Information("cannot find main menu Admin Portal");
            this.mainMenuAdminPortal.SelectPlans();
        }

        public void GetMainMenu_TenantPortal()
        {
            this.mainMenuTenantPortal = new HtmlScrollBarMenu_Tenant(this, By.Id("fxshell-nav1-items"));
        }

        public void GetMainMenu_AdminPortal()
        {
            this.mainMenuAdminPortal = new HtmlScrollBarMenu_Admin(this, By.Id("fxshell-nav1-items"));
        }

        public bool VerifyVmCreated(CreateVmData data)
        {
            Log.Information("Wait 30 sec for the new VM refresh ...");
            System.Threading.Thread.Sleep(30000);

            Log.Information("Find main menu ...");
            GetMainMenu_TenantPortal();
            this.mainMenuTenantPortal.SelectAzureVms();

            Log.Information("Find Azure VMs table ...");
            this.tableAzureVMs = new HtmlTable(this, By.ClassName("fx-grid-full"));
            Log.Information("Find Azure VMs table row for: " + data.serverName + " ...");
            var row = this.tableAzureVMs.Rows[data.serverName];
            Log.Information("Check server status ...");
            row.Status.Click();

            string buildStatus = this.tableAzureVMs.RowValues[data.serverName][0];
            Log.Information("Build Status: " + buildStatus);
            if (buildStatus.ToLower().Contains("complete"))
            {
                Log.Information("Create VM success!");
                return true;
            }
            else
            {
                Log.Information("Create VM failed. Error Message: " + this.tableAzureVMs.RowValues[data.serverName][1]);
                return false;
            }
        }



        public void CloseFirstTimeWizard()
        {
            this.btnCloseFirstTimeWizard.Click();
        }

    }
}

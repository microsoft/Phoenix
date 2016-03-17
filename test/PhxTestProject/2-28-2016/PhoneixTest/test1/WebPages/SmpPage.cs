
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
        private HtmlButton btnNew { get; set; }

        [FindsBy(How = How.ClassName, Using = "fxs-drawer-drawermenu")]
        private HtmlSection drawer { get; set; }

        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableAzureVMs { get; set; }

        [FindsBy(How = How.Id, Using="fxshell-nav1-scroll")]
        private HtmlScrollBarMenu mainMenu { get; set; }

        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }


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
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
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

            Log.Information("---Go through wizard to create addon---");
            var createAddonWizard = new CreateAddonWizard(this.Browser);
            createAddonWizard.Step1(data); createAddonWizard.GoNext();
            createAddonWizard.Step2(data); createAddonWizard.Complete();
            createAddonWizard.Step3(data); createAddonWizard.Complete();
            Log.Information("---Create add-on request send successfully---");
        }

        public void OpenDrawer()
        {
            Log.Information("Click New button to open drawer.");
            this.btnNew.Click();
        }

        public bool VerifyVmCreated(CreateVmData data)
        {
            this.mainMenu.SelectAzureVms();

            var row = this.tableAzureVMs.Rows[data.serverName];
            if (row == null)
            {
                Log.Error("Could not find new VM: " + data.serverName + " in VMs list.");
                return false;
            }

            if (row.Status.Text != "Completed")
            {
                Log.Information("Create VM success. VM name: " + data.serverName);
                return true;
            }
            else
            {
                Log.Error("Create VM failed. VM name: " + data.serverName + ". Error messge: " + row.Detail.Text);
                return false;
            }
        }

        public void CloseFirstTimeWizard()
        {
            this.btnCloseFirstTimeWizard.Click();
        }

    }
}

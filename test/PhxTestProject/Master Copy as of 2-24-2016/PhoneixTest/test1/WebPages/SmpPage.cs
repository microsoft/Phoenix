
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
            Browser.WaitForAjax();
            OpenDrawer();
            this.drawer.SelectItem("AZURE VMS");
            this.drawer.SelectItem("CREATE AZURE VM");

            var createVmWizard = new CreateVmWizard(this.Browser);
            createVmWizard.Step1(data); createVmWizard.GoNext();
            createVmWizard.Step2(data); createVmWizard.GoNext();
            createVmWizard.Step3(data); createVmWizard.Complete();
        }

        public void CreatePlanFromNewButton(CreatePlanData data)
        {
            Browser.WaitForAjax();
            OpenDrawer();
            this.drawer.SelectItem("PLAN");
            this.drawer.SelectItem("CREATE PLAN");

            var createPlanWizard = new CreatePlanWizard(this.Browser);
            createPlanWizard.Step1(data); createPlanWizard.GoNext();
            createPlanWizard.Step2(data); createPlanWizard.GoNext();
            createPlanWizard.Step3(data); createPlanWizard.Complete();
        }

        public void OpenDrawer()
        {
            Log.Information("Click New button to open drawer.");
            this.btnNew.Click();
        }

        public bool VerifyVmCreated(CreateVmData data)
        {
            return this.tableAzureVMs.Items.Keys.Contains(data.serverName);
        }

        public void CloseFirstTimeWizard()
        {
            this.btnCloseFirstTimeWizard.Click();
        }

    }
}

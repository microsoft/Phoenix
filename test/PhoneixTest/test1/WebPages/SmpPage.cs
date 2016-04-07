
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
    using OpenQA.Selenium.Support.UI;
    using OpenQA.Selenium.Interactions;
    using System.Diagnostics;
    using System.Threading;

    public class SmpPage : Page
    {
        public SmpPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.ClassName, Using = "fxs-drawertaskbar-newbutton")]
        protected HtmlButton btnNew { get; set; }

        [FindsBy(How = How.ClassName, Using = "fxs-drawertray-button fx-button")]
        protected HtmlButton completedActions { get; set; }

        [FindsBy(How = How.ClassName, Using = "fxs-drawer-drawermenu")]
        protected HtmlSection drawer { get; set; }

        private HtmlTable tableAzureVMs { get; set; }

        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableAllItems { get; set; }

        protected HtmlScrollBarMenu_Tenant mainMenuTenantPortal { get; set; }

        protected HtmlScrollBarMenu_Admin mainMenuAdminPortal { get; set; }

        [FindsBy(How = How.Id, Using = "fxshell-nav1-items")]
        protected HtmlScrollBarMenu_Admin mainMenu { get; set; }

        [FindsBy(How = How.Name, Using = "Service Management")]
        protected HtmlDiv smp { get; set; }

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
            createVmWizard.Step1(data, this); createVmWizard.GoNext();
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

            Log.Information("---Go through wizard to create VM---");
            var createVmWizard = new CreateVmWizard(this.Browser);
            createVmWizard.Step1(data, this); createVmWizard.GoNext();
            createVmWizard.Step2(data); createVmWizard.GoNext();
            createVmWizard.Complete();
            Log.Information("---Create VM request send successfully---");
        }

        public void CreatePlanFromNewButton(CreatePlanData data)
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer();
            Log.Information("---Select Create Plan---");

            Browser.WaitForAjax();

            this.drawer.SelectItem("PLAN");

            Browser.WaitForAjax();
            this.drawer.SelectItem("CREATE PLAN");

            Log.Information("---Go through wizard to create plan---");
            var createPlanWizard = new CreatePlanWizard(this.Browser);
            createPlanWizard.Step1(data); createPlanWizard.GoNext();
            createPlanWizard.Step2(data); createPlanWizard.GoNext();
            createPlanWizard.Step3(data); createPlanWizard.Complete();
            Log.Information("---Create plan request send successfully---");
        }

        public void CreateUserFromNewButton(TenantData data, string planName)
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer();
            Log.Information("---Select User Account---");

            Browser.WaitForAjax();

            this.drawer.SelectItem("USER ACCOUNT");

            Browser.WaitForAjax();
            this.drawer.SelectItem("QUICK CREATE");

            var emailTextBox = new HtmlTextBox(this, this.Browser.FindElement(By.Id("accountEmail")));
            emailTextBox.SetText(data.emailAddress);
            var passwordTextBox = new HtmlTextBox(this, this.Browser.FindElement(By.Id("accountPassword")));
            passwordTextBox.SetText(data.password);
            var repeatPswTextBox = new HtmlTextBox(this, this.Browser.FindElement(By.Id("confirmPassword")));
            repeatPswTextBox.SetText(data.password);
            Thread.Sleep(1000 * 2);
            IWebElement ee = this.SearchContext.FindElement(By.Id("accountPlanDropDown"));
            SelectElement e = new SelectElement(ee);
            e.WrappedElement.Click();
            e.SelectByText(planName + " (public)");
            Thread.Sleep(1000 * 2);
            var createButton = new HtmlButton(this, this.Browser.FindElement(By.XPath("//*[text()='Create']")));
            createButton.Click();
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
            this.btnNew.ExcuteScriptOnElement(".click()");
        }

        public void MouseHoverByJavaScript(IWebElement targetElement)
        {
            string javaScript = "var evObj = document.createEvent('MouseEvents');" +
                                "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                                "arguments[0].dispatchEvent(evObj);";
            IJavaScriptExecutor js = this.Browser as IJavaScriptExecutor;
            js.ExecuteScript(javaScript, targetElement);
        }

        public void OpenMenuPlans()
        {
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
            Log.Information("Click Completed operation button...");
            Thread.Sleep(1000 * 15);
            var completedOp = new HtmlButton(this, By.ClassName("fxs-drawertray-button"));
            completedOp.Click();

            Log.Information("Check the progress box...");

            var progressBox = new HtmlDiv(this, By.ClassName("fxs-progressbox-header"));
            if (progressBox.Text == "Successfully submitted VM request.")
                return true;
            else
                return false;
        }
    }
}


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


    public class AddonConfigPage : SmpPage
    {
        public AddonConfigPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlButton tabPlans { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:nth-child(2)")]
        private HtmlButton tabAddons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:nth-child(3)")]
        private HtmlButton tabSubscriptions { get; set; }

        //[FindsBy(How = How.Id, Using = "__fx-grid58")]
        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableAddonServices { get; set; }

        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }


        // Addon details..
        private HtmlTextBox clientId;
        private HtmlTextBox clientKey;
        private HtmlTextBox tenantId;
        private HtmlTextBox azureSubscriptioin;


        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void OpenCreateVm()
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer(); 
            Log.Information("---Select Create VM---");
            this.drawer.SelectItem("AZURE VMS");
            this.drawer.SelectItem("CREATE AZURE VM");
        }


        public void SelectAddonServiceInTableAndCheckDatails(CreateAddonData data)
        {
            Browser.WaitForAjax();
            // The row's key should comes from input parameter data.
            string name = "SQL Servers";
            Log.Information("---Find Add-on services table...---");
            this.tableAddonServices = new HtmlTable(this, By.CssSelector("div.fx-scrollbar-scrollable div.fx-grid-container table.fx-grid-full"));
            this.tableAddonServices.Rows[name].SelectAndCheckDatails();
            //this.tableServices.Rows["MySQL Servers"].SelectAndCheckDatails();
            //this.tableServices.Rows["Cmp Wap Extension"].SelectAndCheckDatails();

            ////var subscriptionOnboardPage = new AzureSubscriptionOnboardingPage(this.Browser);
            ////subscriptionOnboardPage.AddSubscription("SubScr001", "SubScription001", data.clientId, data.clientKey, data.tenantId, data.azureSubscription);
        }


    }
}

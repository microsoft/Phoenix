
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

        [FindsBy(How = How.Id, Using = "__fxshell-pivotlist21-0")]
        private HtmlButton tabPlans { get; set; }

        [FindsBy(How = How.Id, Using = "__fxshell-pivotlist21-1")]
        private HtmlButton tabAddons { get; set; }

        [FindsBy(How = How.Id, Using = "__fxshell-pivotlist21-2")]
        private HtmlButton tabSubscriptions { get; set; }

        //[FindsBy(How = How.Id, Using = "__fx-grid58")]
        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableServices { get; set; }

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


        public void OnboardSubscription(CreateAddonData data)
        {
            this.tableServices = new HtmlTable(this, By.ClassName("fx-grid-full"));
            this.tableServices.Rows["Cmp Wap Extension"].SelectAndCheckDatails();

            var subscriptionOnboardPage = new AzureSubscriptionOnboardingPage(this.Browser);
            subscriptionOnboardPage.AddSubscription("SubScr001", "SubScription001", data.clientId, data.clientKey, data.tenantId, data.azureSubscription);




            ////this.clientId = new HtmlTextBox(this, By.Name("")); // ???
            ////this.clientKey = new HtmlTextBox(this, By.Name(""));
            ////this.tenantId = new HtmlTextBox(this, By.Name(""));
            ////this.azureSubscriptioin = new HtmlTextBox(this, By.Name(""));

            ////this.tableAddons.Click();

            ////this.tableAddons = new HtmlTable(this, By.Id(""));

            //////this.clientId.Input(data.clientId);
            //////this.clientKey.Input(data.clientKey);
            //////this.tenantId.Input(data.tenantId);
            //////this.azureSubscriptioin.Input(data.azureSubscription);


        }


    }
}

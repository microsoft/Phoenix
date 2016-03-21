
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


    public class SubscriptionPage : SmpPage
    {
        public SubscriptionPage(IWebDriver browser) : base(browser) { }

        //[FindsBy(How = How.CssSelector, Using = "form.aux-dialog-form div.aux-dialog-item:first-child input.fx-textbox")]
        //[FindsBy(How = How.XPath, Using = "//input[@data-val-length='Azure subscription friendly name is required']")]
        //[FindsBy(How = How.XPath, Using = "//form[@class='aux-dialog-form pln-form']/div[1]/input")]
        //[FindsBy(How = How.CssSelector, Using = "form.aux-dialog-form div:first-child")]
        //[FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:first-child")]
        [FindsBy(How = How.Id, Using = "azuresub-obn-scn")]
        private HtmlTextBox name { get; set; }

        [FindsBy(How = How.Id, Using = "description")]
        private HtmlTextBox description { get; set; }

        [FindsBy(How = How.Id, Using = "clientID")]
        private HtmlTextBox clientId { get; set; }

        [FindsBy(How = How.Id, Using = "clientKey")]
        private HtmlTextBox clientKey { get; set; }

        [FindsBy(How = How.Id, Using = "tenantID")]
        private HtmlTextBox tenantId { get; set; }

        [FindsBy(How = How.Id, Using = "accountId")]
        private HtmlTextBox azureSuscription { get; set; }

        [FindsBy(How = How.Id, Using = "azuresub-btn-add")]
        private HtmlButton submit { get; set; }


        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }


        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void OnboardSubscription(CreatePlanData data, string subscriptionName) // , string cId, string cKey, string tId, string subscpt
        {
          //  System.Threading.Thread.Sleep(6000);

            Log.Information("---Input onboard subscription parameters...---");
            var frame = this.Browser.SwitchTo().Frame("plansIframeWind"); IWebElement e;

            e = frame.FindElement(By.Id("name"));
            this.name = new HtmlTextBox(this, e);
            this.name.Input(subscriptionName);

            e = frame.FindElement(By.Id("description"));
            this.description = new HtmlTextBox(this, e);
            this.description.Input(subscriptionName);

            e = frame.FindElement(By.Id("clientID"));
            this.clientId = new HtmlTextBox(this, e);
            this.clientId.Input(data.clientId);

            e = frame.FindElement(By.Id("clientKey"));
            this.clientKey = new HtmlTextBox(this, e);
            this.clientKey.Input(data.clientKey);

            e = frame.FindElement(By.Id("tenantID"));
            this.tenantId = new HtmlTextBox(this, e);
            this.tenantId.Input(data.tenantId);

            e = frame.FindElement(By.Id("accountId"));
            this.azureSuscription = new HtmlTextBox(this, e);
            this.azureSuscription.Input(data.azureSubscription);

            e = frame.FindElement(By.Id("azuresub-btn-add"));
            this.submit = new HtmlButton(this, e);
            this.submit.Click();

            Log.Information("---Onboarding subscription " + subscriptionName + " success.---");
        }

    }
}

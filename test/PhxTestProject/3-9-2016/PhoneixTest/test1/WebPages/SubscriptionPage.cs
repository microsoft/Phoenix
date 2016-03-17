
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
            Browser.WaitForAjax();

            Log.Information("---Input onboard subscription parameters...---");
            //this.name = new HtmlTextBox(this, By.CssSelector("form.aux-dialog-form div.aux-dialog-item:first-child input.fx-textbox"));
            //this.name = new HtmlTextBox(this, By.XPath("//*[@data-val-length='Azure subscription friendly name is required']"));
            this.name.Input(subscriptionName);
            this.description = new HtmlTextBox(this, By.Id("description"));
            this.description.Input(subscriptionName);

            this.clientId = new HtmlTextBox(this, By.Id("clientID"));
            this.clientId.Input(data.clientId);
            this.clientKey = new HtmlTextBox(this, By.Id("clientKey"));
            this.clientKey.Input(data.clientKey);
            this.tenantId = new HtmlTextBox(this, By.Id("tenantID"));
            this.tenantId.Input(data.tenantId);
            this.azureSuscription = new HtmlTextBox(this, By.Id("accountId"));
            this.azureSuscription.Input(data.azureSubscription);

            this.submit = new HtmlButton(this, By.Id("azuresub-btn-add"));
            this.submit.Click();
        }





    }
}


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


    public class SubscriptioinPage : SmpPage
    {
        public SubscriptioinPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.Id, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlTextBox clientId { get; set; }

        [FindsBy(How = How.Id, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlTextBox clientKey { get; set; }

        [FindsBy(How = How.Id, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlTextBox tenantId { get; set; }

        [FindsBy(How = How.Id, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlTextBox azureSuscription { get; set; }

        [FindsBy(How = How.Id, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlButton submit { get; set; }


        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }


        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void OnboardSubscription(string cId, string cKey, string tId, string subscpt)
        {
            Log.Information("---Input onboard subscription parameters...---");
            this.clientId.Input(cId);
            this.clientKey.Input(cKey);
            this.tenantId.Input(tId);
            this.azureSuscription.Input(subscpt);

            this.submit.Click();
        }





    }
}

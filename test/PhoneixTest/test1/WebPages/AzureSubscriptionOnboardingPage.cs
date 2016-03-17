
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


    public class AzureSubscriptionOnboardingPage : SmpPage
    {
        public AzureSubscriptionOnboardingPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.Id, Using = "name")]
        private HtmlTextBox name;

        [FindsBy(How = How.Id, Using = "description")]
        private HtmlTextBox description;

        [FindsBy(How = How.Id, Using = "subscriptionType")]
        private HtmlComboBox subscriptionType;

        [FindsBy(How = How.Id, Using = "accountId")]
        private HtmlTextBox azureSubscriptionId;

        [FindsBy(How = How.Id, Using = "tenantId")]
        private HtmlTextBox tenantId;

        [FindsBy(How = How.Id, Using = "clientId")]
        private HtmlTextBox clientId;

        [FindsBy(How = How.Id, Using = "clientKey")]
        private HtmlTextBox clientKey;

        [FindsBy(How = How.Id, Using = "azuresub-btn-add")]
        private HtmlButton addSubscription;

        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }


        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void AddSubscription(string name, string description, string clientId,
            string clientKey, string tenantId, string azureSubscription)
        {
            this.name.Input(name);
            this.description.Input(description);


            this.clientId.Input(clientId);
            this.clientKey.Input(clientKey);
            this.tenantId.Input(tenantId);
            this.azureSubscriptionId.Input(azureSubscription);

            this.addSubscription.Click();
        }



    }
}

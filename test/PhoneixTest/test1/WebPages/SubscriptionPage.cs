
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
    using OpenQA.Selenium.Interactions;

    public class SubscriptionPage : SmpPage
    {

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
        private HtmlButton AddSubcrpt { get; set; }

        [FindsBy(How = How.Id, Using = "azuresub-btn-add-to-plan")]
        private HtmlButton AddSelectedSubscrpt { get; set; }

        [FindsBy(How = How.Id, Using = "save-plan-config")]
        private HtmlButton Save { get; set; }

        [FindsBy(How = How.Id, Using = "azuresub-btn-remove-from-plan")]
        private HtmlButton RemoveSubscrpt { get; set; }

        [FindsBy(How = How.Name, Using = "Service Management")]
        private HtmlDiv smp { get; set; }

        public SubscriptionPage(IWebDriver browser) : base(browser) { }
      
        public override HtmlControl VerifyPageElement
        {
            get { return smp; }
        }

        public void OnboardSubscription(CreatePlanData data, string subscriptionName) // , string cId, string cKey, string tId, string subscpt
        {
            Log.Information("---Input onboard subscription parameters...---");
            var frame = this.Browser.SwitchTo().Frame("plansIframeWind");
            IWebElement e;

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
            this.AddSelectedSubscrpt = new HtmlButton(this, frame.FindElement(By.Id("azuresub-btn-add-to-plan")));
            this.Save = new HtmlButton(this, frame.FindElement(By.Id("save-plan-config")));
            this.AddSubcrpt = new HtmlButton(this, frame.FindElement(By.Id("azuresub-btn-add")));
            this.RemoveSubscrpt = new HtmlButton(this, frame.FindElement(By.Id("azuresub-btn-remove-from-plan")));

            e = frame.FindElement(By.Id("save-plan-config"));

            this.AddSubcrpt.Click();

            SelectSubscription(subscriptionName);

            this.AddSelectedSubscrpt.Click();
            this.Save.Click();

            Log.Information("---Onboarding subscription " + subscriptionName + " success.---");

            ScrollToElement(e, this.Browser);
            ConfigPlan();
            this.Save.Click();
            Log.Information("---Start plan configuration---");
            System.Threading.Thread.Sleep(1000 * 3);
        }

        public void SelectSubscription(string name)
        {
            var searchText = "//*[text()='match']";
            searchText = searchText.Replace("match", name);
            var row = new HtmlControl(this, By.XPath(searchText));
            row.Click();
        }

        public void ScrollToElement(IWebElement e, IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", e);
            System.Threading.Thread.Sleep(500);
        }

        public void ConfigPlan()
        {
            var checkBoxes = this.Browser.FindElements(By.ClassName("storageLoggingOptions"));
            foreach (var vb in checkBoxes)
            {
                var checkBox = new HtmlCheckBox(this, vb);
                checkBox.Check();
            }

            this.Save.Click();
        }

    }
}


namespace Phoenix.Test.UI.Framework.WebPages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework.Controls;
    using Phoenix.Test.UI.Framework.Logging;

    class CreateAddonWizard : Page
    {
        [FindsBy(How = How.ClassName, Using = "wizard-button-next")]
        private HtmlButton next { get; set; }

        [FindsBy(How = How.ClassName, Using = "wizard-button-back")]
        private HtmlButton back { get; set; }

        // step 1
        [FindsBy(How = How.Id, Using = "planEntityFriendlyName")]
        private HtmlCheckBox addonEntityFriendlyName { get; set; }

        // step 2
        [FindsBy(How = How.Id, Using = "availableService2")]
        private HtmlCheckBox cmpWapExtension { get; set; }

        // step 3

        public CreateAddonWizard(IWebDriver browser)
            : base(browser)
        {
        }

        public override HtmlControl VerifyPageElement
        {
            get { return next; }
        }

        public void GoNext()
        {
            this.next.ExcuteScriptOnElement(".click()");
            System.Threading.Thread.Sleep(500);
        }

        public void Complete()
        {
            this.next.Click();
        }

        public void GoBack()
        {
            this.back.Click();
        }

        public void Step1(CreateAddonData data)
        {
            Log.Information("Input Add-on Friendly Name.");
            this.addonEntityFriendlyName.SetText(data.addonName);
        }

        public void Step2(CreateAddonData data)
        {
            this.cmpWapExtension = new HtmlCheckBox(this, By.Id("availableService2"));
            this.cmpWapExtension.Check();
        }

        public void Step3(CreateAddonData data)
        {
        }
    }
}

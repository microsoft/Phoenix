namespace Phoenix.Test.UI.Framework.WebPages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using OpenQA.Selenium.Support.UI;
    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework.Controls;
    using Phoenix.Test.UI.Framework.Logging;

    public class CreateVmFromTemplateWizard : Page
    {
        public CreateVmFromTemplateWizard(IWebDriver browser)
            : base(browser) 
        {
        }

        [FindsBy(How = How.Id, Using = "TemplateVm")]
        private HtmlTextBox Name { get; set; }

        [FindsBy(How = How.Id, Using = "TemplateText")]
        private HtmlTextBox Template { get; set; }

        [FindsBy(How = How.ClassName, Using = "wizard-button-next")]
        private HtmlButton Next { get; set; }

        public override HtmlControl VerifyPageElement
        {
            get { return this.Name; }
        }

        internal void CreateFromTemplate(string name, string templateData)
        {
            this.Name.SetText(name);
            //  this.Template//.SetText(templateData);
            //     .ExcuteScriptOnElement(".value=", templateData);

            ((IJavaScriptExecutor)this.Browser).ExecuteScript("arguments[0].value=arguments[1];", this.Template.Element, templateData);
            this.Next.Click();
        }
    }
}

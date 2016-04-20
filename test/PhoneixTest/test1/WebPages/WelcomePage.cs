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
    public class WelcomePage:Page
    {
        public WelcomePage(IWebDriver browser) : base(browser) { }

        private HtmlButton btnCloseFirstTimeWizard { get; set; }
      
        private HtmlButton btnNext { get; set; }

         public override HtmlControl VerifyPageElement
         {
             get { return smp; }
         }

         protected HtmlDiv smp { get; set; }

        public bool IsFirstTimeLogin
        {
            get { return this.btnCloseFirstTimeWizard != null; }
        }

        public void CloseWelcomeWizard()
        {
            var buttons = this.Browser.FindElements(By.XPath("//*[text()='Cancel']"));
            var cancelBtn = new HtmlButton(this, buttons.First(b => b.TagName == "img"));
            cancelBtn.Click();
        }

        public void HandleWelcomeWizard()
        {
            var button = this.Browser.FindElement(By.XPath("//a[@title='Next']"));
            var nextBtn = new HtmlButton(this, button);
            nextBtn.Click();
            Thread.Sleep(1000 * 3);
            nextBtn.Click();
            Thread.Sleep(1000 * 3);
            nextBtn.Click();
            Thread.Sleep(1000 * 3);
            nextBtn.Click();
            Thread.Sleep(1000 * 3);
            nextBtn.Click();
            var btn = this.Browser.FindElement(By.XPath("//div[@title='Close']"));
            var closeBtn = new HtmlButton(this, btn);
            closeBtn.Click();
        }
    }
}

namespace Phoenix.Test.UI.Framework.WebPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.UI.Framework.Controls;
    using Phoenix.Test.UI.Framework.WebPages;

    using Phoenix.Test.Common;

    public class AdminLoginPage : Page
    {
        private const string loginWindowTitle = "Authentication Required";

        public AdminLoginPage(IWebDriver browser)
            : base(browser) 
        {
        }

        public override HtmlControl VerifyPageElement
        {
            get { return new HtmlControl(this, By.Name(loginWindowTitle)); }
        }

        public void Login(string userName, string password)
        {
            System.Threading.Thread.Sleep(10000);
            WindowsApplicationHelper.Login(loginWindowTitle, userName, password);
            System.Threading.Thread.Sleep(10000);
        }
    }
}

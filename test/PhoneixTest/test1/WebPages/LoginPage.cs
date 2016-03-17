

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

    public class LoginPage : Page
    {


        public string machineName = "phoenixtest2";
        public int portAdmin = 30091;
        public int portTenant = 30081;

        //public string UrlLocation
        //{
        //    get { return "https://" + machineName + "northamerica.corp.microsoft.com:" + portTenant; }
        //}

        public override HtmlControl VerifyPageElement
        {
            get { return submit; }
        }


        #region Elements

        [FindsBy(How = How.ClassName, Using = "kt-login-button")]
        private HtmlButton login { get; set; }

        [FindsBy(How = How.ClassName, Using = "kt-signup-button")]
        private HtmlTextBox singUp { get; set; }

        [FindsBy(How = How.Name, Using = "LoginMe-Button")]
        private HtmlButton submit { get; set; }

        [FindsBy(How = How.Name, Using = "EmailAddress")]
        private HtmlTextBox emailAddress { get; set; }

        [FindsBy(How = How.Name, Using = "Password")]
        private HtmlTextBox password { get; set; }

        #endregion

        public LoginPage(IWebDriver browser) : base(browser) 
        {
        }

        public SmpPage Login(string userName, string password)
        {
            this.emailAddress.SetText(userName);
            this.password.SetText(password);

            this.submit.ClickButtonToNavigate();

            var page = new SmpPage(Browser);
            return page;
        }




    }
}

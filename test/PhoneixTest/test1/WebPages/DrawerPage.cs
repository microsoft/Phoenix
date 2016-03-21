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

    class DrawerPage : Page
    {
        [FindsBy(How = How.ClassName, Using = "fx-scrollbar-content")]
        private HtmlSection drawerScrollbar { get; set; }

        public DrawerPage(IWebDriver browser)
            : base(browser) 
        {
        }

        public override HtmlControl VerifyPageElement
        {
            get { return drawerScrollbar; }
        }

        public void SelectMenu(string itemName)
        {
            this.drawerScrollbar.SelectItem(itemName);
        }
    }
}

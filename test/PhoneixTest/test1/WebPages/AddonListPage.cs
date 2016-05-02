
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

    public class AddonListPage : SmpPage
    {
        public AddonListPage(IWebDriver browser) : base(browser) { }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:first-child")]
        private HtmlButton tabPlans { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:nth-child(2)")]
        private HtmlButton tabAddons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.fxshell-tabcontainer ul li:nth-child(3)")]
        private HtmlButton tabSubscriptions { get; set; }

        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tableAddons { get; set; }

        [FindsBy(How = How.ClassName, Using = "fx-grid-full")]
        private HtmlTable tablePlans { get; set; }

        public void OpenCreateVm()
        {
            Log.Information("---Click New button---");
            Browser.WaitForAjax();
            OpenDrawer();
            Log.Information("---Select Create VM---");
            this.drawer.SelectItem("AZURE VMS");
            this.drawer.SelectItem("CREATE AZURE VM");
        }

        public void SelectAddonInTableAndCheckDatails(string name)
        {
            Log.Information("---Find Add-ons table...---");
            this.tableAddons = new HtmlTable(this, By.Id("__fx-grid3"));
            this.tableAddons.Rows[name].SelectAndCheckDatails();
        }

        public void SelectPlanInTableAndCheckDatails(string name)
        {
            Log.Information("---Find Plan table...---");
            this.tablePlans = new HtmlTable(this, By.Id("__fx-grid2"));
            this.tablePlans.Rows[name].SelectAndCheckDatails();
        }

        public void SelectPlanInTable(string name)
        {
            Log.Information("---Find Plan table...---");
            this.tablePlans = new HtmlTable(this, By.Id("__fx-grid2"));
            this.tablePlans.Rows[name].Select();
        }

        public void ChangePlanAccess()
        {
            System.Threading.Thread.Sleep(1000 * 3);
            var changeAccessButton = new HtmlButton(this, By.XPath("//*[text()='Change access']"));
            changeAccessButton.Click();

            System.Threading.Thread.Sleep(1000 * 3);
            var buttons = this.Browser.FindElements(By.XPath("//*[text()='Public']"));
            var publicButton = new HtmlButton(this, buttons.First(b => b.TagName == "a"));
            publicButton.Click();

            System.Threading.Thread.Sleep(1000 * 3);
            var confirmButtons = this.Browser.FindElements(By.ClassName("fxs-confirmation-button"));
            var yesButton = new HtmlButton(this, confirmButtons.First(b => b.Text == "YES"));
            yesButton.Click();
            System.Threading.Thread.Sleep(1000 * 5);
            Log.Information("---Done changing plan access...---");  

        }

        public void SelectAddonsTab()
        {
            this.tabAddons.ExcuteScriptOnElement(".click()");
        }

        public void SelectPlansTab()
        {
            this.tabPlans.ExcuteScriptOnElement(".click()");
        }

        public void OnboardSubscription(CreateAddonData data)
        {
            SelectAddonInTableAndCheckDatails(data.addonName);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlScrollBarMenu.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html tag structure like this: 
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section > .section-title ~ .section-body > .section-item.
    /// </summary>
    public class HtmlScrollBarMenu_Admin : HtmlControl
    {
        private Page page;
     
        /// <summary>
        /// Construct HtmlSection found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlScrollBarMenu_Admin(Page page, By by)
            : base(page, by)
        {
            this.page = page;
        }

        /// <summary>
        /// Construct HtmlSection by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlScrollBarMenu_Admin(Page page, IWebElement element)
            : base(page, element)
        {
            this.page = page;
        }

        public void SelectAllItems()
        {
            Log.Information("Select All Items");
            this.findMenuItemByName("ALL ITEMS").Select();
        }
        public void SelectAzureVmCloud()
        {
            Log.Information("Select Azure Vm Cloud");
            this.findMenuItemByName("AZURE VM CLOUD").Select();
        }
        public void SelectWebSiteClouds()
        {
            Log.Information("Select Web Site Clouds");
            this.findMenuItemByName("WEB SITE CLOUDS").Select();
        }
        public void SelectVmClouds()
        {
            Log.Information("Select Vm Clouds");
            this.findMenuItemByName("VM CLOUDS").Select();
        }
        public void SelectServiceBusClouds()
        {
            Log.Information("Select Service Bus Clouds");
            this.findMenuItemByName("SERVICE BUS CLOUDS").Select();
        }
        public void SelectSqlServers()
        {
            Log.Information("Select SQL Servers");
            this.findMenuItemByName("SQL SERVERS").Select();
        }
        public void SelectMySqlServers()
        {
            Log.Information("Select MySQL Servers");
            this.findMenuItemByName("MYSQL SERVERS").Select();
        }
        public void SelectAutomation()
        {
            Log.Information("Select Automation");
            this.findMenuItemByName("AUTOMATION").Select();
        }
        //public void SelectTeamAccessControl()
        //{
        //    Log.Information("Select Team Access Control");
        //    Items[arr[8]].Select();
        //}
        public void SelectPlans()
        {
            Log.Information("Select Plans");
            this.findMenuItemByName("PLANS").Select();
        }
        public void SelectUserAccounts()
        {
            Log.Information("Select User Accounts");
            this.findMenuItemByName("USER ACCOUNTS").Select();
        }

        private HtmlScrollBarMenuItem findMenuItemByName(string name)
        {
            var item = this.Element.FindElements(By.CssSelector(".fxshell-nav1-item")).Where(e => e.Displayed && e.Text.Contains(name)).FirstOrDefault();
            if (item == null)
            {
                throw new NotFoundException("Menu item not found by name: " + name);
            }

            return new HtmlScrollBarMenuItem(page, item);
        }
    }
}
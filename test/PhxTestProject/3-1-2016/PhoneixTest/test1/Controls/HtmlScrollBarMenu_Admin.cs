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
    using System.Collections.Generic;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section > .section-title ~ .section-body > .section-item.
    /// </summary>
    public class HtmlScrollBarMenu_Admin : HtmlControl
    {
        private Page page;
        private string[] arr = { "ALL ITEMS", "AZURE VM CLOUD", "WEB SITE CLOUDS", "VM CLOUDS", 
                                     "SERVICE BUS CLOUDS", "SQL SERVERS", "MYSQL SERVERS", "AUTOMATION", 
                                     "TEAM ACCESS CONTROL", "PLANS", "USER ACCOUNTS" };

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

        /////// <summary>
        /////// Gets the section title text.
        /////// </summary>
        ////public string Title
        ////{
        ////    get
        ////    {
        ////        return this.Element.FindElement(By.ClassName("section-title")).Text;
        ////    }
        ////}

        /////// <summary>
        /////// Gets the section edit link, not all situations will contains a edit link, so may be you should verify is it exist before you using it.
        /////// </summary>
        ////public HtmlLink EditLink
        ////{
        ////    get
        ////    {
        ////        return new HtmlLink(this.page, this.Element.FindElement(By.Id("editAccountLink")));
        ////    }
        ////}

        /// <summary>
        /// Gets all section items as a Dictionary.
        /// </summary>
        public Dictionary<string, HtmlScrollBarMenuItem> Items
        {
            get
            {
                Dictionary<string, HtmlScrollBarMenuItem> sectionItems = new Dictionary<string, HtmlScrollBarMenuItem>();
                var sectionContainers = this.Element.FindElements(By.CssSelector(".fxshell-nav1-item"));

                for (int i = 0; i < arr.Length; i++)
                {   // For Tenant Portal, there are 3 items in scrollbarmenu - ALL ITEMS, AZURE VMS, MY ACCOUNT.
                    if (sectionContainers[i].Displayed)
                    {
                        var item = new HtmlScrollBarMenuItem(this.page, sectionContainers[i]);
                        sectionItems.Add(arr[i], new HtmlScrollBarMenuItem(this.page, sectionContainers[i]));
                    }
                }

                foreach (var item in Items)
                { System.Windows.Forms.MessageBox.Show(item.Key); }

                return sectionItems;
            }
        }

        public void SelectAllItems()
        {
            Log.Information("Select All Items");
            Items[arr[0]].Select();
        }
        public void SelectAzureVmCloud()
        {
            Log.Information("Select Azure Vm Cloud");
            Items[arr[1]].Select();
        }
        public void SelectWebSiteClouds()
        {
            Log.Information("Select Web Site Clouds");
            Items[arr[2]].Select();
        }
        public void SelectVmClouds()
        {
            Log.Information("Select Vm Clouds");
            Items[arr[3]].Select();
        }
        public void SelectServiceBusClouds()
        {
            Log.Information("Select Service Bus Clouds");
            Items[arr[4]].Select();
        }
        public void SelectSqlServers()
        {
            Log.Information("Select SQL Servers");
            Items[arr[5]].Select();
        }
        public void SelectMySqlServers()
        {
            Log.Information("Select MySQL Servers");
            Items[arr[6]].Select();
        }
        public void SelectAutomation()
        {
            Log.Information("Select Automation");
            Items[arr[7]].Select();
        }
        public void SelectTeamAccessControl()
        {
            Log.Information("Select Team Access Control");
            Items[arr[8]].Select();
        }
        public void SelectPlans()
        {
            Log.Information("Select Plans");
            Items[arr[9]].Select();
        }
        public void SelectUserAccounts()
        {
            Log.Information("Select User Accounts");
            Items[arr[10]].Select();
        }


    }
}
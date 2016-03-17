// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlScrollBarMenu_Tenant.cs" company="Microsoft Corporation">
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
    public class HtmlScrollBarMenu_Tenant : HtmlControl
    {
        private Page page;
        private string[] arr = { "ALL ITEMS", "AZURE VMS", "MY ACCOUNT" };

        /// <summary>
        /// Construct HtmlSection found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlScrollBarMenu_Tenant(Page page, By by)
            : base(page, by)
        {
            this.page = page;
        }

        /// <summary>
        /// Construct HtmlSection by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlScrollBarMenu_Tenant(Page page, IWebElement element)
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
                //var sectionContainers = this.Element.FindElements(By.CssSelector(".fx-scrollbar-scrollable .fxs-menu-tablediv"));
                var sectionContainers = this.Element.FindElements(By.CssSelector(".fxshell-nav1-item"));


                for (int i = 0; i < arr.Length; i++)
                {   // For Admin Portal, there are 3 items in scrollbarmenu - ALL ITEMS, AZURE VMS, MY ACCOUNT.
                    if (sectionContainers[i].Displayed)
                    {
                        var item = new HtmlScrollBarMenuItem(this.page, sectionContainers[i]);
                        sectionItems.Add(arr[i], new HtmlScrollBarMenuItem(this.page, sectionContainers[i]));
                    }
                }

                return sectionItems;
            }
        }

        //public Dictionary<string, HtmlScrollBarMenuItem> GetItems()
        //{
        //        Dictionary<string, HtmlScrollBarMenuItem> sectionItems = new Dictionary<string, HtmlScrollBarMenuItem>();
        //        //var sectionContainers = this.Element.FindElements(By.CssSelector(".fx-scrollbar-scrollable .fxs-menu-tablediv"));
        //        var sectionContainers = this.Element.FindElements(By.CssSelector(".fxshell-nav1-item"));
        //        if (sectionContainers == null)
        //        {
        //            System.Windows.Forms.MessageBox.Show("empty!!");
        //            Log.Warning("empty!!");
        //        }
        //        else
        //        { 
        //            System.Windows.Forms.MessageBox.Show(sectionContainers.Count.ToString());
        //            Log.Warning(sectionContainers.Count.ToString());
        //        }

        //        return sectionItems;
        //}

        public void SelectAllItems()
        {
            Log.Information("Select All Items");
            Items[arr[0]].Select();
            System.Threading.Thread.Sleep(3000);
        }

        public void SelectAzureVms()
        {
            Log.Information("Select Azure Vms");
            Items[arr[1]].Select();
            System.Threading.Thread.Sleep(3000);
        }

        public void SelectMyAccount()
        {
            Log.Information("Select My Account");
            Items[arr[2]].Select();
            System.Threading.Thread.Sleep(3000);
        }

    }
}
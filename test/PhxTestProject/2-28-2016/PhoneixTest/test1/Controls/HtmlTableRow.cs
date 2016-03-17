// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTableRow.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html tag structure like following: Need to use xpath get all elements like: td[@data-jsv]
//  <tr class="odd " data-link="" ...>
//    <td class="waz-grid-navigation">
//        <a href="...">Server001</a>
//    </td>
//    <td class="" data-link="" role="gridcell" data-jsv="/213_/251^/212_#214_#252^#215_">Exception</td>                        // column 2
//    <td class="" data-link="" role="gridcell" data-jsv="/213_/251^/212_#214_#252^#215_">Error in CheckCheckVmCreation()</td>  // column 3
//    <td class="" data-link="" role="gridcell" data-jsv="/213_/251^/212_#214_#252^#215_">Central US</td>                       // column 4
//    <td class="" data-link="" role="gridcell" data-jsv="/213_/251^/212_#214_#252^#215_">Basic_A0</td>                         // column 5
//  </tr>
// --------------------------------------------------------------------------------------------------------------------
namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section-item > .section-item-label ~ .section-item-editor.
    /// </summary>
    public class HtmlTableRow
    {
        private Page page;

        /// <summary>
        /// A div.section-item html element which contains current section items.
        /// </summary>
        internal IWebElement itemContainer;

        /// <summary>
        /// Construct HtmlSectionItem by a container element.
        /// </summary>
        /// <param name="itemContainer">The each section items for the container elemnt to get.</param>
        public HtmlTableRow(Page page, IWebElement itemContainer)
        {
            this.page = page;
            this.itemContainer = itemContainer;
        }

        /// <summary>

        public void Select()
        {
            this.Status.Click();
        }

        public string Name
        {
            get { return this.Server.Text; }
        }
        public HtmlLink Server
        {
            get { return new HtmlLink(this.page, this.itemContainer.FindElement(By.ClassName("waz-grid-navigation"))); }
        }

        public ReadOnlyCollection<IWebElement> bindingProperties
        {
            get 
            {
                var elements = this.itemContainer.FindElements(By.XPath("/td[@data-jsv]"));
                if (elements.Count == 4)
                    return elements;
                else
                    return null;
            }
        }

        public HtmlTextBox Status
        {
            get { return new HtmlTextBox(this.page, this.bindingProperties[0]); }
        }

        public HtmlTextBox Detail
        {
            get { return new HtmlTextBox(this.page, this.bindingProperties[1]); }
        }

        public void SelectStatusColumn()
        {
        }


        /////// <summary>
        /////// Gets the item's Value text.
        /////// </summary>
        ////public string ValueText
        ////{
        ////    get
        ////    {
        ////        return this.itemContainer.FindElement(By.ClassName("section-item-editor")).Text;
        ////    }
        ////}

        /////// <summary>
        /////// Gets the item's Help link button, not all situations will contains a edit link, so may be you should verify is it exist before you using it.
        /////// </summary>
        ////public HtmlLink HelpButton
        ////{
        ////    get
        ////    {
        ////        return new HtmlLink(this.page, itemContainer.FindElement(By.ClassName("helpbutton")));
        ////    }
        ////}

        ////public IWebElement RequiredFlag
        ////{
        ////    get { return itemContainer.FindElement(By.ClassName("ReqdFlag")); }
        ////}
    }

}

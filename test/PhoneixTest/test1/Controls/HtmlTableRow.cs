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
    using Phoenix.Test.UI.Framework.Logging;

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

        public void SelectAndCheckDatails()
        {
            this.Server.Click();
            System.Threading.Thread.Sleep(5000);
        }



        public ReadOnlyCollection<IWebElement> bindingProperties
        {
            get 
            {
                Log.Information("Get TableRow's bindingProperties...");
                var elements = this.itemContainer.FindElements(By.CssSelector("td[role='gridcell']"));
                return elements;
            }
        }

        public string Name
        {
            get {
                Log.Information("Get TableRow's Server Text...");
                return this.Server.Text; }
        }
        public HtmlButton Server
        {   

            // get { return new HtmlLink(this.page, this.itemContainer.FindElement(By.ClassName("waz-grid-navigation"))); } //class name should not use .
            get {
                Log.Information("Get TableRow's Server...");
                return new HtmlButton(this.page, this.bindingProperties[0]); } //class name should not use .
        }

        public HtmlButton Status
        {
            get {
                Log.Information("Get TableRow's Server Status...");
                return new HtmlButton(this.page, this.bindingProperties[1]); }
        }

        public void SelectStatusColumn()
        {
        }

    }

}

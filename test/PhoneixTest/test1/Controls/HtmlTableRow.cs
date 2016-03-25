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
    using System.Linq;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;
    using Phoenix.Test.UI.Framework.Logging;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section-item > .section-item-label ~ .section-item-editor.
    /// </summary>
    public class HtmlTableRow
    {
        private HtmlTable table;

        private Page page;

        private IWebElement[] columns;

        /// <summary>
        /// A div.section-item html element which contains current section items.
        /// </summary>
        internal IWebElement itemContainer;

        /// <summary>
        /// Construct HtmlSectionItem by a container element.
        /// </summary>
        /// <param name="itemContainer">The each section items for the container elemnt to get.</param>
        public HtmlTableRow(Page page, HtmlTable table, IWebElement itemContainer)
        {
            this.table = table;
            this.page = page;
            this.itemContainer = itemContainer;
            this.columns = this.itemContainer.FindElements(By.XPath(".//td[@role='gridcell']")).ToArray();
        }

        public HtmlControl GetColumn(string header)
        {
            return new HtmlControl(this.page, this.columns.ElementAt(this.findHeaderIndex(header)));
        }

        public string Name { get { return this.columns[0].Text; } }

        public void Select()
        {
            new HtmlButton(page, this.columns[1]).Click();
        }

        public void SelectAndCheckDatails()
        {
           // new HtmlButton(page, this.columns[0]).Click();
            new HtmlButton(page, this.columns[0].FindElement(By.XPath("./a"))).Click();
        }

        private int findHeaderIndex(string header)
        {
            var headers = this.table.Headers;
            if (header == null || header.Length == 0)
            {
                throw new NotFoundException("Headers not found!");
            }
            int index = -1;
            while (++index < headers.Length)
            {
                if (headers[index].Equals(header))
                {
                    return index;
                }
            }
            throw new NotFoundException("Header not found!");
        }

    }
}

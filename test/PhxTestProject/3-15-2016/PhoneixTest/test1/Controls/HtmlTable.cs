// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTable.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html tag structure like this: 
//
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using System.Collections.Generic;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Logging;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section > .section-title ~ .section-body > .section-item.
    /// </summary>
    public class HtmlTable : HtmlControl
    {
        private Page page;

        public Dictionary<string, string[]> RowValues = new Dictionary<string,string[]>();

        /// <summary>
        /// Construct HtmlSection found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlTable(Page page, By by)
            : base(page, by)
        {
            this.page = page;
        }

        /// <summary>
        /// Construct HtmlSection by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlTable(Page page, IWebElement element)
            : base(page, element)
        {
            this.page = page;
        }

        /// <summary>
        /// Gets all section items as a Dictionary.
        /// </summary>
        public Dictionary<string, HtmlTableRow> Rows
        {
            get
            {
                var sectionItems = new Dictionary<string, HtmlTableRow>();
                var sectionContainers1 = this.Element.FindElements(By.ClassName("even"));
                var sectionContainers2 = this.Element.FindElements(By.ClassName("odd"));

                Log.Information("Find Even Rows: " + sectionContainers1.Count + " item(s).");
                Log.Information("Find Odd Rows: " + sectionContainers2.Count + " item(s).");
                for (int i = 0; i < sectionContainers1.Count; i++)
                {
                    if (sectionContainers1[i].Displayed)
                    {
                        Log.Information("Create Table Rows for Even and add them to dictionary...");
                        var inlineSectionItem = new HtmlTableRow(this.page, sectionContainers1[i]);
                        if (!sectionItems.ContainsKey(inlineSectionItem.Name))
                        {
                            sectionItems.Add(inlineSectionItem.Name, inlineSectionItem);

                            //var elements = inlineSectionItem.itemContainer.FindElements(By.CssSelector("td[role='gridcell']"));
                            var elements = inlineSectionItem.itemContainer.FindElements(By.CssSelector("td"));
                            Log.Information("Even rows...");
                            Log.Information("Count number: " + elements.Count.ToString());
                            var values = new string[elements.Count-1];
                            for (int j = 1; j < elements.Count; j++)
                            {
                                Log.Information("Item" + j + ": " + elements[j].Text);
                                values[j-1] = elements[j].Text;
                            }
                            RowValues.Add(inlineSectionItem.Name, values);
                        }

                    }
                    if (i < sectionContainers2.Count && sectionContainers2[i].Displayed)
                    {
                        Log.Information("Create Table Rows for Odd and add them to dictionary...");
                        var inlineSectionItem = new HtmlTableRow(this.page, sectionContainers2[i]);
                        if (!sectionItems.ContainsKey(inlineSectionItem.Name))
                        {
                            sectionItems.Add(inlineSectionItem.Name, inlineSectionItem);

                            //var elements = inlineSectionItem.itemContainer.FindElements(By.CssSelector("td[role='gridcell']"));
                            var elements = inlineSectionItem.itemContainer.FindElements(By.CssSelector("td"));
                            Log.Information("Odd rows...");
                            Log.Information("Count number: " + elements.Count.ToString());
                            var values = new string[elements.Count - 1];
                            for (int j = 1; j < elements.Count; j++)
                            {
                                Log.Information("Item" + j + ": " + elements[j].Text);
                                values[j - 1] = elements[j].Text;
                            }
                            RowValues.Add(inlineSectionItem.Name, values);
                        }
                    }
                }

                return sectionItems;
            }
        }

    }
}
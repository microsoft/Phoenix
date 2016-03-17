// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlSectionItem.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html tag structure like this: 
//          <ul class="fx-scrollbar-content">
//              <il data-bind="attr: { "aria-selected": selected() ? "true" "false", ...}
//                  <div class = "fxs-menu-tablediv">
//                      <a tabindex="0"> databind="click: function() {if ...}" href="#"></a>
//                  </div>
//              </il>
//              <il data-bind="attr: { "aria-selected": selected() ? "true" "false", ...}
//                  <div class = "fxs-menu-tablediv">
//                      <a tabindex="0"> databind="click: function() {if ...}" href="#"></a>
//                  </div>
//              </il>
//          </ul>
//
//                  <div class = "fxs-menu-tablediv">
//                      <a tabindex="0"> databind="click: function() {if ...}" href="#"></a>
//                          <div class="icon FileShares" data-bind="attr: {}"> // the icons are different...
//                              <img data-bind="" src="" alt=""></img>
//                          </div>
//                          <div class="fxs-menu-text" data-bind="css: {...}">
//                              <div class="fxs-menu-itemtext" data-bind="text: text"></div>
//                          </div>
//                      </a>       
//                  </div>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section-item > .section-item-label ~ .section-item-editor.
    /// </summary>
    public class HtmlScrollBarMenuItem
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
        public HtmlScrollBarMenuItem(Page page, IWebElement itemContainer)
        {
            this.page = page;
            this.itemContainer = itemContainer;
        }

        /// <summary>
        /// Gets the item's Label text.
        /// </summary>
        //public string LabelText
        //{
        //    get
        //    {
        //        return this.itemContainer.FindElement(By.ClassName("fxs-menu-itemtext")).Text;
        //    }
        //}

        //private HtmlButton InnerButton
        //{
        //    get { return new HtmlButton(this.page, By.ClassName("icon FileShares")); }
        //}

        public void Select()
        {
            Log.Information("Select item: " + this.itemContainer.Text);
            this.itemContainer.Click();
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

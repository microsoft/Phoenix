namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

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
        /// Gets the item's Label text.
        /// </summary>
        public string Name
        {
            get
            {
                //return "s";
                var item = this.itemContainer.FindElement(By.ClassName("waz-grid-navigation"));
                return item.Text; //.ClassName("waz-grid-navigation")
            }
        }

        //private HtmlButton InnerButton
        //{
        //    get { return new HtmlButton(this.page, By.ClassName("icon FileShares")); }
        //}

        public void Select()
        {
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

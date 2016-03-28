// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlButton.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html Controls.
// </summary>

namespace Phoenix.Test.UI.Framework.Controls
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.WebPages;
    using System.Collections.ObjectModel;
    using System.Drawing;
    public class HtmlControl : ElementContainer
    {
        private readonly IWebElement element;

        /// <summary>
        /// Gets or sets the By accessor that defines the HtmlControl element.
        /// </summary>
        protected By By { get; private set; }

        /// <summary>
        /// Get or sets the page where the HtmlControl is located.
        /// </summary>
        protected Page Page { get; private set; }

        /// <summary>
        /// Gets the WebElement representing the HtmlControl.
        /// </summary>
        protected internal IWebElement Element
        {
            get
            {
                if (element == null)
                {
                    return Page.SearchContext.FindElement(By);
                }
                else
                {
                    return element;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this control is displayed.
        /// </summary>
        public virtual bool Displayed
        {
            get { return Element.Displayed; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        public virtual bool Enabled
        {
            get { return Element.Enabled; }
        }

        /// <summary>
        /// Gets a value indicating whether the control exists on the DOM.
        /// </summary>
        //public virtual bool Exists
        //{
        //    get { return ExpectedConditionsExt.ElementExists(By)(Page.Browser) != null; }
        //}

        /// <summary>
        /// Gets a <see cref="T:System.Drawing.Point"/> object containing the coordinates of the upper-left corner of this HtmlControl relative to the upper-left corner of the page.
        /// </summary>
        public virtual Point Location
        {
            get { return Element.Location; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this HtmlControl is selected.
        /// </summary>
        public virtual bool Selected
        {
            get { return Element.Selected; }
        }

        /// <summary>
        /// Gets a <see cref="Size"/> object containing the height and width of this HtmlControl.
        /// </summary>
        //public virtual Size Size
        //{

        //    get { return Element.Size; }
        //}

        /// <summary>
        /// Gets the tag name of this HtmlControl.
        /// </summary>
        public virtual string TagName
        {
            get { return Element.TagName; }
        }

        /// <summary>
        /// Gets the innerText of this HtmlControl, without any leading or trailing whitespace, and with other whitespace collapsed.
        /// </summary>
        public virtual string Text
        {
            get { return Element.Text; }
        }

        /// <summary>
        /// Gets the value stored by the control.
        /// </summary>
        //public virtual string Value
        //{
        //    get { return Element.GetAttribute(TestConst.Control.VALUE); }
        //}

        /// <summary>
        /// Finds the IWebElement children of this HtmlControl using a By accessor.
        /// </summary>
        /// <param name="by">By accessor determining which children are returned</param>
        /// <returns>IWebElement children of this HtmlControl matching the By accessor</returns>
        public virtual ReadOnlyCollection<IWebElement> GetChildren(By by)
        {
            return Element.FindElements(by);
        }

        /// <summary>
        /// Finds the IWebElement parent
        /// </summary>
        /// <returns>IWebElement parent</returns>
        public virtual IWebElement GetParent()
        {
            return Element.FindElement(By.XPath(".."));
        }

        /// <summary>
        /// Outputs By if available, otherwise Element.
        /// </summary>
        /// <returns>String representation of control</returns>
        public override string ToString()
        {
            return By != null ? By.ToString() : Element.ToString();
        }

        /// <summary>
        /// Construct HtmlControl found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found</param>
        /// <param name="by">By accessor to find element</param>
        public HtmlControl(Page page, By by)
            : base(page.Browser, by)
        {
            By = by;
            Page = page;
            InitElements(page);
        }

        /// <summary>
        /// Construct HtmlControl by directly passing its element.
        /// Only use this constructor when By accessor is not available.
        /// </summary>
        /// <param name="page">Page where control is found</param>
        /// <param name="element">IWebElement representing the control</param>
        public HtmlControl(Page page, IWebElement element)
            : base(element)
        {
            this.element = element;
            Page = page;
            InitElements(page);
        }

        /// <summary>
        /// Verify whether the HtmlControl enabled during a timeout setting.
        /// </summary>
        /// <param name="timeOut">Set milliseconds timeout value for wait the HtmlControl to enabled. Default value is set by App.config, TestSettings section group, pageTimeOut property value.</param>
        /// <param name="sleepInterval">Set milliseconds timeout value indicating how often to check for the condition to be true. Default is 500 milliseconds.</param>
        /// <returns>Whether the HtmlControl enabled.</returns>
        public bool IsEnabled(int timeOut = 0, int sleepInterval = 0)
        {
            return Page.Browser.Wait((driver) => Element.Enabled,
                timeOut,
                sleepInterval,
                throwErrorIfFalse: false);
        }

        /// <summary>
        /// Verify whether the HtmlControl is visible during a timeout setting.
        /// </summary>
        /// <param name="timeOut">Set milliseconds timeout value for wait the HtmlControl to enabled. Default value is set by App.config, TestSettings section group, pageTimeOut property value.</param>
        /// <param name="sleepInterval">Set milliseconds timeout value indicating how often to check for the condition to be true. Default is 500 milliseconds.</param>
        /// <returns>Whether the HtmlControl is visible.</returns>
        public bool IsVisible(int timeOut = 0, int sleepInterval = 0)
        {
            return Page.Browser.Wait((driver) => Element.Displayed,
                timeOut,
                sleepInterval,
                throwErrorIfFalse: false);
        }

 

      
        /// <summary>
        /// Wait the HtmlControl to enabled during a timeout setting.
        /// </summary>
        /// <param name="timeOut">Set milliseconds timeout value for wait the HtmlControl to enabled. Default value is set by App.config, TestSettings section group, pageTimeOut property value.</param>
        /// <param name="sleepInterval">Set milliseconds timeout value indicating how often to check for the condition to be true. Default is 500 milliseconds.</param>
        public void WaitEnable(int timeOut = 0, int sleepInterval = 0)
        {
            Page.Browser.Wait((driver) =>
            {
                return Element.Enabled;
            },
            timeOut,
            sleepInterval,
                throwErrorMessage: "Wait element enable failed! The element info: " + ToString());
        }

        ///// <summary>
        ///// Wait the HtmlControl to disabled during a timeout setting.
        ///// </summary>
        ///// <param name="timeOut">Set milliseconds timeout value for wait the HtmlControl to disabled. Default value is set by App.config, TestSettings section group, pageTimeOut property value.</param>
        ///// <param name="sleepInterval">Set milliseconds timeout value indicating how often to check for the condition to be true. Default is 500 milliseconds.</param>
        public void WaitDisable(int timeOut = 0, int sleepInterval = 0)
        {
            Page.Browser.Wait((driver) =>
            {
                return !Element.Enabled;
            },
            timeOut,
            sleepInterval,
                throwErrorMessage: "Wait element disable failed! The element info: " + ToString());

        }

        /// <summary>
        /// Waits until the HtmlControl is with text or timeout is reached.
        /// This can help monitor ajax call created div
        /// </summary>
        /// <param name="timeout">How long to wait for HtmlControl to be disabled (ms). Default value is in App.config</param>
        /// <param name="sleepInterval">How often to check for HtmlControl to be disabled (ms). Default is 500ms.</param>
        public void WaitUntilWithText(int timeout = 0, int sleepInterval = 0)
        {
            //Log.Information("Waiting for element ({0}) to be with text.", this.ToString());
            Page.Browser.Wait(
                (driver) => !string.IsNullOrEmpty(driver.FindElement(By).Text),
                timeout,
                sleepInterval);
        }

        /// <summary>
        /// Waits until the HtmlControl text  changes from currentText or timeout is reached.
        /// This can help monitor ajax call updated div
        /// </summary>
        /// <param name="currentText">CurrentText and expteced to change</param>
        /// <param name="timeout">How long to wait for HtmlControl to be disabled (ms). Default value is in App.config</param>
        /// <param name="sleepInterval">How often to check for HtmlControl to be disabled (ms). Default is 500ms.</param>
        public void WaitUntilTextChange(string currentText, int timeout = 0, int sleepInterval = 0)
        {
            //Log.Information("Waiting for element ({0}) to change text from ({1}).", this.ToString(), currentText);
            Page.Browser.Wait(
                (driver) => driver.FindElement(By).Text != currentText,
                timeout,
                sleepInterval);
        }

        #region Methods

        /// <summary>
        /// Clear the control.
        /// </summary>
        public virtual void Clear()
        {
            Element.Clear();
        }

        /// <summary>
        /// Click the control with option to verify the blocker.
        /// </summary>
        /// <param name="verifyBlocker">Whether to verify the blocker (default don't verify)</param>
        public virtual void Click(bool verifyBlocker = false)
        {
            try
            {
                if (Location.Y > Page.GetHeight())
                {
                    Page.Browser.Manage().Window.Maximize();
                }

                //Log.Information("Click element: {0}", this.ToString());
                //this.Element.Click();
                this.ExcuteScriptOnElement(".click();");
                //if (verifyBlocker)
                //{
                //    Page.VerifyBlocker();
                //}
            }
            catch (ElementNotVisibleException exception)
            {
                var message = string.Format("Element {0}", By);
                throw new ElementNotVisibleException(message, exception);
            }
        }

        public virtual void ClickButtonToNavigate()
        {
            this.Click();
            System.Threading.Thread.Sleep(10000);
        }

        public virtual void StripClass()
        {
            ExcuteScriptOnElement(".removeAttribute('class')");
        }

        public void ExcuteScriptOnElement(string script)
        {
            ((IJavaScriptExecutor)Page.Browser).ExecuteScript("arguments[0]" + script, Element);
        }

        /// <summary>
        /// Get an attribute of the control by name.
        /// </summary>
        /// <param name="attributeName">Name of attribute to get</param>
        /// <returns>Value of the attribute</returns>
        public virtual string GetAttribute(string attributeName)
        {
            return Element.GetAttribute(attributeName);
        }

        /// <summary>
        /// Get a CSS value of the control by property name.
        /// </summary>
        /// <param name="propertyName">Name of property to get</param>
        /// <returns>Value of CSS property</returns>
        public virtual string GetCssValue(string propertyName)
        {
            return Element.GetCssValue(propertyName);
        }

        /// <summary>
        /// Send keys to the control.
        /// </summary>
        /// <param name="text">Keys to send</param>
        public virtual void SendKeys(string text)
        {
            WaitEnable(WebDriverExtensions.TimeOut_3Seconds, WebDriverExtensions.TimeOut_ShortInterval);
            Element.SendKeys(text);
            System.Threading.Thread.Sleep(200);
        }

        /// <summary>
        /// Submit the control.
        /// </summary>
        public virtual void Submit()
        {
            WaitEnable(WebDriverExtensions.TimeOut_3Seconds, WebDriverExtensions.TimeOut_ShortInterval);
            Element.Submit();
            System.Threading.Thread.Sleep(200);
        }

        /// <summary>
        /// Set the control text (clears current text), will skip if the text is null or equals current value. Option to use javascript.
        /// </summary>
        /// <param name="inputText">Text to set in control.</param>
        /// <param name="useScroll">Default parameter allows for not invoking ScrollTo(): For 'Inline Edit' TC's</param>
        /// <param name="clickAndClearElement"></param>
        public virtual void SetText(string inputText, bool useScroll = true, bool clickAndClearElement = true)
        {
            //if (useScroll)
            //{
            //    Page.Browser.ScrollTo(this);
            //}
            this.WaitEnable();

            if (inputText != null)
            {
                //Log.Information("Input text: {0} for {1}", inputText, this.ToString());
                if (clickAndClearElement)
                {
                    this.Element.Click();
                    this.Element.Clear();
                }

                this.Element.SendKeys(inputText);
            }
        }

        #endregion




    }
    
}

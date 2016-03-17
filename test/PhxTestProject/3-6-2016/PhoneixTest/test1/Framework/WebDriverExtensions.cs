using System.IO;
//using System.Runtime.Serialization.Json;
using System.Text;

namespace Phoenix.Test.UI.Framework
{
    //using Phoenix.Test.UI.Framework.Configuration;
    //using Phoenix.Test.UI.Framework.Helper;
    using Controls;
    //using Logging;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using WebPages;

    public static class WebDriverExtensions
    {
        // Time Out values for calls to 
        // IsExist(TotalTimeToSearch, IntervalBetweenSearchAttempts)
        // and WaitLoad(TotalTimeToSearch, IntervalBetweenSearchAttempts)
        public const int TimeOut_120Seconds = 120000;
        public const int TimeOut_10Seconds = 10000;
        public const int TimeOut_5Seconds = 5000;
        public const int TimeOut_3Seconds = 3000;
        public const int TimeOut_2Seconds = 2000;
        public const int TimeOut_ShortInterval = 500;
        public const int TimeOut_DoubleInterval = 1000;
        // Sometimes UI takes a long time to render during first page load, this adds some extra wait time for the test
        public const int TimeOut_Warmup_Extra_Wait_90Seconds = 90000;
        static volatile bool _firstPageWait = true;

        public static int PageTimeout = 5000;

        /// <summary>
        /// Scroll to bottom of document.
        /// </summary>
        public static void ScrollBottom(this IWebDriver driver)
        {
            ScrollTo(driver, "0", "Math.max(document.documentElement.scrollHeight,document.body.scrollHeight,document.documentElement.clientHeight)");
        }

        /// <summary>
        /// Scroll to top of document.
        /// </summary>
        public static void ScrollTop(this IWebDriver driver)
        {
            ScrollTo(driver, "0", "Math.min(document.documentElement.scrollHeight,document.body.scrollHeight,document.documentElement.clientHeight)");
        }

        /// <summary>
        /// Scroll to element's location point.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="element">element to which to scroll</param>
        public static void ScrollTo(this IWebDriver driver, HtmlControl element)
        {
            ScrollTo(driver, element.Location.X, element.Location.Y);
        }

        /// <summary>
        /// Scroll to element's location point in a IFrame page.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="element">element to which to scroll</param>
        /// <param name="iframeName">IFrame name or id in page which contains the element.</param>
        public static void ScrollTo(this IWebDriver driver, HtmlControl element, string iframeName)
        {
            driver.SwitchTo().Frame(iframeName);
            ScrollTo(driver, element.Location.X, element.Location.Y);
            driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// Scroll to element's location by Javascript's scrollIntoView.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="element">element to which to scroll</param>
        public static void ScrollIntoView(this IWebDriver driver, HtmlControl element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element.Element);
        }

        /// <summary>
        /// Scroll to a point in the window.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="x">x coordinate in window</param>
        /// <param name="y">y coordinate in window</param>
        public static void ScrollTo(this IWebDriver driver, int x, int y)
        {
            ScrollTo(driver, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture));
        }

        private static void ScrollTo(this IWebDriver driver, string x, string y)
        {
            //Log.Information("Scroll point to: [{0}, {1}]", x, y);
            driver.ExecuteJavaScript(string.Format("window.scrollTo({0},{1});", x, y));
        }

        public static void CenterOnScreen(this IWebDriver driver, HtmlControl element)
        {
            CenterOnScreen(driver, element.Location.X, element.Location.Y);
        }

        /// <summary>
        /// Scroll item at x, y to the center of the screen
        /// For use in TC's that generate the inline edit icon
        /// </summary>
        /// <param name="driver">The browser driver</param>
        /// <param name="x">x coordinate in window</param>
        /// <param name="y">y coordinate in window</param>
        public static void CenterOnScreen(this IWebDriver driver, int x, int y)
        {
            var clientWidthObj = ((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.clientWidth;");
            var clientHeightObj = ((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.clientHeight;");

            if (clientHeightObj == null || clientWidthObj == null)
            {
                //Log.Warning("Get current browser's client width: {0}, height: {1}",
                    //clientHeightObj != null ? clientHeightObj.ToString() : "null",
                    //clientWidthObj != null ? clientWidthObj.ToString() : "null");

                ScrollTo(driver, x, y);
            }
            else
            {
                //Log.Information("Get current browser's client width: {0}, height: {1}", clientHeightObj, clientWidthObj);
                int clientWidth = Convert.ToInt32(clientWidthObj);
                int clientHeight = Convert.ToInt32(clientHeightObj);

                ScrollTo(driver, Math.Max(0, x - clientWidth / 2), Math.Max(0, y - clientHeight / 2));
            }
        }

        internal static void ExecuteJavaScript(this IWebDriver driver, string script, params object[] args)
        {
            var javaScriptExecutor = driver as IJavaScriptExecutor;
            if (javaScriptExecutor == null)
                throw new WebDriverException("Driver does not implement IJavaScriptExecutor");
            javaScriptExecutor.ExecuteScript(script, args);
        }

        public static void SwitchToFrame(this IWebDriver driver, string name)
        {
            driver.SwitchTo().Frame(name);
        }

        public static void SwitchToDefault(this IWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
        }

        public static Uri GetUri(this IWebDriver driver)
        {
            return new Uri(driver.Url);
        }


        // TODO: mae a more common api
        public static bool WaitForSuccessOrFailure(this IWebDriver driver, Func<bool> successCondition, Func<bool> errorCondition)
        {
            //var timeout = TimeSpan.FromMilliseconds(Env.PageTimeout);
            var timeout = TimeSpan.FromMilliseconds(PageTimeout);
            var watch = Stopwatch.StartNew();

            while (true)
            {
                if (errorCondition())
                {
                    return false;
                }
                
                if (successCondition())
                {
                    return true;
                }

                if (watch.Elapsed >= timeout)
                {
                    //Log.Error("Waiting for conditions exceeded timeout. Url in browser was: {0}", driver.Url);
                    throw new WebDriverTimeoutException("Waiting for conditions exceeded timeout.");
                }

                Thread.Sleep(100);
            }
        }

        public static void WaitForConditions(this IWebDriver driver, Func<bool>[] conditions)
        {
            var timeout = TimeSpan.FromMilliseconds(PageTimeout);
            
            var watch = Stopwatch.StartNew();

            while (true)
            {
                if (conditions.Any(condition => condition()))
                {
                    return;
                }

                if (watch.Elapsed >= timeout)
                {
                    //Log.Error("Waiting for conditions exceeded timeout. Url in browser was: {0}", driver.Url);
                    throw new WebDriverTimeoutException("Waiting for conditions exceeded timeout.");
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Encapsulate WebDriverWait class, centralized processing wait timeout setting and Exception handling strategy.
        /// </summary>
        /// <param name="driver">WebDriver to use.</param>
        /// <param name="untilCondition">
        /// A delegate taking an object of type T as its parameter, and returning a TResult.
        /// <para>IWebDriver (driver): The obj from WebDriver.</para>
        /// <para>TResult: The delegate's expected return type.</para>
        /// </param>
        /// <param name="timeOut">Set milliseconds timeout value for wait the HtmlControl loaded. Default value is set by App.config, TestSettings section group, pageTimeOut property value.</param>
        /// <param name="sleepInterval">Set milliseconds timeout value indicating how often to check for the condition to be true. Default is 500 milliseconds.</param>
        /// <param name="throwErrorIfFalse">Input a exception handling strategy to indicating whether throw a exception while the action failed.</param>
        /// <param name="throwErrorMessage">If the throwErrorIfFalse is true, then it will throw a error message that the param specified while the action failed.</param>
        /// <returns>The delegate's return value. If the throwErrorIfFalse is false, then it will return false while the action failed, or return true.</returns>
        public static TResult Wait<TResult>(this IWebDriver driver, Func<IWebDriver, TResult> untilCondition,
            int timeOut = 0,
            int sleepInterval = 0,
            bool throwErrorIfFalse = false,
            string throwErrorMessage = "")
        {
            var result = default(TResult);
            WebDriverWait waitApi;

            var timeOutInterval = timeOut == 0 ? PageTimeout : timeOut;
            if (_firstPageWait)
            {
                timeOutInterval += TimeOut_Warmup_Extra_Wait_90Seconds;
                _firstPageWait = false;
            }
            var timeOutIntervalTimeSpan = TimeSpan.FromMilliseconds(timeOutInterval);


            if (sleepInterval != 0)
            {
                waitApi = new WebDriverWait(new SystemClock(), driver, timeOutIntervalTimeSpan, TimeSpan.FromMilliseconds(sleepInterval));
            }
            else
            {
                waitApi = new WebDriverWait(driver, timeOutIntervalTimeSpan);
            }

            try
            {
                result = waitApi.Until(untilCondition);
            }           
            catch (Exception ex)
            {
                //Log.Error(ex, "Exception of type {0} occured while waiting. Url in browser was: {1}", ex.GetType(), driver.Url);
                if (throwErrorIfFalse)
                {
                    if (string.IsNullOrEmpty(throwErrorMessage))
                    {
                        throw;
                    }

                    throw new Exception(throwErrorMessage, ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Wait for all Ajax calls to finish. For iframe, use Browser.SwitchToFrame("IframeHost") before using.
        /// </summary>
        /// <param name="driver">web driver to use</param>
        /// <param name="timeout">max time to wait for Ajax calls to finish (ms), default in App.config</param>
        /// <param name="sleepInterval">polling interval (ms), default 500ms</param>
        /// <returns>whether all Ajax calls are completed before timeout</returns>
        public static bool WaitForAjax(
            this IWebDriver driver,
            int timeout = 10000,
            int sleepInterval = 10000)
        {
            //Log.Information("Waiting for all Ajax calls to finish.");

            return Wait(driver,
                drv => drv.ExecuteJavaScript<bool>("return $.active == 0"),
                timeout,
                sleepInterval,
                true,
                "WaitForAjax failed!");
        }


        public static void Refresh(this IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.KeyDown(Keys.Control).SendKeys(Keys.F5).Perform();
        }

        /// <summary>
        /// Performs a given action that causes an element to refresh, then waits for the element to refresh before continuing.
        /// Default is for the entire page to refresh. For iframe, you may want to use By.Name("IframeHost").
        /// </summary>
        /// <param name="driver">web driver to use</param>
        /// <param name="action">action to perform</param>
        /// <param name="refreshElement">by selector for element to use when checking for refresh, default is entire html document</param>
        /// <param name="timeout">max time to wait for element to refresh (ms), default in App.config</param>
        /// <param name="sleepInterval">polling interval (ms), default 500ms</param>
        /// <returns>whether element refreshed successfully</returns>
        public static bool WaitForRefresh(
            this IWebDriver driver,
            Action action,
            By refreshElement = null,
            int timeout = 0,
            int sleepInterval = 0)
        {
            if (refreshElement == null)
            {
                refreshElement = By.TagName("html");
            }
            //Log.Information("Will wait for this element to refresh: {0}", refreshElement.ToString());

            var element = driver.FindElement(refreshElement);

            action();

            var result = Wait(driver,
                drv =>
                {
                    try
                    {
                        return element.Displayed && false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                    catch (WebDriverTimeoutException)
                    {
                        return false;
                    }
                },
                timeout,
                sleepInterval,
                true,
                 "WaitForRefresh failed!");

            if (result)
            {
                //Log.Information("Element refreshed: {0}", refreshElement.ToString());
            }
            else
            {
                //Log.Information("Element did not refresh: {0}", refreshElement.ToString());
            }
            return result;
        }

        /////// <summary>
        /////// You Javascript command should return JSON string in order to deserialize that result correctly.
        /////// Example of a script: return "{ \"prop\" : \"Hello World!\"}"
        /////// </summary>
        /////// <typeparam name="T"></typeparam>
        /////// <param name="driver"></param>
        /////// <param name="script"></param>
        /////// <returns></returns>
        ////public static T ExecuteJavaScriptAndDeserializeResult<T>(this IWebDriver driver, string script)
        ////{
        ////    var json = driver.ExecuteJavaScript<string>(script);
        ////    var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        ////    var s = new DataContractJsonSerializer(typeof(T));
        ////    return (T)s.ReadObject(stream);
        ////}

        /// <summary>
        /// This method is used to switch to a window if you know the window handle.
        /// </summary>
        /// <param name="handle">Window handle</param>
        public static void SwitchToWindow(this IWebDriver driver, string handle)
        {
            driver.SwitchTo().Window(handle);
        }

        /// <summary>
        /// Switch to a pop up window by clicking on an HtmlControl.
        /// </summary>
        /// <param name="controlToClick">HtmlControl to click to open the pop up window.</param>
        public static void SwitchToPopupWindow(this IWebDriver driver, HtmlControl controlToClick)
        {
            PopupWindowFinder pop = new PopupWindowFinder(driver, TimeSpan.FromSeconds(15));
            driver.SwitchToWindow(pop.Click(controlToClick.Element));
        }

        /// <summary>
        /// Switch to a pop up window by invoking a method.
        /// </summary>
        /// <param name="methodToOpenWindow">Method to invoke for opening the pop up window.</param>
        public static void SwitchToPopupWindow(this IWebDriver driver, Action methodToOpenWindow)
        {
            PopupWindowFinder pop = new PopupWindowFinder(driver, TimeSpan.FromSeconds(15));
            driver.SwitchToWindow(pop.Invoke(methodToOpenWindow));
        }
    }
}

namespace Phoenix.Test.UI.TestCases
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.IE;
    using System;
    using System.Diagnostics;

    [TestClass]
    public abstract class WAPTestBase
    {
        private EventLog _eventLog;
        public InternetExplorerDriver driver;

        protected WAPTestBase()
        {
            _eventLog = new EventLog("Application");
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            this.driver = new InternetExplorerDriver();
            this.driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            this.driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(15));
            this.driver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            this.driver.Quit();
        }
    }
}
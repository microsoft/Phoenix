namespace Phoenix.Test.UI.TestCases
{
    //using Phoenix.Test.UI.Framework.Configuration;
    //using Phoenix.Test.UI.Framework.Extensions;
    using Framework;
    //using Framework.Models;
    using System.Diagnostics;
    //using Microsoft.Test.MaDLybZ;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;

    [TestClass]
    public abstract class WAPTestBase
    {
        private EventLog _eventLog;
        //private FirefoxDriver driver;
        public InternetExplorerDriver driver;

        protected WAPTestBase()
        {
            _eventLog = new EventLog("Application");
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            //this.driver = new FirefoxDriver();
            this.driver = new InternetExplorerDriver();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            this.driver.Close();
        }

    }
}
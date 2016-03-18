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
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using System.Data;
    using System.IO;
    using Phoenix.Test.UI.Framework.Logging;

    [TestClass]
    public abstract class WAPTestBase
    {
        private EventLog _eventLog;
        //private FirefoxDriver driver;
        public RemoteWebDriver driver;
        public DataSet config;

        public string browserName;

        protected WAPTestBase()
        {
            _eventLog = new EventLog("Application");
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            config = new DataSet();
            try
            {
                config.ReadXml(Directory.GetCurrentDirectory() + @"\PhoenixTest.exe.config");
            }
            catch (FileNotFoundException ex)
            {
                Log.Information("Read configure file failed !! " + ex.Message);
            }

            ReadBrowserConfig(config);
            CreateDriver();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            this.driver.Close();
        }

        public void ReadBrowserConfig(DataSet config)
        {
            this.browserName = config.Tables[2].Rows[0].ItemArray[2].ToString().Trim().ToLower();
        }

        public void CreateDriver()
        {
            if (browserName == "firefox")
            {
                this.driver = new FirefoxDriver();
            }
            else if (browserName == "chrome")
            {
                this.driver = new ChromeDriver();
            }
            else if (browserName == "ie" || browserName == "internetexplorer")
            {
                this.driver = new InternetExplorerDriver();
            }
            else
            {
                this.driver = new InternetExplorerDriver();
            }
        }

    }
}
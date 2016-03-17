namespace Phoenix.Test.UI.TestCases
{
    //using Phoenix.Test.UI.Framework.Configuration;
    //using Phoenix.Test.UI.Framework.Extensions;
    using Framework;
    //using Framework.Models;
    using System.Diagnostics;
    //using Microsoft.Test.MaDLybZ;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class WAPTestBase
    {
        private EventLog _eventLog;

        protected WAPTestBase()
        {
            _eventLog = new EventLog("Application");
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
        }

        [TestCleanup]
        public virtual void TestCleanup()
        { }

    }
}
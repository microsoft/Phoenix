namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.Logging;

    public class databasesTest
    {
        private List<appDatabase> Databases = null;
        private bool testPassed;
        private bool testFinished;

        public databasesTest(List<appDatabase> dbServers)
        {
            this.Databases = dbServers;
            this.testPassed = false;
            this.testFinished = false;
        }

        private void Passed()
        {
            this.testPassed = true;
            this.testFinished = true;
        }

        private void Failed()
        {
            this.testPassed = false;
            this.testFinished = true;
        }

        public bool getTestPassed()
        {
            return this.testPassed;
        }

        public bool getTestFinished()
        {
            return this.testFinished;
        }

        public void runDatabasesTest()
        {
            Thread.Sleep(5000);
            //test to see if the DBs can be connected to
            Log.Information("---Testing Database Connections---");

            foreach (appDatabase Database in this.Databases)
            {
                string dbTestResult = "";
                dbTestResult = testRemoteSQLConnection(Database.serverName,Database.databaseName,Database.userid,Database.password);
                if (dbTestResult.Contains("FAILED")) { Failed(); }
                else { Passed(); }
                Log.Information(dbTestResult);
            }
        }

        public static string testRemoteSQLConnection(string serverName,string databaseName,string userAccount, string userPswd)
        {
            string output = "";
            try
            {
                SqlConnection testConnection = new SqlConnection("server=" + serverName + ", 1433; database=" + databaseName + "; user id=" + userAccount + "; password=" + userPswd + "; Integrated Security=False");
                testConnection.Open();
                testConnection.Close();
                output += "SQL Connect Test: Server: " + serverName + "; Database: " + databaseName + " - PASSED";
            }
            catch (Exception ex)
            {
                output += "SQL Connect Test: Server: " + serverName + "; Database: " + databaseName + "; " + ex.Message.ToString() + " - FAILED";

            }
            return output;
        }

    }
}

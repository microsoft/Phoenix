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

    public class resourceProviderTest
    {
        private bool testPassed;
        private bool testFinished;
        private string userAccount;
        private string userPswd;
        private string serverName;

        // The following constructor has parameters for two of the three 
        // properties. 
        public resourceProviderTest(appDatabase TempDB)
        {
            this.userAccount = TempDB.userid;
            this.userPswd = TempDB.password;
            this.serverName = TempDB.serverName;
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

        public void runResourceProviderTest()
        {
            SqlDataAdapter rpAdapter;
            DataTable rpTable;
            string rpQuery = "SELECT InstanceID FROM [Microsoft.MgmtSvc.Store].[mp].[ResourceProviders] WHERE NAME = 'CmpWapExtension' and Enabled = 1";
            string output = "";
            int rpCount = 0;

            Thread.Sleep(5000);

            //test to see if the DB can be connected to
            Log.Information("---Testing Resource Provider ---");

            try
            {
                SqlConnection rpConnection = new SqlConnection("server=" + serverName + ", 1433; database=Microsoft.MgmtSvc.Store; user id=" + userAccount + "; password=" + userPswd + "; Integrated Security=False");
                rpConnection.Open();
                rpAdapter = new SqlDataAdapter(rpQuery, rpConnection);
                rpTable = new DataTable();
                rpCount = rpAdapter.Fill(rpTable);
                if (rpCount != 0)
                {
                    foreach (DataRow row in rpTable.Rows) // Loop over the rows.
                    {
                        foreach (var item in row.ItemArray) // Loop over the items.
                        {
                            output += "Resource Provider instance ID:" + item + " found on " + serverName + " - PASSED";
                            Passed();
                            Log.Information(output);
                        }
                    }

                }
                else
                {
                    output += "No entry for the WAP Resource Provider found on " + serverName + " in Microsoft.MgmtSvc.Store DB in Resource Provider Table - FAILED";
                    Failed();
                    Log.Information(output);
                }
                rpConnection.Close();
            }
            catch (Exception ex)
            {
                output += "RP DB Table Connection: Server: " + serverName + "; Database: Microsoft.Mgmtsvc.Store; " + ex.Message.ToString() + " - FAILED";
                Failed();
                Log.Information(output);
            }
        }
    }
}

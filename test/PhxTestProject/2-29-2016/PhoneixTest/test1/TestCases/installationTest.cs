namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Data.SqlClient;
using Microsoft.Web.Administration;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.Logging;


    public class installationTest
    {
        private bool Passed;
        private string SQLAdminAccount;
        private string SQLAdmPswd;
        private string SQLServer;

        public void Initialize()
        {
            Passed = true;
        }

        private void Failed()
        {
            Passed = false;
        }

        public bool GetPass()
        {
            return Passed;
        }

        //I plan to refactor this into a class where the machines can be added to a list within the class
        //with an ADD method.   Then running the TEST method will run all of them through the test code below
        //in a nice for-loop.  -KH
        public void dbTest(string userAccount, string password, string serverName)
        {
            this.SQLAdminAccount = userAccount;
            this.SQLAdmPswd = password;
            this.SQLServer = serverName;

            //test to see if the DBs can be connected to
            Log.Information("---Testing Database Connections---");

            string cmpdbTestResult = testRemoteSQLConnection("CMP_DB", serverName, userAccount, password);
            if (cmpdbTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(cmpdbTestResult);

            string cmpwapdbTestResult = testRemoteSQLConnection("CMPWAP_DB", serverName, userAccount, password);
            if (cmpwapdbTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(cmpwapdbTestResult);

            string mgmtstoreddbTestResult = testRemoteSQLConnection("Microsoft.MgmtSvc.Store", serverName, userAccount, password);
            if (mgmtstoreddbTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(mgmtstoreddbTestResult);

        }

        //I plan to refactor this into a class where the machines can be added to a list within the class
        //with an ADD method.   Then running the TEST method will run all of them through the test code below
        //in a nice for-loop.- KH
        public void serviceTest(string serverName)
        {
            //test to see if the services are up and running
            Log.Information("---Checking Services Installation and Status---");

            string cmpworkerSvcTestResult = remoteServiceStatus("CmpWorkerService", serverName);
            if (cmpworkerSvcTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(cmpworkerSvcTestResult);

            string w3SvcTestResult = remoteServiceStatus("W3SVC", serverName);
            if (w3SvcTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(w3SvcTestResult);

            string sqlSvcTestResult = remoteServiceStatus("MSSQLSERVER", serverName);
            if (sqlSvcTestResult.Contains("FAILED")) { Failed(); }
            Log.Information(sqlSvcTestResult);

        }


        public void siteTest(string serverName)
        {
            //test to see if the websites are up and running
            Log.Information("---Checking Web Applications Status---");
            string siteTestResults = readSiteData(serverName);
            if (siteTestResults.Contains("FAILED")) { Failed(); }
            Log.Information(siteTestResults);
        }

        public static string testRemoteSQLConnection(string databaseName, string serverName, string userName, string password)
        {
            string output = "";
            try
            {
                SqlConnection testConnection = new SqlConnection("server=" + serverName + ", 1433; database=" + databaseName + "; user id=" + userName + "; password=" + password + "; Integrated Security=True");
                testConnection.Open();
                testConnection.Close();
                output += "SQL Connect Test: Server: " + serverName + "; Database: " + databaseName + " - PASSED";
            }
            catch (Exception ex)
            {
                output += "SQL Connect Test: Server: " + serverName + "; Database: " + databaseName + "; " + ex.Message + " - FAILED";
            }
            return output;
        }

        public static string remoteServiceStatus(string serviceName, string serverName)
        {
            string output = "";
            try
            {
                // get list of Windows services on the remote machine
                ServiceController[] services = ServiceController.GetServices(serverName);

                // try to find service name in the list of services
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName)
                    {
                        output = serviceName + ": Installed on " + serverName;
                        if (service.Status.ToString() == "Running")
                        {
                            // add this status if it is running
                            output += ", Status: Running - PASSED";
                        }
                        else
                        {
                            // ad this status if it is in some other state
                            output += ", Status: " + service.Status.ToString() + " - FAILED";
                        }
                        return output;
                    }
                }
                // if the server can be connected to but the service isn't installed it will reach this point
                return serviceName + "is NOT installed on " + serverName + " - FAILED";
            }
            catch
            {
                // return this is an exception occurs because the server cannot be accessed or found
                return "Cannot access or find the server: " + serverName + " - FAILED";
            }
        }

        //this function will only run locally and only if run-as admin
        //future plans is to also split this apart into a class with a list of servers and ADD and TEST method
        public static string readSiteData(string serverName)
        {
            string siteOutput = "";
            try
            {
                ServerManager IIS = new ServerManager();
                //siteOutput = "      PASSED";
                foreach (Site site in IIS.Sites)
                {
                    siteOutput += site.Name + ", ";
                    siteOutput += site.State + ", ";
                    foreach (Binding bindingElement in site.Bindings)
                    {
                        siteOutput += bindingElement.Protocol + ", ";
                        string parseport = bindingElement.EndPoint + "";
                        int portpos = parseport.IndexOf(":");
                        siteOutput += parseport.Substring(portpos + 1) + ", ";
                    }
                    siteOutput += site.Applications["/"].VirtualDirectories[0].PhysicalPath + " - PASSED";
                    Log.Information(siteOutput);
                    siteOutput = "";
                }
            }
            catch
            {
                siteOutput = serverName + " could NOT be accessed - FAILED";
            }
            return siteOutput;
        }
    }
}

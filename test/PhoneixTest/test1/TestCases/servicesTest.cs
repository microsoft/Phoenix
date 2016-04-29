namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Threading;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.Logging;

    public class servicesTest
    {
        private List<string> Services = new List<string>();
        private bool testPassed;
        private bool testFinished;
        private string serverName;

        public servicesTest(string server)
        {
            this.serverName = server;
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

        public void AddService(string serviceName)
        {
            Services.Add(serviceName);
        }

        public void runServicesTest()
        {
            Thread.Sleep(5000);
            //test to see if the services are up and running
            Log.Information("---Checking Services Installation and Status---");

            foreach (string Service in this.Services)
            {
                string svcTestResult = "";
                svcTestResult = remoteServiceStatus(Service, serverName);
                if (svcTestResult.Contains("FAILED")) { Failed(); }
                else { Passed(); }
                Log.Information(svcTestResult);
            }
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
                return serviceName + " is NOT installed on " + serverName + " - FAILED";
            }
            catch
            {
                // return this is an exception occurs because the server cannot be accessed or found
                return "Cannot access or find the server: " + serverName + " - FAILED";
            }
        }

    }
}

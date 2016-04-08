namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.Logging;

    public class sitesTest
    {
        private List<appSite> Sites = new List<appSite>();
        private bool testPassed;
        private bool testFinished;


        public sitesTest(List<appSite> siteServers)
        {
            this.Sites = siteServers;
            this.testPassed = false;
            this.testFinished = false;
        }

        private void Passed()
        {
            this.testPassed = true;
        }

        private void Failed()
        {
            this.testPassed = false;
        }

        public bool getTestPassed()
        {
            return this.testPassed;
        }

        public bool getTestFinished()
        {
            return this.testFinished;
        }

         public void runSitesTest()
        {
            string siteOutput = "";
            string siteName = "";
            string siteBinding = "";
            string sitePath = "";
            string appState = "";
            string siteState = "";
            string siteProtocol = "";
            string sitePort = "";
            string sitePassed = "";
            string siteServer = "";
            int siteCount = 0;
            Log.Information("---Checking Web Applications Status---");

            //set-up our access specifications
            var connOptions = new ConnectionOptions
            {
                EnablePrivileges = true,
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketPrivacy
            };

            try   //try to connect to the server
            {


                foreach (appSite site in Sites)
                {   
                    //iterate through the site list
                    //Log.Information(site);
                    //grab the site name, protocol, and port - we don't want to have it iterate through all the sites, so we are selecting the right one by name.
                    var WMIScope = new ManagementScope("\\\\" + site.serverName + "\\root\\WebAdministration", connOptions);
                    siteServer = site.serverName;
                    WMIScope.Connect();
                    
                    var WMISiteQuery = new ObjectQuery("SELECT * FROM SITE WHERE Name = '" + site.appName + "'");
                    try  //try grabbing the site information including the binding info
                    {

                        var siteSearcher = new ManagementObjectSearcher(WMIScope, WMISiteQuery);
                        if (siteSearcher.Get().Count == 0)
                        {
                            Log.Information(site + " is missing or could not be connected to - FAILED");
                            Failed();
                        }
                        foreach (ManagementObject siteQueryObj in siteSearcher.Get())
                        {
                            siteName = siteQueryObj["Name"].ToString();
                            object returnSiteVal = siteQueryObj.InvokeMethod("GetState", null);
                            switch (returnSiteVal.ToString())
                            {
                                case "0": siteState = "Site: Starting"; break;
                                case "1": siteState = "Site: Started"; break;
                                case "2": siteState = "Site: Stopping"; break;
                                case "3": siteState = "Site: Stopped"; break;
                                case "4": siteState = "Site: Unknown"; break;
                            }
                            //see if we have any site bindings if the bindings equal null it doesnt do much - revisit
                            if (siteQueryObj["Bindings"] == null)
                                siteBinding = siteQueryObj["Bindings"].ToString();
                            else  //else check the virtual directory and the application pool if it can be done.
                            {
                                var WMISitePathQuery = new ObjectQuery("SELECT * FROM VIRTUALDIRECTORY WHERE SITENAME ='" + site.appName + "'");
                                try //try checking the virtual directory
                                {
                                    var pathSearcher = new ManagementObjectSearcher(WMIScope, WMISitePathQuery);
                                    foreach (ManagementObject pathQueryObj in pathSearcher.Get())
                                    {
                                        sitePath = pathQueryObj["PhysicalPath"].ToString();
                                        //Log.Information(sitePath);
                                    }

                                    var WMIAPPStateQuery = new ObjectQuery("SELECT * FROM APPLICATIONPOOL WHERE NAME = '" + site.appName + "'");
                                    try   // try checking the application pool
                                    {
                                        var appStateSearcher = new ManagementObjectSearcher(WMIScope, WMIAPPStateQuery);

                                        //iterate through the applications found and check their state
                                        foreach (ManagementObject queryObj in appStateSearcher.Get())
                                        {
                                            object returnAppVal = queryObj.InvokeMethod("GetState", null);

                                            switch (returnAppVal.ToString())
                                            {
                                                case "0": appState = "App: Starting"; break;
                                                case "1": appState = "App: Started"; break;
                                                case "2": appState = "App: Stopping"; break;
                                                case "3": appState = "App: Stopped"; break;
                                                case "4": appState = "App: Unknown"; break;
                                            }

                                        }
                                    }
                                    catch (ManagementException applicationErr)
                                    {
                                        Log.Information("An error occured while querying for Application Data: " + applicationErr.Message);
                                    }

                                }
                                catch (ManagementException virtualDirectoryErr)
                                {
                                    Log.Information("An error occurred while querying for Virtual Directory Data: " + virtualDirectoryErr.Message);
                                }
                                var siteBindings = siteQueryObj["Bindings"] as ManagementBaseObject[];
                                //Log.Information("Site Bindings: " + siteBindings.Count());
                                if (siteBindings != null)
                                {
                                    var siteProtocolObject = siteBindings[0].Properties["protocol"].Value;
                                    siteProtocol = siteProtocolObject.ToString();
                                    var sitePortObject = siteBindings[0].Properties["bindinginformation"].Value;
                                    sitePort = sitePortObject.ToString();
                                }
                                else
                                {
                                    siteProtocol = "No Protocol Found";
                                    sitePort = "No Port Found";
                                }
                                // test to see if the site is up and running otherwise.
                                if ((siteState == "Site: Starting" || siteState == "Site: Started") && 
                                    (appState == "App: Starting" || appState == "App: Started") && 
                                    (sitePort != "No Port Found" || siteProtocol != "No Protocol Found"))
                                {
                                    Passed();
                                    sitePassed = "- PASSED";
                                    siteCount++;
                                }
                                else
                                {
                                    Failed();
                                    sitePassed = "- FAILED";
                                }
                                siteOutput = siteServer + ", " + siteName + ", " + siteProtocol + ", " + sitePort + ", " + sitePath + ", " + siteState + ", " + appState + " " + sitePassed;
                                Log.Information(siteOutput);
                            }
                        }

                    }
                    catch (ManagementException siteErr)
                    {
                        Log.Information(site.appName + ": Cannot connect to or find the site: " + siteErr.Message);
                    }
                } //end of the foreach that iterates through the site list
                if (siteCount < Sites.Count)
                {
                    Log.Information("There should be " + Sites.Count + " active sites.  The Installation Test only counts " + siteCount + " active sites.");
                    Failed();
                }
            }
            catch (ManagementException serverErr)
            {
                Log.Information("Cannot connect to the server, Root/Webadministration may not be installed : " + siteServer + "; " + serverErr.Message + " - FAILED");
                Failed();
            }
            this.testFinished = true;
        }
    }
}

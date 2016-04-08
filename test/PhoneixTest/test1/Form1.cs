namespace Phoenix.Test.UI
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Firefox;
    using Phoenix.Test.Data;
    using Phoenix.Test.Installation.TestCase;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.UI.TestCases;
    using System;
    using System.Windows.Forms;
    using System.Threading;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.textBox_UserName.Text) || String.IsNullOrWhiteSpace(this.textBox_Password.Text) ||
                String.IsNullOrWhiteSpace(this.textBox_AdminPortalServer.Text))
            {
                MessageBox.Show("Please input Administrator username, password, and Admin Portal Servername.");
                return;
            }

            var test = new AdminPortalTest(
                this.textBox_UserName.Text, 
                this.textBox_Password.Text, 
                this.textBox_AdminPortalServer.Text,
                this.textBox_ClientId.Text, 
                this.textBox_ClientKey.Text, 
                this.textBox_TenantId.Text, 
                this.textBox1.Text,
                this.tenantUserAccount.Text,
                this.tenantUserPsw.Text);

            var data = test.GetCreatePlanData();
            data.clientId = this.textBox_ClientId.Text; data.clientKey = this.textBox_ClientKey.Text;
            data.tenantId = this.textBox_TenantId.Text; data.azureSubscription = this.textBox1.Text;
            var anySubscriptionName = test.GetRandomSubscriptionName();
            var tenantData = new TenantData()
            {
                emailAddress = this.tenantUserAccount.Text,
                password = this.tenantUserPsw.Text
            };

            test.AdminCreatePlanTest(data);
            test.AdminChangePlanAccess(data);
            test.AdminCreateUserTest(tenantData, data.planName);
            test.AdminOnboardSubscriptionTest(data, anySubscriptionName);
            test.TestCleanup();
        }

        [TestInitialize]
        public static void Init()
        {
            MessageBox.Show("Init");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ( String.IsNullOrWhiteSpace(this.textBox_TenantPortalServer.Text))
            {
                MessageBox.Show("Tenant Portal Servername.");
                return;
            }

            var test = new TenantPortalTest(this.tenantUserAccount.Text, this.tenantUserPsw.Text, this.textBox_TenantPortalServer.Text,this.regions.Text);
            test.TestInitialize();
            test.TenantCreateVmFromNewButtonTest();
            test.TestCleanup();

            MessageBox.Show("Tests done! Please check the VM status on tenant portal.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_TestPlan_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            //initialize values for status on screen
            cmpwapProgressBar.Value = 0;
            infraCheckPic.BackgroundImage = null;
            infraCheckPic.Update();
            lblCheckInfra.Text = "GATHER INFRASTRUCTURE INFORMATION";
            lblCheckInfra.Update();
            resourceProviderPic.BackgroundImage = null;
            resourceProviderPic.Update();
            lblCheckResourceProvider.Text = "CHECK RESOURCE PROVIDER";
            databasesPic.BackgroundImage = null;
            databasesPic.Update();
            lblCheckDatabases.Text = "CHECK DATABASES";
            servicesPic.BackgroundImage = null;
            servicesPic.Update();
            lblCheckServices.Text = "CHECK CMP SERVICE";
            webappsPic.BackgroundImage = null;
            webappsPic.Update();
            lblCheckWebApps.Text = "CHECK WEB APPS";
            lblAllTests.Text = "INSTALLATION TEST IN PROGRESS";
            lblAllTests.Visible = false;
            lblAllTests.Update();
            allTestsPic.BackgroundImage = null;
            allTestsPic.Update();

            //Use the manifest class to read in information for distributed environments.
            lblCheckInfra.Text = "GATHERING INFRASTRUCTURE INFORMATION...";
            lblCheckInfra.Update();
            lblAllTests.Visible = true;
            lblAllTests.Update();
            for (int progressCount = 0; progressCount < 2; progressCount++)
            {
                cmpwapProgressBar.PerformStep();
                cmpwapProgressBar.Update();
            }
            var Manifest = new infraManifest(textBox_ServerName.Text);
            appDatabase CMPDB = Manifest.GetCMPServer();
            appDatabase RPDB = Manifest.GetStoreServer();
            List<appDatabase> Databases = Manifest.GetDatabases();
            List<appSite> Sites = Manifest.GetSites();

            if (Manifest.GetManifestPassed() && Manifest.GetManifestFinished())
            {
                infraCheckPic.BackgroundImage = imageList1.Images[1];
                infraCheckPic.Update();
                lblCheckInfra.Text = "GATHERED INFRASTRUCTURE INFORMATION - PASSED";
                lblCheckInfra.Update();

                lblCheckResourceProvider.Text = "CHECKING RESOURCE PROVIDER...";
                lblCheckResourceProvider.Update();
                for (int progressCount = 0; progressCount < 2; progressCount++)
                {
                    cmpwapProgressBar.PerformStep();
                    cmpwapProgressBar.Update();
                    lblAllTests.Text = "INSTALLATION TEST IN PROGRESS.";
                    lblAllTests.Update();
                }
                var rpTest = new resourceProviderTest(RPDB);
                rpTest.runResourceProviderTest();

                Thread.Sleep(2000);
                if (rpTest.getTestPassed())
                {
                    resourceProviderPic.BackgroundImage = imageList1.Images[1];
                    resourceProviderPic.Update();
                    lblCheckResourceProvider.Text = "RESOURCE PROVIDER CHECKED - PASSED";
                    lblCheckResourceProvider.Update();
                }
                else
                {
                    resourceProviderPic.BackgroundImage = imageList1.Images[0];
                    resourceProviderPic.Update();
                    lblCheckResourceProvider.Text = "RESOURCE PROVIDER CHECKED - FAILED";
                    lblCheckResourceProvider.Update();
                }


                if (rpTest.getTestFinished())
                {
                    lblCheckDatabases.Text = "CHECKING DATABASES...";
                    lblCheckDatabases.Update();
                    for (int progressCount = 0; progressCount < 3; progressCount++)
                    {
                        cmpwapProgressBar.PerformStep();
                        cmpwapProgressBar.Update();
                        lblAllTests.Text = "INSTALLATION TEST IN PROGRESS..";
                        lblAllTests.Update();
                    }
                    Thread.Sleep(2000);
                    var dbTest = new databasesTest(Databases);
                    dbTest.runDatabasesTest();

                    if (dbTest.getTestPassed())
                    {
                        databasesPic.BackgroundImage = imageList1.Images[1];
                        databasesPic.Update();
                        lblCheckDatabases.Text = "DATABASES CHECKED - PASSED";
                        lblCheckDatabases.Update();
                    }
                    else
                    {
                        databasesPic.BackgroundImage = imageList1.Images[0];
                        databasesPic.Update();
                        lblCheckDatabases.Text = "DATABASES CHECKED - FAILED";
                        lblCheckDatabases.Update();
                    }


                    if (dbTest.getTestFinished())
                    {
                        lblCheckServices.Text = "CHECKING CMP SERVICE...";
                        lblCheckServices.Update();
                        for (int progressCount = 0; progressCount < 1; progressCount++)
                        {
                            cmpwapProgressBar.PerformStep();
                            cmpwapProgressBar.Update();
                            lblAllTests.Text = "INSTALLATION TEST IN PROGRESS...";
                            lblAllTests.Update();
                        }
                        Thread.Sleep(2000);
                        var svcsTest = new servicesTest(textBox_ServerName.Text);
                        svcsTest.AddService("CmpWorkerService");
                        //svcsTest.AddService("");  <- to add more services in the future if necessary.  I had the SQL and IIS services being tested, but it is redundant
                        //as the database and site tests will log if anything is wrong.
                        svcsTest.runServicesTest();

                        if (svcsTest.getTestPassed())
                        {
                            servicesPic.BackgroundImage = imageList1.Images[1];
                            servicesPic.Update();
                            lblCheckServices.Text = "CMP SERVICE - PASSED";
                            lblCheckServices.Update();
                        }
                        else
                        {
                            servicesPic.BackgroundImage = imageList1.Images[0];
                            servicesPic.Update();
                            lblCheckServices.Text = "CMP SERVICE - FAILED";
                            lblCheckServices.Update();
                        }

                        if (svcsTest.getTestFinished())
                        {
                            lblCheckWebApps.Text = "CHECKING WEB APPS...";
                            lblCheckWebApps.Update();
                            for (int progressCount = 0; progressCount < 13; progressCount++)
                            {
                                cmpwapProgressBar.PerformStep();
                                cmpwapProgressBar.Update();
                                lblAllTests.Text = "INSTALLATION TEST IN PROGRESS....";
                                lblAllTests.Update();
                            }
                            Thread.Sleep(2000);
                            var stsTest = new sitesTest(Sites);
                            stsTest.runSitesTest();
                            if (stsTest.getTestPassed())
                            {
                                webappsPic.BackgroundImage = imageList1.Images[1];
                                webappsPic.Update();
                                lblCheckWebApps.Text = "WEB APPS - PASSED";
                                lblCheckWebApps.Update();
                            }
                            else
                            {
                                webappsPic.BackgroundImage = imageList1.Images[0];
                                webappsPic.Update();
                                lblCheckWebApps.Text = "WEB APPS - FAILED";
                                lblCheckWebApps.Update();
                            }
                        }
                    }
                }
                lblAllTests.Text = "INSTALLATION TEST - COMPLETED!";
                lblAllTests.Update();
                allTestsPic.BackgroundImage = imageList1.Images[1];
                allTestsPic.Update();
                Log.Information("---END-OF-INSTALL-TEST---");
            }
            else  //all of it fails without the infrastructure manifest, skip it all and set them to failed if that is the case.
            {
                infraCheckPic.BackgroundImage = imageList1.Images[0];
                infraCheckPic.Update();
                lblCheckInfra.Text = "GATHERED INFRASTRUCTURE INFORMATION - FAILED";
                lblCheckInfra.Update();
                Log.Information("--- Manifest Creation Failed ---");

                resourceProviderPic.BackgroundImage = imageList1.Images[0];
                resourceProviderPic.Update();
                lblCheckResourceProvider.Text = "RESOURCE PROVIDER CHECKED - FAILED";
                lblCheckResourceProvider.Update();
                Log.Information("--- Resource Provider Check Failed as a result of Manifest Creation Failed ---");

                databasesPic.BackgroundImage = imageList1.Images[0];
                databasesPic.Update();
                lblCheckDatabases.Text = "DATABASES CHECKED - FAILED";
                lblCheckDatabases.Update();
                Log.Information("--- Database Check Failed as a result of Manifest Creation Failed ---");

                servicesPic.BackgroundImage = imageList1.Images[0];
                servicesPic.Update();
                lblCheckServices.Text = "CMP SERVICE - FAILED";
                lblCheckServices.Update();
                Log.Information("--- CMP Service Check Failed as a result of Manifest Creation Failed ---");

                webappsPic.BackgroundImage = imageList1.Images[0];
                webappsPic.Update();
                lblCheckWebApps.Text = "WEB APPS - FAILED";
                lblCheckWebApps.Update();
                Log.Information("--- Web Application Check Failed as a result of Manifest Creation Failed ---");

                lblAllTests.Text = "INSTALLATION TEST - COMPLETED!";
                lblAllTests.Update();
                allTestsPic.BackgroundImage = imageList1.Images[1];
                allTestsPic.Update();
                Log.Information("---END-OF-INSTALL-TEST---");

                for (int progressCount = 0; progressCount < 25; progressCount++)
                {
                    cmpwapProgressBar.PerformStep();
                    cmpwapProgressBar.Update();
                }
            }
        }
        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                textBox_Password.PasswordChar = '*';
            }
            else if (checkBox2.Checked == true)
            {
                textBox_Password.PasswordChar = (char)(byte)0;
            }
            textBox_Password.Refresh();
        }
    }
}

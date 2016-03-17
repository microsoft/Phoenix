using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using Phoenix.Test.UI.Framework.Advertiser.Pages;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.WebPages;
using Phoenix.Test.UI.TestCases;
using Phoenix.Test.Data;
//using Phoenix.Test.UI.Framework.Configuration;
//using Framework;
//using Framework.Authentication;
//using Framework.Helper;
//using Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
//using CampaignSummaryPage = Framework.Advertiser.Pages.CampaignSummaryPage;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Phoenix.Test.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var test = new AdminPortalTest();
            test.AdminCreateUserAccountTest();
        }



        public void TestCase02()
        {
            var driver = new FirefoxDriver();
            var page = new SmpPage(driver);
            //string user = "test01@microsoft.com";
            //string psw = GetPassword();

            driver.Url = "https://khphoenixsql2.redmond.corp.microsoft.com:30081/#Workspaces/All/dashboard";
            driver.Manage().Window.Maximize();
            //page.AddSubscription();
        }

        [TestInitialize]
        public static void Init()
        {
            MessageBox.Show("Init");
            //var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
            //var campaignCount = campaignSummaryPage.CampaignGridRowCount();

            //if (campaignCount < 2)
            //{
            //    for (var i = 0; i < 2 - campaignCount; i++)
            //    {
            //        HavingCreated(aMinimalValidCampaign()
            //            .WithName(expectedCampaignName()[i]));
            //    }
            //}

            //var keywordSummaryPage = campaignSummaryPage.MainCampaignSummarySection.NavigateKeywordsTab();

            //keywords = keywordSummaryPage.OpenAddKeywords();

            //campaignList = keywords.GetCampaignListOptionText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var test = new TenantPortalTest();
            test.TestInitialize();
            test.TenantCreateVmFromNewButtonTest();
            test.TestCleanup();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_TestPlan_Click(object sender, EventArgs e)
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phoenix.Test.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        // [TestMethod,
        // Owner(TestOwners.JohnYu),
        // Priority(TestPriority.Bvt)]
        // //Description("Verify summary tabs load correctly"),
        // //Category(Category.Bvt)]
        //public void TestCase1()
        //{   
        //    Phoenix.Test.UI.Framework.Pages.LoginPage loginPage = 
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
        //    campaignSummaryPage.MainCampaignSummarySection.NavigateAdGroupTab();
        //    campaignSummaryPage.MainCampaignSummarySection.NavigateAdsTab();
        //    campaignSummaryPage.MainCampaignSummarySection.NavigateKeywordsTab();
        //}

    }
}


namespace Phoenix.Test.UI.TestCases
{

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Support.UI;
    using Phoenix.Test.Common;
    using Phoenix.Test.Data;
    using Phoenix.Test.UI.Framework;
    using Phoenix.Test.UI.Framework.WebPages;
    using System.Data;

    [TestClass]
    public class TenantPortalTest : WAPTestBase
    {
        private LoginPage loginPage;
        private SmpPage smpPage;
        private string userName;
        private string password;
        private string serverName;

        public TenantPortalTest(string userAccount, string password, string serverName)
        {
            this.userName = userAccount;
            this.password = password;
            this.serverName = serverName;
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            driver.Url = "https://" + serverName + ":30081";

            this.driver.Wait(ExpectedConditions.TitleContains("Authentication"));
            this.driver.WaitForAjax();

            this.loginPage = new LoginPage(driver);
            LoginTenantProtal(this.userName, this.password);
            var welcomeWiz = new WelcomePage(driver);
            welcomeWiz.HandleWelcomeWizard();
            this.smpPage = new SmpPage(driver);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod(),
        Owner(TestOwners.JohnYu),
        Priority(TestPriority.P0),
        TestCategory(TestCategory.BVT),
        Description("Verify tenant user can create a VM from new button.")]
        public void TenantCreateVmFromNewButtonTest()
        {
            var createVmData = GetCreateVmData();
            smpPage.CreateVmFromNewButton(createVmData);
            smpPage.VerifyVmCreated(createVmData);
        }

        public void TenantCreateVmFromMainMenuTest()
        {
            var createVmData = GetCreateVmData();
            smpPage.CreateVmFromNewButton(createVmData);
            smpPage.VerifyVmCreated(createVmData);
        }

        public void LoginTenantProtal(string user, string psw)
        {
           loginPage.Login(user, psw);
        }

        public CreateVmData GetCreateVmData()
        {
            string anyVmName = GetRandomVmName();
            return new CreateVmData() { groupName = TestDataUtils.GetRandomCharString(7), serverName = anyVmName, userName = TestDataUtils.GetRandomCharString(7), localAdminPassword = "Password0)" };
        }

        public string GetRandomVmName()
        {
            return TestDataUtils.GetRandomCharString(8);
        }

        public void ReadConfigTestUser(DataSet config)
        {
            this.userName = config.Tables[6].Rows[0].ItemArray[0].ToString();
            this.password = config.Tables[6].Rows[0].ItemArray[1].ToString();
        }


































        //[TestMethod]
        ////[Owner(TestOwners.Longbin),
        ////Priority(TestPriority.P1),
        ////Description("[859415]different adgroup context"),
        ////Category(Category.Ads)]
        //public void test01()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
        //    campaignSummaryPage.EnterCampaign(campaignName)
        //        .EnterAdGroup(adGroupName);
        //    var adSummaryPage = campaignSummaryPage.MainCampaignSummarySection.NavigateAdsTab();

        //    var textModel = new TextAdModel()
        //    {
        //        AdTitle = adSummaryPage.GetAdDataInGrid(StringEnums.GridColumns.Adtitle, adTitle: textAdTitle),
        //        AdDisplayURL = adSummaryPage.GetAdDisplayURL(adTitle: textAdTitle),
        //        AdDescription = adSummaryPage.GetTextAdDescription(adTitle: textAdTitle),
        //        AdDestinationURL = adSummaryPage.GetAdDestinationURL(adTitle: textAdTitle)
        //    };

        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();

        //    Assert.IsTrue(createAdSection.CheckTextAdsFiles(textModel),
        //        "Failed to filled as the first text ad in the grid.");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("[859416]different account context"),
        //Category(Category.Ads)]
        //public void Create_AdDefaultDataVerificationOnAccountLevel()
        //{
        //    var wunderbarSection = new WunderbarSection(Browser);

        //    var accountList = wunderbarSection.GetAccountListText();
        //    if (wunderbarSection.GetAccountListText().Count > 2)
        //    {
        //        var campaignSummaryPage = wunderbarSection.SelectAccount(accountList[1]);
        //        if (!campaignSummaryPage.GetSummaryGrid().HasRows())
        //        {
        //            var campagnBuilder = aCampaign().With(anAdGroup().With(aTextAd()).With(aKeyword()));
        //            campaignSummaryPage.GoToCreateRegularCampaignPage()
        //                .FillStep1(campagnBuilder)
        //                .SaveStep1()
        //                .FillStep2(campagnBuilder)
        //                .SaveStep2();
        //        }
        //        campaignSummaryPage.MainCampaignSummarySection
        //        .NavigateAdsTab();

        //        var adSummaryPage = new AdSummaryPage(Browser);
        //        if (adSummaryPage.AdGridRowCount() <= 0)
        //        {
        //            adSummaryPage.ShowCreateAdPanel().FillTextAdInfo(aTextAd())
        //                .Save();
        //        }

        //        // Get first text ad title.
        //        string firstAdTitle = adSummaryPage.GetFilteredFirstAdTitle(GridFilterTarget.AdType, GridFilterOperator.Equal, "Text ad");

        //        var textModel = new TextAdModel()
        //        {
        //            AdTitle = firstAdTitle,
        //            AdDisplayURL = adSummaryPage.GetAdDisplayURL(adTitle: firstAdTitle),
        //            AdDescription = adSummaryPage.GetTextAdDescription(adTitle: firstAdTitle),
        //            AdDestinationURL = adSummaryPage.GetAdDestinationURL(adTitle: firstAdTitle)
        //        };

        //        var createAdSection = adSummaryPage.ShowCreateAdPanel();

        //        Assert.IsTrue(createAdSection.CheckTextAdsFiles(textModel),
        //            "Failed to filled as the first text ad in the grid.");
        //    }
        //    else
        //        throw new NotSupportedException("This customer only have one account!");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("[859426]Filter for an ad based ad id"),
        //Category(Category.Ads)]
        //public void FilterTextAd_BasedAdIdThenCreateNewAd()
        //{
        //    var adSummaryPage = NavigateToPage<CampaignSummaryPage>().MainCampaignSummarySection
        //        .NavigateAdsTab();
        //    var adId = adSummaryPage.GetAdDataInGrid(StringEnums.GridColumns.AdID, textAdTitle);

        //    adSummaryPage.FilterAd(GridFilterTarget.AdID, GridFilterOperator.Equal, adId);
        //    Assert.IsTrue(adSummaryPage.GetTotalAdCounts("Ad title").Equals(1),
        //        "Failed to filter for an ad based ad id.");

        //    var newTextAdModel = new TextAdBuilder().Build();
        //    adSummaryPage.ShowCreateAdPanel()
        //        .ChooseCampaignAndAdgroup(campaignName, null)
        //        .FillTextAdInfo(newTextAdModel)
        //        .Save();

        //    Assert.IsTrue(adSummaryPage.CheckTextAdInGrid(newTextAdModel.AdTitle),
        //        "Failed to create new text ad.");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("[422138]ad type"),
        //Category(Category.Ads)]
        //public void AdType_InAdGrid()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    var expectAdType = new string[3]{
        //        "Text ad",
        //        "WAP mobile ad",            
        //        "Rich ad"
        //    };
        //    adSummaryPage.FilterAd(GridFilterTarget.AdType, GridFilterOperator.Contains, expectAdType[0]);

        //    Assert.IsFalse(adSummaryPage.AdGridRowCount().Equals(0),
        //        "Cannot find Text ad type ads.");

        //    adSummaryPage.FilterAd(GridFilterTarget.AdType, GridFilterOperator.Contains, expectAdType[1]);

        //    if (adSummaryPage.AdGridRowCount().Equals(0))
        //    {
        //        NavigateToPage<CampaignSummaryPage>();
        //        HavingCreated(
        //            aCampaign()
        //                     .With(anAdGroup()
        //                     .With(aMobileAd())
        //                    .With(aKeyword()))
        //            );
        //        GotoAdSummaryPage();
        //    }
        //    Assert.IsFalse(adSummaryPage.AdGridRowCount().Equals(0),
        //        "Cannot find WAP mobile ad type ads.");

        //    adSummaryPage.FilterAd(GridFilterTarget.AdType, GridFilterOperator.Contains, expectAdType[2]);

        //    // Cannot create rich ad using selenium webdriver because it will handle windows pop-up.
        //    // Please manually add rich ad then check if show in grid.
        //    Assert.IsTrue(adSummaryPage.AdGridRowCount().Equals(0),
        //        "Cannot find Rich ad type ads.");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("DevicePref: AdsGrid: Create a text ad without device pref.Test at acocunt view"),
        //Category(Category.Ads)]
        //public void Create_ATextAdWithoutDevicePrefAtAccountView()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel()
        //        .ChooseCampaignAndAdgroup(campaignName, adGroupName: null);
        //    var oneTextBuilder = new TextAdBuilder();

        //    createAdSection.FillTextAdInfo(oneTextBuilder);
        //    createAdSection.Save();

        //    Assert.AreEqual<string>("All", adSummaryPage.GetAdDataInGrid(StringEnums.GridColumns.Devicepreference, oneTextBuilder.Build().AdTitle),
        //        "Failed to show All in new text ad with device pref option unselected at account level");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("DevicePref: AdsGrid: Create a text ad without device pref.Test at campaign view"),
        //Category(Category.Ads)]
        //public void Create_ATextAdWithoutDevicePrefAtCampaignView()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();

        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
        //    campaignSummaryPage.EnterCampaign(campaignName);
        //    campaignSummaryPage.MainCampaignSummarySection
        //        .NavigateAdsTab()
        //        .ShowCreateAdPanel();
        //    var twoTextBuilder = new TextAdBuilder();

        //    createAdSection.FillTextAdInfo(twoTextBuilder);
        //    createAdSection.Save();

        //    Assert.AreEqual<string>("All", adSummaryPage.GetAdDataInGrid(StringEnums.GridColumns.Devicepreference, twoTextBuilder.Build().AdTitle),
        //        "Failed to show All in new text ad with device pref option unselected at campaign level");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Longbin),
        //Priority(TestPriority.P1),
        //Description("DevicePref: AdsGrid: Create a text ad without device pref.Test at adgroup view"),
        //Category(Category.Ads)]
        //public void Create_ATextAdWithoutDevicePrefAtAdgroupView()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();

        //    campaignSummaryPage.EnterCampaign(campaignName)
        //        .EnterAdGroup(adGroupName);
        //    var adSummaryPage = campaignSummaryPage.MainCampaignSummarySection
        //        .NavigateAdsTab();
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    var threeTextBuilder = new TextAdBuilder();

        //    createAdSection.FillTextAdInfo(threeTextBuilder);
        //    createAdSection.Save();

        //    Assert.AreEqual<string>("All", adSummaryPage.GetAdDataInGrid(StringEnums.GridColumns.Devicepreference, threeTextBuilder.Build().AdTitle),
        //        "Failed to show All in new text ad with device pref option unselected at adgroup level");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Xiaolong),
        //Priority(TestPriority.P1),
        //Description("[842858]DevicePref: Edit a text ad with device pref.Test enabling and disabling device pref.Test at account view"),
        //Category(Category.Ads)]
        //public void Edit_AdDevicePrefAtAcocuntView()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
        //    campaignSummaryPage.EnterCampaign(campaignName)
        //        .EnterAdGroup(adGroupName);
        //    var adSummaryPage = campaignSummaryPage.MainCampaignSummarySection.NavigateAdsTab();

        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    var threeTextBuilder = new TextAdBuilder();
        //    createAdSection.FillTextAdInfo(threeTextBuilder);
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefDisplayed(), "Failed to display mobile device pref in ads summary grid");
        //    createAdSection.CheckMobileDevicePref();
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefSelecte(), "Failed to edit mobile device pref in ads summary grid");
        //    createAdSection.UnCheckMobileDevicePref();
        //    createAdSection.Save();

        //    adSummaryPage.EnterAds(threeTextBuilder);
        //    Assert.IsFalse(createAdSection.IsMobileDevicePrefSelecte(), "Failed to save mobile device pref in ads summary grid");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Xiaolong), Priority(TestPriority.P1),
        //Description("[842858]DevicePref: Edit a text ad with device pref.Test enabling and disabling device pref.Test at campaign view"),
        //Category(Category.Ads)]
        //public void Edit_AdDevicePrefAtCampaignView()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();

        //    campaignSummaryPage.EnterCampaign(campaignName);
        //    var adSummaryPage = campaignSummaryPage.MainCampaignSummarySection
        //        .NavigateAdsTab();
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    var threeTextBuilder = new TextAdBuilder();
        //    createAdSection.FillTextAdInfo(threeTextBuilder);
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefDisplayed(), "Failed to display mobile device pref in ads summary grid");
        //    createAdSection.CheckMobileDevicePref();
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefSelecte(), "Failed to edit mobile device pref in ads summary grid");
        //    createAdSection.UnCheckMobileDevicePref();
        //    createAdSection.Save();

        //    adSummaryPage.EnterAds(threeTextBuilder);
        //    Assert.IsFalse(createAdSection.IsMobileDevicePrefSelecte(), "Failed to save mobile device pref in ads summary grid");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Xiaolong), Priority(TestPriority.P1),
        //Description("[842858]DevicePref: Edit a text ad with device pref.Test enabling and disabling device pref.Test at adgroup view"),
        //Category(Category.Ads)]
        //public void Edit_AdDevicePrefAtAdgroupView()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();

        //    campaignSummaryPage.EnterCampaign(campaignName)
        //        .EnterAdGroup(AdGroupBuilder.DefaultAdgroupName);
        //    var adSummaryPage = campaignSummaryPage.MainCampaignSummarySection
        //        .NavigateAdsTab();
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    var threeTextBuilder = new TextAdBuilder();
        //    createAdSection.FillTextAdInfo(threeTextBuilder);
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefDisplayed(), "Failed to display mobile device pref in ads summary grid");
        //    createAdSection.CheckMobileDevicePref();
        //    Assert.IsTrue(createAdSection.IsMobileDevicePrefSelecte(), "Failed to edit mobile device pref in ads summary grid");
        //    createAdSection.UnCheckMobileDevicePref();
        //    createAdSection.Save();

        //    adSummaryPage.EnterAds(threeTextBuilder);
        //    Assert.IsFalse(createAdSection.IsMobileDevicePrefSelecte(), "Failed to save mobile device pref in ads summary grid");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden),
        //Priority(TestPriority.P1),
        //Description("Verify a new adText can be created and saved inline"),
        //Category(Category.Ads)]
        //public void InlineCreate_AdTextCanBeCreatedAndSavedInline()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    var updateAdTitle = AnyAdTitle();

        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    createAdSection.ChooseCampaignAndAdgroup(campaignName)
        //        .FillTextAdInfo(aTextAd().WithTitle(updateAdTitle));
        //    createAdSection.Save();
        //    Assert.IsTrue(adSummaryPage.CheckTextAdInGrid(updateAdTitle), "Failed to save the created text ad");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden), Priority(TestPriority.P1),
        //Description("Verify first available text ad can be deleted"),
        //Category(Category.Ads)]
        //public void InlineDelete_FirstAvailableTextAd()
        //{

        //    var adSummaryPage = this.GotoAdSummaryPage();
        //    var initialGridCount = adSummaryPage.GetTotalAdCounts("Ad title");
        //    Assert.IsTrue(initialGridCount >= 1, "InlineDelete_FirstAvailableTextAd: Ad Count = {0}; None to Delete.");

        //    adSummaryPage.DeleteAd(new[] { 1 });

        //    var currentGridAdCount = adSummaryPage.GetTotalAdCounts("Ad title");

        //    Assert.AreEqual(initialGridCount - 1, currentGridAdCount, "Failed to delete the first available text ad");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden),
        //Priority(TestPriority.P1),
        //Description("Verify selectable columns can be add in ads summary grid"),
        //Category(Category.Ads)]
        //public void InlineAdd_AdSelectedColumnCanBeAddedInGridAndSavedInline()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    adSummaryPage.AddAllSelectableColumnsInAdGrid();
        //    Assert.IsTrue(adSummaryPage.CheckAllSelectedColumnsInKeywordGrid(), "Failed to add all the selected column in ads summary grid");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden), Priority(TestPriority.P1),
        //Description("Verify selectable columns can be removed from ads summary grid "),
        //Category(Category.Ads)]
        //public void InlineRemove_AdSelectedColumnCanBeRemovedFromTheGridAndSavedInlineExceptDefault()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    adSummaryPage.RemoveAllSelectableColumnsFromAdGrid();
        //    Assert.AreEqual(1, adSummaryPage.GetSelectedColumnsInKeywordGrid(), "Failed to removed all the selected column from ads summary grid expect the defult");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden),
        //Priority(TestPriority.P1),
        //Description("Verify ad can be created and cancel inline"),
        //Category(Category.Ads)]
        //public void InlineCancel_AdCreationCanBeCancelInline()
        //{
        //    var adSummaryPage = GotoAdSummaryPage();
        //    var adTitle = AnyAdTitle();

        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    createAdSection.FillTextAdInfo(aTextAd().WithTitle(adTitle));

        //    createAdSection.Cancel();
        //    Assert.IsFalse(adSummaryPage.CheckTextAdInGrid(adTitle), "failed to cancel the created text ad");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden),
        //Priority(TestPriority.P1),
        //Description("Verify a new adText can be created and save and create another also be saved inline"),
        //Category(Category.Ads)]
        //public void InlineCreate_AdTextCanBeSavedAndCreateAnotherInline()
        //{
        //    //  TextAdBuilder adModel;
        //    var adSummaryPage = GotoAdSummaryPage();

        //    var updateAdTitle = AnyAdTitle();

        //    //Create the first Ad
        //    var createAdSection = adSummaryPage.ShowCreateAdPanel();
        //    createAdSection.ChooseCampaignAndAdgroup(campaignName).FillTextAdInfo(aTextAd().WithTitle(updateAdTitle));
        //    var createAnotherAdTextInline = createAdSection.SaveAndCreateAnother();

        //    //create another ad inline
        //    var secondAdTitle = AnyAdTitle();
        //    createAnotherAdTextInline.ChooseCampaignAndAdgroup(campaignName).FillTextAdInfo(aTextAd().WithTitle(secondAdTitle));
        //    createAdSection.Save();

        //    Assert.IsTrue(adSummaryPage.CheckTextAdInGrid(updateAdTitle), "Failed to save the first created text ad");
        //    adSummaryPage.RemoveFilter();
        //    Assert.IsTrue(adSummaryPage.CheckTextAdInGrid(secondAdTitle), "Failed to save the second created text ad");
        //}

        //[TestMethod]
        //[Owner(TestOwners.Eden),
        //Priority(TestPriority.P1),
        //Description("Verify Ad is retained when create and saved defult Ad when pagination is set to 200Text ads"),
        //Category(Category.Ads)]
        //public void InlineSave_preFilledAdWhenANewAdCreated_withTextPageSetTo200()
        //{
        //    var campaignSummaryPage = NavigateToPage<CampaignSummaryPage>();
        //    campaignSummaryPage.EnterCampaign(campaignName)
        //        .EnterAdGroup(adGroupName);
        //    var adSummarypage = campaignSummaryPage.MainCampaignSummarySection.NavigateAdsTab();

        //    adSummarypage.SetTextRowsPerPage(200);

        //    //Create the Text Ad 
        //    var createAdWith200PaginationSet = adSummarypage.ShowCreateAdPanel();
        //    createAdWith200PaginationSet.Save();

        //    //Note: The purpose of this test is to show new ad can be create in any pagination set 
        //    adSummarypage.SetTextRowsPerPage(20);
        //    var createAdWith20PaginationSet = adSummarypage.ShowCreateAdPanel();
        //    createAdWith20PaginationSet.Save();

        //    //TODO Verification
        //}

        ///// <summary>
        /////Go to the ad summary page.
        ///// </summary>
        ///// <returns>AdSummaryPage.</returns>
        //private AdSummaryPage GotoAdSummaryPage()
        //{
        //    var campaignSumamryPage = NavigateToPage<CampaignSummaryPage>();
        //    return campaignSumamryPage.MainCampaignSummarySection.NavigateAdsTab();
        //}
    }
}

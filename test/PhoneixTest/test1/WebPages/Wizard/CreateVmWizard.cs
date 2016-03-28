using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Test.UI.Framework.WebPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.UI.Framework.Controls;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.Data;
    using System.Windows.Forms;
    using OpenQA.Selenium.Support.UI;

    class CreateVmWizard : Page
    {
        [FindsBy(How = How.ClassName, Using = "wizard-button-next")]
        private HtmlButton next { get; set; }

        [FindsBy(How = How.ClassName, Using = "wizard-button-back")]
        private HtmlButton back { get; set; }

        // step 1
        [FindsBy(How = How.Name, Using = "SubscriptionName")]
        private HtmlComboBox planName { get; set; }

        [FindsBy(How = How.Id, Using = "VmAppNameSelect")]
        private HtmlTextBox groupName { get; set; }

        [FindsBy(How = How.Id, Using = "VmRegion")]
        private HtmlComboBox region { get; set; }

        // step 2
        [FindsBy(How = How.Id, Using = "VmServerName")]
        private HtmlTextBox serverName { get; set; }

        [FindsBy(How = How.Id, Using = "VmSourceImage")]
        private HtmlComboBox azureOsVersion { get; set; }

        [FindsBy(How = How.Id, Using = "VmAdminGroup")]
        private HtmlTextBox userName { get; set; }

        [FindsBy(How = How.Id, Using = "VmAdminPassword")]
        private HtmlTextBox localAdminPassword { get; set; }

        // step 3
        [FindsBy(How = How.Id, Using = "extradrivescheckbox")]
        private HtmlCheckBox needAdditionalDrives { get; set; }


        
        public CreateVmWizard(IWebDriver browser)
            : base(browser) 
        {
        }

        public override HtmlControl VerifyPageElement
        {
            get { return next; }
        }

        public void GoNext()
        {
            System.Threading.Thread.Sleep(500);
            this.next.Click();
        }

        public void Complete()
        {
            this.next.Click();
        }

        public void GoBack()
        {
            this.back.Click();
        }

        public void Step1(CreateVmData data,Page page)
        {
            Log.Information("Input Group Name.");
            this.groupName.SetText(data.groupName);
            IWebElement ee = page.SearchContext.FindElement(By.Id("VmRegion"));
            SelectElement e = new SelectElement(ee);
            e.WrappedElement.Click();
            e.SelectByText("Central US");
        }

        public void Step2(CreateVmData data)
        {
            Log.Information("Input Server Name.");
            this.serverName.SetText(data.serverName);
            Log.Information("Input User Name.");
            this.userName.SetText(data.userName);
            Log.Information("Input Password.");
            this.localAdminPassword.SetText(data.localAdminPassword);
        }

        public void Step3(CreateVmData data)
        {
            //if (data.needAdditionalDrives != null && data.needAdditionalDrives)
            //    this.needAdditionalDrives.Check();
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="PrerequisitesProgressPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Next Manager Setup PrerequisitesProgress Page
//           This is the page that handles prequisites checks progress.
// </summary>
// This UI is for temp usage. The real code will be checked in M3
// TODO: bug#47407
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Threading;
    using System.Xml;  
    using CMP.Setup.SetupFramework;
    using WpfResources;

    using FilteredPrerequisitesCounts =
            System.Collections.Generic.Dictionary<string, int>;

    using XmlFilteredPrerequisites =
            System.Collections.Generic.Dictionary<string, System.Xml.XmlDocument>;

    /// <summary>
    /// TODO: Verify if this is already defined somewhere
    /// </summary>
    public enum PrerequisitesCheckResult 
    { 
        /// <summary>
        /// Prerequisites Check Unknown 
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Prerequisites Check Passed
        /// </summary>
        Passed = 0,

        /// <summary>
        /// Prerequisites Check Warning 
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Prerequisites Check Failed 
        /// </summary>
        Failed = 2 
    }

    /// <summary>
    /// Interaction logic for PrerequisitesProgressPage.xaml
    /// </summary>
    public partial class PrerequisitesProgressPage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PrerequisitesProgressPage(Page page)
            : base(page, WPFResourceDictionary.PrerequisitesStepTitle, 2)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        public PrerequisitesProgressPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The delegate used to report progress 
        /// </summary>
        private delegate void ProgressDelegate();

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();

            this.Page.Host.SetNextButtonState(true, false, null);
            
            // Launch the prereqs checks 
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                progressBarCheckPrerequisites.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new ProgressDelegate(this.LaunchPrerequisiteChecks));
            }
            
            // Save the ID of this page so that we can later jump to this page to check again prerequisites 
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ReloadPrerequisitesPageChoice, this.Page.Id);
        }

        /// <summary>
        /// Exits the page.
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        /// <summary>
        /// Processes Installs Prerequisites in a seperate thread.  Then jumps to the next page when done.
        /// </summary>
        private void LaunchPrerequisiteChecks()
        {
            this.Page.Host.SetPreviousButtonState(true, false, null);
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessPrerequisitesCheckThread));
        }

        /// <summary>
        /// Queue Work Item thread: process installs prereqs. 
        /// </summary>
        /// <param name="o">Unused - Req'ed by WaitCallback</param>
        private void ProcessPrerequisitesCheckThread(object o)
        {
            Debug.WriteLine("ProcessPrerequisitesCheck Starting");
            
            // If we are not uninstalling, Do the prereq check
            // Get the list of components we are installing
            SetupHelpers.DoAllPrerequisiteChecks();
            Debug.WriteLine("ProcessPrerequisitesCheck Done");
            
            // We use the delegate below to move to the next page.
            this.progressBarCheckPrerequisites.Dispatcher.Invoke(
                DispatcherPriority.Normal, new ProgressDelegate(this.PrerequisitesDone));
        }

        /// <summary>
        /// Invoked on the UI thread when prereqs checks are complete. 
        /// </summary>
        private void PrerequisitesDone()
        {
            // Process results 
            this.ProcessPrerequisiteChecksResults();
            
            // Adjust UI 
            this.Page.Host.SetPreviousButtonState(true, false, null);
            this.Page.Host.SetNextButtonState(true, false, null);
            
            // Navigate away
            PageNavigation.Instance.MoveToNextPage();
        }

        /// <summary>
        /// Analyze the data produced by the prereq process, filter and 
        /// save everything in the property bag 
        /// </summary>
        private void ProcessPrerequisiteChecksResults()
        {
            // TODO Iplement prerequisite automatic installation checks.
        }
    }
}

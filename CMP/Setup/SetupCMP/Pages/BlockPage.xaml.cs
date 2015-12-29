//-----------------------------------------------------------------------
// <copyright file="AddRemovePage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the page that provides the setup blocking reason
// </summary>
// This UI is for temp usage. The real code will be checked in M3
// TODO: bug#47405
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.IO;
    using CMP.Setup.SetupFramework;

    /// <summary>
    /// Interaction logic for BlockPage.xaml
    /// </summary>
    public partial class BlockPage : BasePageForWpfControls
    {

        public BlockPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            InitializeComponent();
        }

        public BlockPage()
        {
            InitializeComponent();
        }

        public override void EnterPage()
        {
            base.EnterPage();
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.BlockReason))
            {
                blockReason.Text = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.BlockReason);
            }

            this.Page.Host.SetNextButtonState(true, false, null);
        }

        public override void ExitPage()
        {
            base.ExitPage();
        }

    }
}

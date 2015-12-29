//-----------------------------------------------------------------------
// <copyright file="SwitchPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is just a base landing page.
// </summary>
//-----------------------------------------------------------------------
#region Using directives

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
using CMP.Setup.SetupFramework;
using WpfResources;

#endregion

namespace CMP.Setup
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class SwitchPage : BasePageForWpfControls
    {
        public SwitchPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 0)
        {
            InitializeComponent();
        }

        public SwitchPage()
        {
            InitializeComponent();
        }

        public override void EnterPage()
        {
            base.EnterPage();
        }

        /// <summary>
        /// ExitPage
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }
    }
}

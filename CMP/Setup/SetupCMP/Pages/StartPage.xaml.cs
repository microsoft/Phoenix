//-----------------------------------------------------------------------
// <copyright file="StartPage.xaml.cs" company="Microsoft">
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

#endregion

namespace CMP.Setup
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : BasePageForWpfControls
    {
        public StartPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 0)
        {
            InitializeComponent();
        }

        public StartPage()
        {
            InitializeComponent();
        }

        public override void EnterPage()
        {
            base.EnterPage();
            // This is just a base landing page... we should move to the next page right away.
            // This is done the the main program (program.cs)
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

// Copyright (c) Microsoft Corporation.  All rights reserved.
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

namespace CMP.Setup
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class TemplatePage : BasePageForWpfControls
    {
        public TemplatePage(CMP.Setup.SetupFramework.Page page)
            : base(page, "Template Page Do Not Display", 0)
        {
            InitializeComponent();
        }

        public TemplatePage()
        {
            InitializeComponent();
        }

        public override void EnterPage()
        {
            base.EnterPage();
        }

        public override void ExitPage()
        {
            base.ExitPage();
        }
    }
}

//---------------------------------------------------------------------------
// <copyright file="WPFFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> 
// </summary>
//---------------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections.Generic;
    using CMP.Setup.SetupFramework;
    using System.Text;
    
    class WpfFactory : Factory
    {
        public override IPageHost CreateHost()
        {
            return new WpfFormsHostPage();
        }

        public override IPageUI CreatePage(Page page, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            return (IPageUI)Activator.CreateInstance(type, page);
        }
    }
}

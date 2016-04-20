// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlComboBox.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html ComboBox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

    class HtmlComboBox : HtmlControl
    {

        public HtmlComboBox(Page page, By by)
            : base(page, by)
        {
        }
    }
}

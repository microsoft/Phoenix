using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
  

    class HtmlComboBox : HtmlControl
    {

        public HtmlComboBox(Page page, By by)
            : base(page, by)
        {
          
         
        }
    }
}

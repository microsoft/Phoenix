using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Documents;

namespace CMP.Setup
{
    class AutoHyperlink : Hyperlink
    {
        public AutoHyperlink()
        {
            Click += (o, e) => Process.Start(NavigateUri.ToString());
        }
    }
}

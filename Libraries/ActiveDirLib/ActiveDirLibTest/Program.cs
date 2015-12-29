using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ActiveDirLib.DomainJoin.Join("redmond.corp.microsoft.com", "---", "markwes",    
                "OU=ITManaged,OU=ITServices,DC=redmond,DC=corp,DC=microsoft,DC=com");
        }
    }
}

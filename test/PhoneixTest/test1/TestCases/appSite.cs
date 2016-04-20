namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class appSite
    {
        public string serverName;
        public string appName;

        public appSite(string server, string app){
            this.serverName = server;
            this.appName = app;
        }
    }
}

namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class appDatabase
    {
        public string serverName;
        public string databaseName;
        public string password;
        public string userid;

        public appDatabase(string server, string dbname, string userid, string password){
            this.serverName = server;
            this.databaseName = dbname;
            this.userid = userid;
            this.password = password;
        }
    }
}

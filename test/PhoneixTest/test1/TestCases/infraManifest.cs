namespace Phoenix.Test.Installation.TestCase
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Phoenix.Test.UI.Framework;
using Phoenix.Test.UI.Framework.Logging;
using KryptoLib;


    //gathers the website and database information from the CMPWAPEXTENSION web.config
    //The assumption is that the CMP service is installed on the same box 
    //and that the user is an admin on the box



    public class infraManifest
    {
        private List<appSite> appSites = new List<appSite>();
        private List<appDatabase> appDatabases = new List<appDatabase>();
        private bool infraManifestFinished;
        private bool infraManifestPassed;

        public bool GetManifestFinished()
        {
            return this.infraManifestFinished;
        }

        public bool GetManifestPassed()
        {
            return this.infraManifestPassed;
        }

        public void Failed()
        {
            this.infraManifestPassed = false;
        }

        public void Passed()
        {
            this.infraManifestPassed = true;
        }

        public void Finished()
        {
            this.infraManifestFinished = true;
        }
        public string parseConnection(string connectionstr)
        //parses the database connection strings from the web.config file of CMPWAPExtension
        {
            string datasource = "";
            string catalog = "";
            string user = "";
            string password = "";
            string [] connectionParts;
            string [] valueParts;
            appDatabase parseDB;

            //get the individual parts isolated
            connectionParts = connectionstr.Split(';');                                                                                                                                     
            foreach (string connectionPart in connectionParts)
            {
                //Log.Information(connectionPart);

                string tempParseString = connectionPart;
                valueParts = tempParseString.Split('=');
                valueParts[0] = valueParts[0].ToLower();
                //I was changing the case of the value string at this point, and causing myself grief.  Can't change the password case.


                if (valueParts[0].Contains("data source") || valueParts[0].Contains("server")) { 
                    valueParts[1] = valueParts[1].ToLower();
                    datasource = valueParts[1];
                }
                if (valueParts[0].Contains("initial catalog") || valueParts[0].Contains("database"))
                { 
                    valueParts[1] = valueParts[1].ToLower();
                    catalog = valueParts[1];
                    //Log.Information(catalog);
                }
                if (valueParts[0].Contains("user id"))
                { 
                    user = valueParts[1];
                }
                if (valueParts[0].Contains("password"))
                { 
                    password = valueParts[1];
                }

            }

            parseDB = new appDatabase(datasource, catalog, user, password);
            appDatabases.Add(parseDB);
            connectionstr = "Connect String to " + catalog + " on server " + datasource + " found";
            return connectionstr;
        }

        public void loadPortalDBSiteData(appDatabase Database)
        {
            SqlDataAdapter siteInfoAdapter;
            DataTable siteInfoTable;
            string siteInfoQuery = "";
            int siteInfoCount = 0;
            string [] siteparts;
            string serverName;

            try
            {
                SqlConnection siteInfoConnection = new SqlConnection("server=" + Database.serverName + ", 1433; database=" + Database.databaseName + "; user id=" + Database.userid + "; password=" + Database.password + "; Integrated Security=False");
                siteInfoConnection.Open();
                siteInfoQuery = "SELECT [Namespace],[Value] FROM [Microsoft.MgmtSvc.PortalConfigStore].[Config].[Settings] WHERE Name Like '%FQDN%'";
                siteInfoAdapter = new SqlDataAdapter(siteInfoQuery, siteInfoConnection);
                siteInfoTable = new DataTable();
                siteInfoCount = siteInfoAdapter.Fill(siteInfoTable);
                if (siteInfoCount != 0){
                    foreach (DataRow row in siteInfoTable.Rows) // Loop over the rows.
                    {
                        //parse out the site server name
                        siteparts = row.ItemArray[1].ToString().Split(':');
                        serverName = siteparts[1].Replace("//","");
                        switch (row.ItemArray[0].ToString())
                        {
                            case "AdminSite": 
                                appSites.Add(new appSite(serverName,"MgmtSvc-AdminSite")); 
                                break;
                            case "AuthSite":
                                appSites.Add(new appSite(serverName, "MgmtSvc-AuthSite")); 
                                break;
                            case "TenantSite":
                                appSites.Add(new appSite(serverName, "MgmtSvc-TenantSite")); 
                                break;
                            case "WindowsAuthSite":
                                appSites.Add(new appSite(serverName, "MgmtSvc-WindowsAuthSite")); 
                                break;

                        }

                    }
                }
                else
                {
                    Log.Information("No Portal Sites Information Found In The [Microsoft.MgmtSvc.PortalConfigStore].[Config].[Settings] DB Table.");
                    Failed();
                }
                siteInfoConnection.Close();
            }
            catch (Exception e)
            {
                Log.Information(e.Message.ToString());
                Failed();
            }
        }

        public void loadStoreAPISiteData(appDatabase Database)
        {
            SqlDataAdapter siteInfoAdapter;
            DataTable siteInfoTable;
            string siteInfoQuery = "";
            int siteInfoCount = 0;
            string[] siteparts;
            string serverName;

            try
            {
                SqlConnection siteInfoConnection = new SqlConnection("server=" + Database.serverName + ", 1433; database=" + Database.databaseName + "; user id=" + Database.userid + "; password=" + Database.password + "; Integrated Security=False");
                siteInfoConnection.Open();
                siteInfoQuery = "SELECT [Namespace],[Value] FROM [Microsoft.MgmtSvc.Store].[Config].[Settings] WHERE Name Like '%fqdn%'";
                siteInfoAdapter = new SqlDataAdapter(siteInfoQuery, siteInfoConnection);
                siteInfoTable = new DataTable();
                siteInfoCount = siteInfoAdapter.Fill(siteInfoTable);
                if (siteInfoCount != 0)
                {
                    foreach (DataRow row in siteInfoTable.Rows) // Loop over the rows.
                    {
                        //parse out the site server name
                        siteparts = row.ItemArray[1].ToString().Split(':');
                        serverName = siteparts[1].Replace("//", "");  //trim the forward slash
                        switch (row.ItemArray[0].ToString())
                        {
                            case "AdminAPI":
                                appSites.Add(new appSite(serverName, "MgmtSvc-AdminAPI"));
                                break;
                            case "TenantAPI":
                                appSites.Add(new appSite(serverName, "MgmtSvc-TenantAPI"));
                                break;
                            case "TenantPublicAPI":
                                appSites.Add(new appSite(serverName, "MgmtSvc-TenantPublicAPI"));
                                break;
                        }

                    }
                }
                else
                {
                    Log.Information("No API Sites Information Found In The [Microsoft.MgmtSvc.Store].[Config].[Settings] DB Table.");
                    Failed();
                }
                siteInfoConnection.Close();
            }
            catch (Exception e)
            {
                Log.Information(e.Message.ToString());
                Failed();
            }
        }

        public void loadResourceProviderSiteDate(appDatabase Database)
        {
            SqlDataAdapter siteInfoAdapter;
            DataTable siteInfoTable;
            string siteInfoQuery = "";
            int siteInfoCount = 0;
            string[] siteparts;
            string serverName;

            try
            {
                SqlConnection siteInfoConnection = new SqlConnection("server=" + Database.serverName + ", 1433; database=" + Database.databaseName + "; user id=" + Database.userid + "; password=" + Database.password + "; Integrated Security=False");
                siteInfoConnection.Open();
                siteInfoQuery = "SELECT [Name],[AdminForwardingAddress] FROM [Microsoft.MgmtSvc.Store].[mp].[ResourceProviders]";
                siteInfoAdapter = new SqlDataAdapter(siteInfoQuery, siteInfoConnection);
                siteInfoTable = new DataTable();
                siteInfoCount = siteInfoAdapter.Fill(siteInfoTable);
                if (siteInfoCount != 0)
                {
                    foreach (DataRow row in siteInfoTable.Rows) // Loop over the rows.
                    {
                        //parse out the site server name
                        siteparts = row.ItemArray[1].ToString().Split(':');
                        serverName = siteparts[1].TrimStart('/');
                        //it concerns me that the the complete site addresses are not in the AdminForwardingAddress, but it is the best information available.
                        switch (row.ItemArray[0].ToString())
                        {
                            case "monitoring":
                                appSites.Add(new appSite(serverName, "MgmtSvc-Monitoring"));
                                break;
                            case "mysqlservers":
                                appSites.Add(new appSite(serverName, "MgmtSvc-MySQL"));
                                break;
                            case "usageservice":
                                appSites.Add(new appSite(serverName, "MgmtSvc-Usage"));
                                break;
                            case "marketplace":
                                appSites.Add(new appSite(serverName, "MgmtSvc-WebAppGallery"));//
                                break;
                            case "sqlservers":
                                appSites.Add(new appSite(serverName, "MgmtSvc-SQLServer"));
                                break;
                            case "CmpWapExtension":
                                appSites.Add(new appSite(serverName, "MgmtSvc-CmpWapExtension"));
                                break;
                        }

                    }
                }
                else
                {
                    Log.Information("No Resource Provider Sites Information Found In The [Microsoft.MgmtSvc.Store].[mp].[ResourceProviders] DB Table.");
                    Failed();
                }
                siteInfoConnection.Close();
            }
            catch (Exception e)
            {
                Log.Information(e.Message.ToString());
                Failed();
            }
        }

        public appDatabase GetCMPServer()
        //look-up CMP database and return information if found otherwise, send back blank information.
        {
            foreach(appDatabase TempDB in appDatabases){
                if (TempDB.databaseName == "cmp_db"){
                    return TempDB;
                }
            }
            return new appDatabase("", "cmp_db", "", "");
        }

        public appDatabase GetStoreServer()
        //look-up CMP database and return information if found otherwise, send back blank information.
        {
            foreach (appDatabase TempDB in appDatabases)
            {
                if (TempDB.databaseName == "microsoft.mgmtsvc.store")
                {
                    return TempDB;
                }
            }
            return new appDatabase("", "microsoft.mgmtsvc.store", "", "");
        }

        public List<appDatabase> GetDatabases()
        //look-up CMP database and return information if found otherwise, send back blank information.
        {
            return this.appDatabases;
        }

        public List<appSite> GetSites()
        //look-up CMP database and return information if found otherwise, send back blank information.
        {
            return this.appSites;
        }

        public infraManifest(string cmpserver)
        {
            this.infraManifestFinished = false;
            this.infraManifestPassed = false;
            bool encrypted = false;
            string encryptedPasswordString = "";
            string passwordString = "";
            string dbString = "";
            string kryptoString = "";
            string[] kryptoParts;
            X509Krypto Krypto = null;
            //try finding the file web.config for CMP WAP extension on the on the target server
            Log.Information("--- Creating Phoenix Manifest Of Databases and Sites ---");
            if (File.Exists("\\\\" + cmpserver + "\\c$\\inetpub\\MgmtSvc-CmpWapExtension\\web.config")){
                Log.Information("CMPWAPExtension web.config file found on " + cmpserver);
                try
                {   // Read the SQL database names using an XMLReader
                    XmlReader xmlReader = XmlReader.Create("\\\\" + cmpserver + "\\c$\\inetpub\\MgmtSvc-CmpWapExtension\\web.config");
                    List<string> connections = new List<string>();

                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            //Log.Information(xmlReader.Name);
                            if (xmlReader.Name == "add")
                            {
                                while (xmlReader.MoveToNextAttribute())
                                {
                                    if (xmlReader.Name == "name")
                                    {
                                        if (xmlReader.Value == "CMPContext" || xmlReader.Value == "MicrosoftMgmtSvcCmpContext" || xmlReader.Value == "MicrosoftMgmtSvcStoreContext")
                                        {
                                            //Log.Information(xmlReader.Value);
                                            //grab the database connections strings
                                            string connection = xmlReader.GetAttribute("connectionString");
                                            if (connection != null && connection.Count() > 0)
                                            {
                                                if (connection.Contains("cmp_0") && connection.Contains("Password=;")) { 
                                                    encrypted = true;
                                                }
                                                connections.Add(connection);
                                            }
                                        }
                                    }
                                    else if (xmlReader.Name == "key" && encrypted)
                                    {
                                        if (xmlReader.Value == "KryptoCert")
                                        {
                                            Log.Information("Found the KryptoCert in web.config on "+cmpserver+".");
                                            
                                            //Get the strings needed to initialize the Krypto object
                                            kryptoString = xmlReader.GetAttribute("value");
                                            kryptoParts = kryptoString.Split(',');
                                           
                                            //NOTE: x.509 will only grab certificate info from the local machine this code runs on.  
                                            //If this code is run remotely to test a machine that has encrypted passwords in its connectionstrings,
                                            //the machine it is run from will need a copy of the same certificate installed locally.

                                            //initialize the Krypto object
                                            try
                                            {
                                                Krypto = new X509Krypto(kryptoParts[1], kryptoParts[0], kryptoParts[2]);
                                                Log.Information("Retrieved local instance of CMPWAPExtension security certificate.");
                                            }
                                            catch (Exception e)
                                            {
                                                string eMessage = e.Message.ToString();
                                                if (eMessage.Contains("Requested value") && eMessage.Contains("was not found."))
                                                {
                                                    Log.Information("EXCEPTION: The security certificate could be found locally on the machine the Installation Test is running from.");
                                                    Log.Information("EXCEPTION: Please install a copy of the security certificate, used to encrypt the CMPWAPExtension connection strings,");
                                                    Log.Information("EXCEPTION: on the local server.  Security Certificates for the CMPWAPExtension machine cannot be accessed remotely.");
                                                }
                                                Failed();
                                            }
                                        }
                                        else if (xmlReader.Value == "CMPContextPassword" || xmlReader.Value == "MicrosoftMgmtSvcCmpContextPassword" || xmlReader.Value == "MicrosoftMgmtSvcStoreContextPassword")
                                        {
                                            //get the Encrypted Password and Decrypt it
                                            encryptedPasswordString = xmlReader.GetAttribute("value");
                                            passwordString = Krypto.DecrpytKText(encryptedPasswordString);
                                            dbString = xmlReader.Value.Replace("Password", "");

                                            //look through the connections strings and fill in the password.
                                            kryptoParts = kryptoString.Split(',');
                                            Krypto = new X509Krypto(kryptoParts[1], kryptoParts[0], kryptoParts[2]); 


                                            for (int connectionCount = 0; connectionCount <= connections.Count()-1; connectionCount++)
                                            {
                                                switch (dbString)
                                                {
                                                    case "CMPContext":
                                                        if (connections[connectionCount].Contains("CMP_DB")) { connections[connectionCount]=connections[connectionCount].Replace("Password=;","Password="+passwordString+";"); }
                                                        break;
                                                    case "MicrosoftMgmtSvcCmpContext":
                                                        if (connections[connectionCount].Contains("CMPWAP_DB")) { connections[connectionCount] = connections[connectionCount].Replace("Password=;", "Password=" + passwordString + ";"); }
                                                        break;
                                                    case "MicrosoftMgmtSvcStoreContext":
                                                        if (connections[connectionCount].Contains("Microsoft.MgmtSvc.Store")) { connections[connectionCount] = connections[connectionCount].Replace("Password=;", "Password=" + passwordString + ";"); }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                xmlReader.MoveToElement();
                            }
                        }
                    }
                    if (connections.Count < 3)
                    {
                        Log.Information("There are less than 3 connections strings in the local web.config file - The webconfig should have 3 SQL connection strings.");
                        Failed();
                    }
                    else
                    {
                        foreach (string connectstr in connections)
                        {
                            Log.Information(parseConnection(connectstr));
                            foreach(appDatabase Database in appDatabases){
                                if (Database.databaseName == "microsoft.mgmtsvc.store")
                                {
                                    //Make a connection to the database so we can read site information from the tables
                                    try
                                    {
                                        //All of these functions are essentially the same, mroe time to refactor could turn it into just one function, and get rid of the redundancy
                                        loadPortalDBSiteData(Database);
                                        loadStoreAPISiteData(Database);
                                        loadResourceProviderSiteDate(Database);
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Information("Exception:"+e.Message.ToString());
                                        Failed();
                                    }
                                }
                            }
                        }
                        Log.Information("** Database and Site Manifest **");
                        foreach (appDatabase TempDB in appDatabases)
                        {
                            //this may seem dumb to have ifs for each database and a boolean for each one, indicating encryption, but I want to still work even if 
                            //if the strings are mixed, encrypted or not.

                            if (encrypted)
                            {
                                Log.Information("server:" + TempDB.serverName + ",DB:" + TempDB.databaseName + ",User:" + TempDB.userid + ",Password:[encrypted]");
                            }
                            else
                            {
                                Log.Information("server:" + TempDB.serverName + ",DB:" + TempDB.databaseName + ",User:" + TempDB.userid + ",Password:" + TempDB.password);
                            }
                        }
                        foreach (appSite TempSite in appSites)
                        {
                            Log.Information("server:" + TempSite.serverName + ",app:" + TempSite.appName);
                        }
                        Log.Information("**      END-OF-MANIFEST       **");
                        Passed();
                    }
                }
                catch (Exception e)
                {
                    Log.Information("The file could not be read:" + e.Message.ToString());
                    Failed();
                }
            }
            else
            {
                Log.Information("The server: " + cmpserver + " - Cannot be found or does not exist.");
                Failed();
            }
            Finished();
        }



    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace WebHostBasicAuth.Modules
{
    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public class BasicCreds
    {
        private string _NamePassword = null;
        private string _Name = null;
        private string _Password = null;

        /// <summary> </summary>
        public string Name { get { return _Name; } }
        /// <summary> </summary>
        public string Password { get { return _Password; } }
        /// <summary> </summary>
        public string NamePassword { get { return _NamePassword; } }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// 
        //*********************************************************************

        public BasicCreds(string name, string password)
        {
            if (null == name)
                throw new Exception("'name' argument is null");

            if (null == password)
                throw new Exception("'password' argument is null");

            _Name = name;
            _Password = password;
            _NamePassword = name + "\\" + password;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePassword"></param>
        /// 
        //*********************************************************************

        public BasicCreds(string namePassword)
        {
            if (null == namePassword)
                throw new Exception("'namePassword' argument is null");

            var NP = namePassword.Split('\\');

            if (2 != namePassword.Length)
                throw new Exception("'namePassword' argument not in <name>\\<password> format");

            _Name = NP[0];
            _Password = NP[1];
            _NamePassword = namePassword;
        }
    }

    //*********************************************************************
    ///
    /// <summary>
    /// Handles BasicAuth creds
    /// http://www.asp.net/web-api/overview/security/basic-authentication
    /// </summary>
    /// 
    //*********************************************************************

    public class BasicAuthHttpModule : IHttpModule
    {
        private const string Realm = "CMP";
        public static List<BasicCreds> UserList = null;
        private string _cmpDbConnectionString;
        private EventLog _eventLog;

        #region Init

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// 
        //*********************************************************************

        public void Init(HttpApplication context)
        {
            //UserList = GetUserList();

            InitEventlog();
            GetDbConnectionString();
            FetchUserAccounts();

            // Register event handlers
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void InitEventlog()
        {
            try
            {
                _eventLog = new EventLog("Application")
                {
                    Source = CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName
                };
                _eventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
            }
            catch (Exception)
            {
                _eventLog = null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void GetDbConnectionString()
        {
            try
            {
                var xk = new KryptoLib.X509Krypto(null);
                _cmpDbConnectionString = xk.GetKTextConnectionString(
                    "CMPContext", "CMPContextPassword");

                if (null == _cmpDbConnectionString)
                {
                    if (null != _eventLog)
                        _eventLog.WriteEntry(
                            "Startup Exception: Unable to get DB connection string. Check the .config file 'CMPContext' and 'CMPContextPassword' keys",
                            EventLogEntryType.Error, 10, 10);
                    return;
                }

                if (null != _eventLog)
                    _eventLog.WriteEntry("CMP DB Connection string : " +
                        GetClippenConnectionString(_cmpDbConnectionString),
                        EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry(
                        "Startup Exception: Exception in GetDbConnectionString() CMP : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
                        EventLogEntryType.Error, 10, 10);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchUserAccounts()
        {
            try
            {
                var cs = new CmpServiceLib.CmpService(
                    _eventLog, _cmpDbConnectionString, null);

                var userAcountList = cs.FetchUserAccounts();

                if (null == userAcountList)
                {
                    userAcountList = WebHostBasicAuth.Modules.BasicAuthHttpModule.GetDefaultAccountList();

                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using default service user account.",
                            EventLogEntryType.Information, 1, 1);
                }
                else if (0 == userAcountList.Count)
                {
                    userAcountList = WebHostBasicAuth.Modules.BasicAuthHttpModule.GetDefaultAccountList();

                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using default service user account.",
                            EventLogEntryType.Information, 1, 1);
                }
                else
                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using DB source service user accounts.",
                            EventLogEntryType.Information, 1, 1);

                WebHostBasicAuth.Modules.BasicAuthHttpModule.UserList =
                    new List<WebHostBasicAuth.Modules.BasicCreds>();

                foreach (var UA in userAcountList)
                    WebHostBasicAuth.Modules.BasicAuthHttpModule.UserList.Add(
                        new WebHostBasicAuth.Modules.BasicCreds(UA.Name, UA.Password));

                if (null != _eventLog)
                    _eventLog.WriteEntry("Service user accounts set OK",
                        EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry("Startup Exception: Unable to fetch CMP service user accounts : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
                        EventLogEntryType.Error, 10, 10);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        string GetClippenConnectionString(string connectionString)
        {
            string clippedString = null;

            try
            {
                var index = _cmpDbConnectionString.IndexOf("Password=");
                clippedString = _cmpDbConnectionString.Substring(0, index + 9) + "XXX";
                var index2 = _cmpDbConnectionString.Substring(index).IndexOf(";");
                clippedString += _cmpDbConnectionString.Substring(index + index2);
            }
            catch (Exception)
            {
                clippedString = "[MALFORMED]";
            }

            return clippedString;
        }

        #endregion

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static List<BasicCreds> GetUserList()
        {
            if (null == UserList)
                UserList = SetDefaultAccountList();

            return UserList;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// 
        //*********************************************************************

        private static void SetPrincipal(System.Security.Principal.IPrincipal principal)
        {
            System.Threading.Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Here is where you would validate the username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool IsAuthorized(string username, string password)
        {
            GetUserList();

            var namePassword = username + "\\" + password;

            return UserList.Any(user => user.NamePassword.Equals(namePassword));
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientCert"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool IsAuthorized(HttpClientCertificate clientCert)
        {
            return IsAuthorized(clientCert.Subject, clientCert.SerialNumber);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool AuthenticateUser(string credentials)
        {
            var validated = false;
            try
            {
                var encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                var separator = credentials.IndexOf(':');
                var name = credentials.Substring(0, separator);
                var password = credentials.Substring(separator + 1);

                validated = IsAuthorized(name, password);

                if (validated)
                {
                    var identity = new System.Security.Principal.GenericIdentity(name);
                    SetPrincipal(new System.Security.Principal.GenericPrincipal(identity, null));
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                validated = false;

            }
            return validated;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientCert"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool AuthenticateUser(HttpClientCertificate clientCert)
        {
            var validated = false;
            try
            {
                var name = clientCert.Subject;

                validated = IsAuthorized(clientCert);

                if (validated)
                {
                    var identity = new System.Security.Principal.GenericIdentity(name);
                    SetPrincipal(new System.Security.Principal.GenericPrincipal(identity, null));
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                validated = false;

            }
            return validated;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //*********************************************************************

        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;

            //*** Test ***
            //var identity = new System.Security.Principal.GenericIdentity("Bob");
            //SetPrincipal(new System.Security.Principal.GenericPrincipal(identity, null));

            if (0 < request.ClientCertificate.Subject.Length)
                AuthenticateUser(request.ClientCertificate);

            //*** Test ***

            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// If the request was unauthorized, add the WWW-Authenticate header 
        /// to the response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //*********************************************************************

        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        {
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static List<CmpServiceLib.Models.CmpServiceUserAccount> GetDefaultAccountList()
        {
            var ual = new List<CmpServiceLib.Models.CmpServiceUserAccount>(1);

            var xk = new KryptoLib.X509Krypto(null);

            var cmpServiceUserName =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("CmpServiceUserName") as string;
            var cmpServiceUserPassword =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("CmpServiceUserPassword") as string;

            if (null == cmpServiceUserName)
                return ual;
            if (null == cmpServiceUserPassword)
                return ual;

            cmpServiceUserPassword = xk.DecrpytKText(cmpServiceUserPassword);

            ual.Add(new CmpServiceLib.Models.CmpServiceUserAccount() 
                { Name = cmpServiceUserName, Password = cmpServiceUserPassword });

            return ual;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static List<BasicCreds> SetDefaultAccountList()
        {
            if (null == UserList)
                UserList = new List<BasicCreds>(1);

            var xk = new KryptoLib.X509Krypto(null);

            var cmpServiceUserName =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("CmpServiceUserName") as string;
            var cmpServiceUserPassword =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("CmpServiceUserPassword") as string;

            if (null == cmpServiceUserName)
                return UserList;
            if (null == cmpServiceUserPassword)
                return UserList;

            cmpServiceUserPassword = xk.DecrpytKText(cmpServiceUserPassword);

            UserList.Add(new BasicCreds(cmpServiceUserName, cmpServiceUserPassword));

            return UserList;
        }
    }
}
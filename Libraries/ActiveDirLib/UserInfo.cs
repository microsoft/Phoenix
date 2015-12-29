using System;
using System.Security.Principal;

namespace ActiveDirLib
{
    public class UserInfo
    {
        public static string GetUserEmail(WindowsIdentity UserIdentity)
        {
            string tempCurrentUserEmail = null;
            var UserName = UserIdentity.Name;

            UserName = UserName.Substring(UserName.IndexOf("\\") + 1);
            var Entry = new System.DirectoryServices.DirectoryEntry("LDAP://RootDSE");
            var sFQDN = System.Convert.ToString(Entry.Properties["defaultNamingContext"].Value);
            var myDE = new System.DirectoryServices.DirectoryEntry("LDAP://" + sFQDN);

            var mySearcher = new System.DirectoryServices.DirectorySearcher(myDE);

            mySearcher.Filter = "sAMAccountName=" + UserName;
            mySearcher.PropertiesToLoad.Add("Mail");
            try
            {
                var myresult = mySearcher.FindOne();
                tempCurrentUserEmail = System.Convert.ToString(myresult.Properties["Mail"][0]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not establish an email address for user '" + UserName + "' : " + ex.Message);
            }

            return tempCurrentUserEmail;
        }
    }
}

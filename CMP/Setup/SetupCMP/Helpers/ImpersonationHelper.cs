using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace CMP.Setup.Helpers
{
    class ImpersonationHelper : IDisposable
    {
        private WindowsImpersonationContext windowsImpersonationContext = null;
        private IntPtr tokenHandle = CMP.Setup.Helpers.NativeMethods.NullIntPtr;
        private bool logonSuccessful = false;
        private IntPtr processHandle = CMP.Setup.Helpers.NativeMethods.NullIntPtr;

        public ImpersonationHelper()
        {
            bool isDatabaseImpersonation = SetupInputs.Instance.FindItem(SetupInputTags.RemoteDatabaseImpersonationTag);
            if (isDatabaseImpersonation)
            {
                String userName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminNameTag);
                String domain = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminDomainTag);
                InputParameter pwd = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminPasswordTag);
                this.Impersonate(userName, domain, pwd);
            }
            else
            {
                this.windowsImpersonationContext = null;
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool RevertToSelf();

        public void Dispose()
        {
            if (this.windowsImpersonationContext != null)
            {
                // Revert the user context to the windows user represented by this object
                this.windowsImpersonationContext.Undo();
            }
            if (!CMP.Setup.Helpers.NativeMethods.NullIntPtr.Equals(this.tokenHandle))
            {
                CMP.Setup.Helpers.UnsafeNativeMethods.CloseHandle(this.tokenHandle);
            }
            if (!CMP.Setup.Helpers.NativeMethods.NullIntPtr.Equals(this.processHandle))
            {
                CMP.Setup.Helpers.UnsafeNativeMethods.CloseHandle(this.processHandle);
                if (!RevertToSelf())
                {
                    int winError = Marshal.GetLastWin32Error();
                    throw new Exception("The remote SQL Server administrator credentials provided are not valid");
                }
            }
            SetupLogger.LogInfo("Out of Impersonation");
        }

        private void Impersonate(string userName, string domain, InputParameter pwd)
        {
            string userAccountName = String.Format(SetupConstants.UserAccountTemplate, domain, userName);
            SetupLogger.LogInfo("We are going to impersonate as {0}.", userAccountName);


            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            this.tokenHandle = CMP.Setup.Helpers.NativeMethods.NullIntPtr;

            IntPtr password = IntPtr.Zero;

            try
            {
                if (pwd != null)
                {
                    password = Marshal.SecureStringToGlobalAllocUnicode(pwd);
                }

                logonSuccessful = CMP.Setup.Helpers.UnsafeNativeMethods.LogonUser(
                    userName, domain, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref this.tokenHandle);

                // LogonUser() if successful will return a non-zero value else it will return a 0
                if (logonSuccessful)
                {
                    WindowsIdentity windowsIdentity = new WindowsIdentity(this.tokenHandle);
                    this.windowsImpersonationContext = windowsIdentity.Impersonate();
                }
                else
                {
                    // LogonUser has failed so GetLastWin32Error() and throw an exception
                    int winError = Marshal.GetLastWin32Error();
                    throw new Exception("The remote SQL Server administrator credentials provided are not valid");
                }
            }
            catch (ArgumentException)
            {
                throw new Exception("The remote SQL Server administrator credentials provided are not valid");
            }
            catch (SecurityException)
            {
                // Constructor WindowsIdentity(this.tokenHandle) can throw this exception
                // This can mean either The caller does not have the correct permissions OR A Win32 error occured

                // WindowsIdentity.Impersonate() can also throw this exception
                // This can mean that A Win32 error occured
                throw new Exception("The remote SQL Server administrator credentials provided are not valid");
            }
            catch (InvalidOperationException)
            {
                // WindowsIdentity.Impersonate() can throw this exception
                // This can mean that An anonymous identity attempted to perform an impersonation
                throw new Exception("The remote SQL Server administrator credentials provided are not valid");
            }
            finally
            {
                if (IntPtr.Zero != password)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(password);
                }
            }
        }

    }
}

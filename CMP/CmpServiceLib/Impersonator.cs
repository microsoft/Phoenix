//*****************************************************************************
//*
//* File:
//* Author: Mark West (mark.west@microsoft.com)
//* Copyright: Microsoft 2011
//*
//*****************************************************************************

using System;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace CmpServiceLib
{
    //*************************************************************************
    ///
    /// <summary>
    /// Impersonation of a user. Allows to execute code under another
    /// user context.
    /// Please note that the account that instantiates the Impersonator class
    /// needs to have the 'Act as part of operating system' privilege set.
    /// </summary>
    /// <remarks>	
    /// This class is based on the information in the Microsoft knowledge base
    /// article http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158
    /// 
    /// Encapsulate an instance into a using-directive like e.g.:
    /// 
    ///		...
    ///		using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
    ///		{
    ///			...
    ///			[code that executes under the new context]
    ///			...
    ///		}
    ///		...
    /// 
    /// </remarks>
    /// 
    //*************************************************************************

    public class Impersonator :
        IDisposable
    {
        /// <summary> </summary>
        public const int EXCEPTION_Impersonator_ImpersonateValidUser_failed = 1;

        #region Public methods.
        //*********************************************************************
        ///
        /// <summary>
        /// Constructor. Starts the impersonation with the given credentials.
        /// Please note that the account that instantiates the Impersonator class
        /// needs to have the 'Act as part of operating system' privilege set.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domainName">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        /// 
        //*********************************************************************

        public Impersonator( string userName, string domainName, string password)
        {
            ImpersonateValidUser(userName, domainName, password);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Tests the creds.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password"></param>
        /// <returns>True if valid, false if invalid</returns>
        /// 
        //*********************************************************************

        public static bool TestCreds(string name, string domain, string password)
        {
            if (null == name | null == domain | null == password)
                return false;

            try
            {
                using (new Impersonator(name, domain, password))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        // ------------------------------------------------------------------
        #endregion

        #region IDisposable member.
        // ------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            UndoImpersonation();
        }

        // ------------------------------------------------------------------
        #endregion

        #region P/Invoke.
        // ------------------------------------------------------------------

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(
            IntPtr handle);

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        // ------------------------------------------------------------------
        #endregion

        #region Private member.
        // ------------------------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// Does the actual impersonation.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domain">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        /// 
        //*********************************************************************

        private void ImpersonateValidUser( string userName, string domain, string password)
        {
            WindowsIdentity tempWindowsIdentity = null;
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;
            string Message;

            try
            {
                if (RevertToSelf())
                {
                    if (LogonUser( userName, domain, password,
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                        else
                        {
                            Message = string.Format("Unable to impersonate user '{0}/{1}'. User name and/or password may be invalid. Impersonator.ImpersonateValidUser() code: '{2}'",
                                domain, userName, Marshal.GetLastWin32Error());
                            throw new ImpersonatorException(Message, EXCEPTION_Impersonator_ImpersonateValidUser_failed);
                        }
                    }
                    else
                    {
                        Message = string.Format("Unable to impersonate user '{0}/{1}'. User name and/or password may be invalid. Impersonator.ImpersonateValidUser() code: '{2}'",
                            domain, userName, Marshal.GetLastWin32Error());
                        throw new ImpersonatorException(Message, EXCEPTION_Impersonator_ImpersonateValidUser_failed);
                    }
                }
                else
                {
                    Message = string.Format("Unable to impersonate user '{0}/{1}'. User name and/or password may be invalid. Impersonator.ImpersonateValidUser() code: '{2}'",
                        domain, userName, Marshal.GetLastWin32Error());
                    throw new ImpersonatorException(Message, EXCEPTION_Impersonator_ImpersonateValidUser_failed);
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    CloseHandle(tokenDuplicate);
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Reverts the impersonation.
        /// </summary>
        /// 
        //*********************************************************************

        private void UndoImpersonation()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }
        }

        private WindowsImpersonationContext impersonationContext = null;

        // ------------------------------------------------------------------
        #endregion
    }

    //*************************************************************************
    ///
    /// <summary>
    /// Thrown by members of the SamDbInterface class
    /// </summary>
    /// 
    //*************************************************************************
    [Serializable()]
    public class ImpersonatorException : Exception
    {
        /// <summary>
        /// Thrown by members of the SamAppInfoException class
        /// </summary>
        public ImpersonatorException()
        {
        }

        /// <summary>
        /// Thrown by members of the SamAppInfoException class
        /// </summary>
        /// <param name="message">Human readable text specific to this exception</param>
        /// <param name="hResult"></param>
        public ImpersonatorException(string message, int hResult)
            : base(message)
        {
            base.Data.Add("HRESULT", hResult); base.Data.Add("TYPE", this.GetType().Name);
        }

        /// <summary>
        /// Thrown by members of the SamAppInfoException class
        /// </summary>
        /// <param name="message">Human readable text specific to this exception</param>
        /// <param name="hResult"></param>
        /// <param name="inner">The inner exception if available</param>
        public ImpersonatorException(string message, int hResult, Exception inner)
            : base(message, inner)
        {
            base.Data.Add("HRESULT", hResult); base.Data.Add("TYPE", this.GetType().Name);
        }
    }
}

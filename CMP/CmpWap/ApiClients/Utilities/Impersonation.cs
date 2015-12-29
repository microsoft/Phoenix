//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Impersonation Class
    /// </summary>
    public static class Impersonation
    {
        /// <summary>
        /// Impersonate main function. begins Impersonation context
        /// </summary>
        /// <param name="domain">domain name of the principal you wish to impersonate</param>
        /// <param name="username">username of the principal you wish to impersonate</param>
        /// <param name="password">password of the principal you wish to impersonate</param>
        /// <param name="handler">delegate Action</param>
        public static void Impersonate(string domain, string username, string password, Action handler)
        {
            SafeTokenHandle safeHandle;
            var result = NativeMethods.LogonUser(
                username,
                domain == null ? "." : domain,
                password,
                NativeMethods.LogonType.LOGON32_LOGON_NEW_CREDENTIALS,
                NativeMethods.LogonProvider.LOGON32_PROVIDER_WINNT50,
                out safeHandle);

            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            using (safeHandle)
            {
                using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeHandle.DangerousGetHandle()))
                {
                    handler();
                }
            }
        }

        private static class NativeMethods
        {
            /// <summary>
            /// Indicates the type of Logon
            /// </summary>
            public enum LogonType
            {
                /// <summary>
                /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on  
                /// by a terminal server, remote shell, or similar process.
                /// This logon type has the additional expense of caching logon information for disconnected operations; 
                /// therefore, it is inappropriate for some client/server applications,
                /// such as a mail server.
                /// </summary>
                LOGON32_LOGON_INTERACTIVE = 2,

                /// <summary>
                /// This logon type is intended for high performance servers to authenticate plaintext passwords.
                /// The LogonUser function does not cache credentials for this logon type.
                /// </summary>
                LOGON32_LOGON_NETWORK = 3,

                /// <summary>
                /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without 
                /// their direct intervention. This type is also for higher performance servers that process many plaintext
                /// authentication attempts at a time, such as mail or Web servers. 
                /// The LogonUser function does not cache credentials for this logon type.
                /// </summary>
                LOGON32_LOGON_BATCH = 4,

                /// <summary>
                /// Indicates a service-type logon. The account provided must have the service privilege enabled. 
                /// </summary>
                LOGON32_LOGON_SERVICE = 5,

                /// <summary>
                /// This logon type is for GINA DLLs that log on users who will be interactively using the computer. 
                /// This logon type can generate a unique audit record that shows when the workstation was unlocked. 
                /// </summary>
                LOGON32_LOGON_UNLOCK = 7,

                /// <summary>
                /// This logon type preserves the name and password in the authentication package, which allows the server to make 
                /// connections to other network servers while impersonating the client. A server can accept plaintext credentials 
                /// from a client, call LogonUser, verify that the user can access the system across the network, and still 
                /// communicate with other servers.
                /// NOTE: Windows NT:  This value is not supported. 
                /// </summary>
                LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

                /// <summary>
                /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
                /// The new logon session has the same local identifier but uses different credentials for other network connections. 
                /// NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
                /// NOTE: Windows NT:  This value is not supported. 
                /// </summary>
                LOGON32_LOGON_NEW_CREDENTIALS = 9,
            }

            /// <summary>
            /// Indicates the Logon Provider
            /// </summary>
            public enum LogonProvider
            {
                /// <summary>
                /// Use the standard logon provider for the system. 
                /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
                /// is not in UPN format. In this case, the default provider is NTLM. 
                /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
                /// </summary>
                LOGON32_PROVIDER_DEFAULT = 0,

                /// <summary>
                /// Win NT 3.5
                /// </summary>
                LOGON32_PROVIDER_WINNT35 = 1,

                /// <summary>
                /// Win NT 4.0
                /// </summary>
                LOGON32_PROVIDER_WINNT40 = 2,

                /// <summary>
                /// Win NT 5.0
                /// </summary>
                LOGON32_PROVIDER_WINNT50 = 3
            }

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool LogonUser(
                string username,
                string domain,
                string password,
                LogonType logonType,
                LogonProvider logonProvider,
                out SafeTokenHandle token);

            /// <summary>
            /// Closes Handle
            /// </summary>
            /// <param name="handle">represents a pointer or a handle</param>
            /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr handle);
        }

        /// <summary>
        /// SafeToken Handle for working with Impersonation Context
        /// </summary>
        private sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeTokenHandle()
                : base(true)
            {
            }

            /// <summary>
            /// Method to release Handle
            /// </summary>
            /// <returns>true if it successfully closes the handle. False otherwise.</returns>
            protected override bool ReleaseHandle()
            {
                return NativeMethods.CloseHandle(handle);
            }
        }
    }
}
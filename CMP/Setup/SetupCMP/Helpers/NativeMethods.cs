using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CMP.Setup.Helpers
{
    class NativeMethods
    {
        /// <summary>
        /// The CloseServiceHandle function closes a handle to a service control manager or service object.
        /// </summary>
        /// <param name="handle">Handle to the service control manager object or the service object to close.</param>
        /// <returns>true if success, false if fails</returns>
        /// <remarks>Component : Setup (kushaln)</remarks>
        [DllImport("Advapi32.dll", EntryPoint = "CloseServiceHandle", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseServiceHandle(IntPtr handle);

        /// <summary>
        /// The QueryServiceConfig function retrieves the configuration parameters of the specified service.
        /// </summary>
        /// <param name="hService">Handle to the service</param>
        /// <param name="lpServiceConfig">Pointer to a buffer that receives the service configuration information</param>
        /// <param name="cbBufSize">Size of the buffer pointed to by the lpServiceConfig parameter, in bytes</param>
        /// <param name="pcbBytesNeeded">Pointer to a variable that receives the number of bytes needed to return all the configuration information </param>
        /// <returns>true if succeeds, false if fails</returns>
        /// <remarks>Component : Setup (kushaln)</remarks>
        [DllImport("Advapi32.dll", EntryPoint = "QueryServiceConfigW", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool QueryServiceConfig(IntPtr hService,
            ref QUERY_SERVICE_CONFIGW lpServiceConfig,
            UInt32 cbBufSize,
            ref UInt32 pcbBytesNeeded
            );

        /// <summary>
        /// Open Service Control Manager database
        /// </summary>
        /// <param name="lpMachineName">name of machine on which service is installed</param>
        /// <param name="lpDatabaseName">name of database</param>
        /// <param name="dwDesiredAccess">access flags as desired</param>
        /// <returns>IntPtr : handle to SCM</returns>
        /// <remarks>Component : Setup (kushaln)</remarks>
        [DllImport("Advapi32.dll", EntryPoint = "OpenSCManagerW", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenSCManager(String lpMachineName,
            String lpDatabaseName,
            int dwDesiredAccess);

        /// <summary>
        /// Open an existing service
        /// </summary>
        /// <param name="hSCManager">handle to SCM database (previous call to OpenSCManager)</param>
        /// <param name="lpServiceName">service name</param>
        /// <param name="dwDesiredAccess">access flags as desired</param>
        /// <returns>IntPtr : handle to the service</returns>
        /// <remarks>Component : Setup (kushaln)</remarks>
        [DllImport("Advapi32.dll", EntryPoint = "OpenServiceW", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenService(IntPtr hSCManager,
            String lpServiceName,
            int dwDesiredAccess);

        ///<summary>
        ///The DeleteService function provides an API for deleting a service.
        ///</summary>
        ///<param name="handle">Handle of service to be deleted. Must have DELETE rights</param>
        ///<returns>true if service was successfully deleted, false otherwise</returns>
        ///<remarks>
        ///Use Marshal.GetWin32LastError() to get errors in this call.
        ///Component: Setup (prkumar)
        ///</remarks>
        [DllImport("Advapi32.dll", EntryPoint = "DeleteService", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool DeleteService(IntPtr handle);


        /// <summary>
        ///ChangeServiceConfig for disabling/enabling the service
        /// </summary>
        /// <param name="hService"></param>
        /// <param name="dwServiceType"></param>
        /// <param name="dwStartType"></param>
        /// <param name="dwErrorControl"></param>
        /// <param name="lpBinaryPathName"></param>
        /// <param name="lpLoadOrderGroup"></param>
        /// <param name="lpdwTagId"></param>
        /// <param name="lpDependencies"></param>
        /// <param name="lpServiceStartName">Always pass null</param>
        /// <param name="lpPassword">Always pass new IntPtr(SERVICE_NO_CHANGE)</param>
        /// <param name="lpDisplayName"></param>
        /// <returns></returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool ChangeServiceConfig(IntPtr hService,
            int dwServiceType,
            int dwStartType,
            int dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName
            );


        /// <summary>
        /// Required to call the LockServiceDatabase function to acquire a lock on the database. (Passed to OpenSCManager)
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const short SC_MANAGER_LOCK = 0x0008;

        /// <summary>
        /// Required to call the CreateService function to create a service object and add it to the database. (Passed to OpenSCManager)
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const short SC_MANAGER_CREATE_SERVICE = 0x0002;

        /// <summary>
        /// standard access rights (Passed to OpenSCManager)
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;

        /// <summary>
        /// Required to connect to the service control manager (Passed to OpenSCManager)
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const short SC_MANAGER_CONNECT = 0x0001;

        /// <summary>
        /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int SERVICE_DISABLED = 0x00000004;

        /// <summary>
        /// A service started by the service control manager when a process calls the StartService function.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int SERVICE_DEMAND_START = 0x00000003;

        /// <summary>
        /// A service that has its own process
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        public const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;

        /// <summary>
        ///Constant specifying normal error control for the service
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>

        public const int SERVICE_ERROR_NORMAL = 0x00000001;

        /// <summary>
        /// Constant specifying if the service is interactive
        /// </summary>
        public const int SERVICE_INTERACTIVE_PROCESS = 0x00000100;

        /// <summary>
        /// A service started automatically by the service control manager during system startup.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int SERVICE_AUTO_START = 0x00000002;

        /// <summary>
        /// The QUERY_SERVICE_CONFIGW structure is used by the QueryServiceConfig function to return
        /// configuration information about an installed service.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192), CLSCompliant(false), ComVisible(false)]
        public struct QUERY_SERVICE_CONFIGW
        {
            /// <summary>
            /// Type of service.
            /// </summary>
            public UInt32 dwServiceType;

            /// <summary>
            /// When to start the service.
            /// </summary>
            public UInt32 dwStartType;

            /// <summary>
            /// Severity of the error, and action taken, if this service fails to start.
            /// </summary>
            public UInt32 dwErrorControl;

            /// <summary>
            /// Pointer to a null-terminated string that contains the fully qualified path to the service binary file.
            /// </summary>
            public IntPtr lpBinaryPathName;

            /// <summary>
            /// Pointer to a null-terminated string that names the load ordering group to which this service belongs.
            /// If the member is NULL or an empty string, the service does not belong to a load ordering group.
            /// </summary>
            public IntPtr lpLoadOrderGroup;

            /// <summary>
            /// Unique tag value for this service in the group specified by the lpLoadOrderGroup parameter.
            /// </summary>
            public UInt32 dwTagId;

            /// <summary>
            /// Pointer to an array of null-separated names of services or load ordering groups that must start before this service.
            /// </summary>
            public IntPtr lpDependencies;

            /// <summary>
            /// Pointer to a null-terminated string that is the account under whose credentials the service runs
            /// </summary>
            public IntPtr lpServiceStartName;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the display name to be used by service control programs to identify the service.
            /// </summary>
            public IntPtr lpDisplayName;
        };

        /// <summary>
        /// Used by ChangeServiceConfig2 to set the description of the service.
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192), CLSCompliant(false), ComVisible(false)]
        public struct SERVICE_DESCRIPTION
        {
            public String lpDescription;
        };

        public const int SERVICE_CONFIG_DESCRIPTION = 1,
            SERVICE_CONFIG_FAILURE_ACTIONS = 2,
            SERVICE_CONFIG_SERVICE_SID_INFO = 5;

        /// <summary>
        ///Constants specifying the various service access rights.
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        public const int SERVICE_QUERY_CONFIG = 0x0001;
        public const int SERVICE_CHANGE_CONFIG = 0x0002;
        public const int SERVICE_QUERY_STATUS = 0x0004;
        public const int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        public const int SERVICE_START = 0x0010;
        public const int SERVICE_STOP = 0x0020;
        public const int SERVICE_PAUSE_CONTINUE = 0x0040;
        public const int SERVICE_INTERROGATE = 0x0080;
        public const int SERVICE_USER_DEFINED_CONTROL = 0x0100;

        public const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);

        /// <summary>
        /// Constant specifying that the service configuration has not changed
        /// </summary>
        public const int SERVICE_NO_CHANGE = unchecked((int)0xFfffffff);

        /// <summary>
        ///Delete constant for deleting a service access. Passed to OpenService
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        public const int DELETE = (0x00010000);

        /// <summary>
        /// For ChangeServiceConfig2
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        [CLSCompliant(false), ComVisible(false)]
        public enum SC_ACTION_TYPE
        {
            SC_ACTION_NONE = 0,
            SC_ACTION_RESTART = 1,
            SC_ACTION_REBOOT = 2,
            SC_ACTION_RUN_COMMAND = 3
        };

        /// <summary>
        /// For ChangeServiceConfig2 for setting SID type for the service
        /// </summary>
        [CLSCompliant(false), ComVisible(false)]
        public enum SERVICE_SID_TYPE
        {
            SERVICE_SID_TYPE_NONE = 0,
            SERVICE_SID_TYPE_UNRESTRICTED = 1,
            SERVICE_SID_TYPE_RESTRICTED = 3
        };

        /// <summary>
        /// Used by ChangeServiceConfig2 to set the failure recovery parameters of the service.
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192), CLSCompliant(false), ComVisible(false)]
        public struct SC_ACTION
        {
            public SC_ACTION_TYPE Type;
            public int Delay;
        };

        /// <summary>
        /// Used by ChangeServiceConfig2 to set the SID type of the service.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192), CLSCompliant(false), ComVisible(false)]
        public struct SERVICE_SID_INFO
        {
            public SERVICE_SID_TYPE serviceSidType;
        };

        /// <summary>
        /// Used by ChangeServiceConfig2 to set the failure recovery parameters of the service.
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192), CLSCompliant(false), ComVisible(false)]
        public struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;
            public String lpRebootMsg;
            public String lpCommand;
            public int cActions;
            public IntPtr sc_Action;
        };

        /// <summary>
        /// The service is disabled
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        public const int ERROR_SERVICE_DISABLED = 1058;
        /// <summary>
        /// The specified service does not exist as an installed service.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

        /// <summary>
        /// The specified service is marked for deletion.
        /// </summary>
        /// <remarks>Component : Setup (prkumar)</remarks>
        public const int ERROR_SERVICE_MARKED_FOR_DELETE = 1072;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto,
    SetLastError = true, PreserveSig = true)]
        private static extern bool LookupAccountName(
            string lpSystemName, string lpAccountName,
            IntPtr psid, ref int cbsid,
            StringBuilder domainName, ref int cbdomainLength,
            ref int use);
        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle);
        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern long LsaRemoveAccountRights(
            IntPtr PolicyHandle, IntPtr AccountSid,
            Boolean AllRights,
            LSA_UNICODE_STRING[] UserRights,
            long CountOfRights);
        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern long LsaAddAccountRights(
            IntPtr PolicyHandle, IntPtr AccountSid,
            LSA_UNICODE_STRING[] UserRights,
            long CountOfRights);

        [DllImport("advapi32.dll")]
        private static extern long LsaNtStatusToWinError(long status);

        /// <summary>
        /// ChangeServiceConfig2 for setting the service description
        /// </summary>
        /// <param name="hService">Handle to the service</param>
        /// <param name="dwInfoLevel">Info level for service description (SERVICE_CONFIG_DESCRIPTION)</param>
        /// <param name="lpInfo"></param>
        /// <returns></returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool ChangeServiceConfig2(IntPtr hService,
            int dwInfoLevel,
            ref SERVICE_DESCRIPTION lpInfo
            );

        /// <summary>
        /// ChangeServiceConfig2 for setting the service failure recovery parameters
        /// </summary>
        /// <param name="hService">Handle to the service</param>
        /// <param name="dwInfoLevel">Info level for service description (SERVICE_CONFIG_FAILURE_ACTIONS)</param>
        /// <param name="lpInfo"></param>
        /// <returns>success=>true</returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool ChangeServiceConfig2(IntPtr hService,
            int dwInfoLevel,
            ref SERVICE_FAILURE_ACTIONS lpInfo
            );

        /// <summary>
        /// ChangeServiceConfig2 for setting the service SID type
        /// </summary>
        /// <param name="hService">Handle to the service</param>
        /// <param name="dwInfoLevel">Info level for service SID type (SERVICE_CONFIG_SERVICE_SID_INFO)</param>
        /// <param name="lpInfo"></param>
        /// <returns>success=>true</returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool ChangeServiceConfig2(IntPtr hService,
            int dwInfoLevel,
            ref SERVICE_SID_INFO lpInfo
            );

        ///<summary>
        ///The CreateService function provides an API for creating a service.
        ///</summary>
        ///<param name="hSCManager">Handle to SCM database with Create service authroization</param>
        ///<param name="lpServiceName">Name of the service to create</param>
        ///<param name="lpDisplayName">Name of service as required to be displayed in UI</param>
        ///<param name="dwDesiredAccess">The desired access on the newly created service</param>
        ///<param name="dwServiceType">The type of the service (OWN/SHARED PROC)</param>
        ///<param name="dwStartType">Start type of the service(AUTO/MANUAL ETC.)</param>
        ///<param name="dwErrorControl">Error type option for the service</param>
        ///<param name="lpBinaryPathName">Path to the binary for the service with arguments</param>
        ///<param name="lpLoadOrderGroup">Load order group</param>
        ///<param name="lpdwTagId">[out]Tag ID</param>
        ///<param name="lpDependencies">Dependencies of the service</param>
        ///<param name="lpServiceStartName">The username under which the service should be started</param>
        ///<param name="lpPassword">Password for the username of the service</param>
        ///<returns>Handle to the newly created service</returns>
        ///<remarks>
        ///Use Marshal.GetWin32LastError() to get errors in this call.
        ///Component: Setup (prkumar)
        ///</remarks>

        [DllImport("Advapi32.dll", EntryPoint = "CreateServiceW", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern IntPtr CreateService(IntPtr hSCManager,
            String lpServiceName,
            String lpDisplayName,
            int dwDesiredAccess,
            int dwServiceType,
            int dwStartType,
            int dwErrorControl,
            String lpBinaryPathName,
            String lpLoadOrderGroup,
            IntPtr lpdwTagId,
            String lpDependencies,
            String lpServiceStartName,
            [In] IntPtr lpPassword
            );

        /// <summary>
        /// LockServiceDatabase
        /// </summary>
        /// <param name="hSCManager">Handle to the service manager</param>
        /// <returns>Handle to the lock</returns>
        [DllImport("Advapi32.dll", EntryPoint = "LockServiceDatabase", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

        /// <summary>
        /// UnlockServiceDatabase
        /// </summary>
        /// <param name="ScLock">Handle to the database lock</param>
        /// <returns>success=>true</returns>
        [DllImport("Advapi32.dll", EntryPoint = "UnlockServiceDatabase", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall), CLSCompliant(false)]
        public static extern bool UnlockServiceDatabase(IntPtr ScLock);

        private enum LSA_AccessPolicy : long
        {
            POLICY_VIEW_LOCAL_INFORMATION = 0x00000001L,
            POLICY_VIEW_AUDIT_INFORMATION = 0x00000002L,
            POLICY_GET_PRIVATE_INFORMATION = 0x00000004L,
            POLICY_TRUST_ADMIN = 0x00000008L,
            POLICY_CREATE_ACCOUNT = 0x00000010L,
            POLICY_CREATE_SECRET = 0x00000020L,
            POLICY_CREATE_PRIVILEGE = 0x00000040L,
            POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080L,
            POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100L,
            POLICY_AUDIT_LOG_ADMIN = 0x00000200L,
            POLICY_SERVER_ADMIN = 0x00000400L,
            POLICY_LOOKUP_NAMES = 0x00000800L,
            POLICY_NOTIFICATION = 0x00001000L
        }
        [DllImport("advapi32.dll")]
        private static extern bool IsValidSid(IntPtr pSid);

        [DllImport("advapi32.dll")]
        internal static extern long LsaClose(IntPtr ObjectHandle);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("advapi32")]
        public static extern void FreeSid(IntPtr pSid);


        /// <summary>Adds a privilege to an account</summary>
        /// <param name="accountName">Name of an account - "domain\account" or only "account"</param>
        /// <param name="privilegeName">Name ofthe privilege</param>
        /// <param name="add">Add or Remove</param>
        /// <returns>The windows error code returned by LsaAddAccountRights</returns>
        public static long SetRight(String accountName, String privilegeName, bool add)
        {
            long winErrorCode = 0; //contains the last error
            //pointer an size for the SID
            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            //StringBuilder and size for the domain name
            StringBuilder domainName = new StringBuilder();
            int nameSize = 0;
            //account-type variable for lookup
            int accountType = 0;

            //get required buffer size
            LookupAccountName(String.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType);

            //allocate buffers
            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            //lookup the SID for the account
            bool result = LookupAccountName(String.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType);
            if (!result)
            {
                // Call GLE immediately to avoid contamination from other calls
                winErrorCode = GetLastError();
            }

            //say what you're doing
            SetupLogger.LogInfo("LookupAccountName result = " + result);
            SetupLogger.LogInfo("LookupAccountName domainName: " + domainName.ToString());

            if (!result)
            {
                SetupLogger.LogInfo("LookupAccountName failed: " + winErrorCode);
            }
            else
            {

                //initialize an empty unicode-string
                LSA_UNICODE_STRING systemName = new LSA_UNICODE_STRING();

                //combine all policies
                int access = (int)(
                    LSA_AccessPolicy.POLICY_AUDIT_LOG_ADMIN |
                    LSA_AccessPolicy.POLICY_CREATE_ACCOUNT |
                    LSA_AccessPolicy.POLICY_CREATE_PRIVILEGE |
                    LSA_AccessPolicy.POLICY_CREATE_SECRET |
                    LSA_AccessPolicy.POLICY_GET_PRIVATE_INFORMATION |
                    LSA_AccessPolicy.POLICY_LOOKUP_NAMES |
                    LSA_AccessPolicy.POLICY_NOTIFICATION |
                    LSA_AccessPolicy.POLICY_SERVER_ADMIN |
                    LSA_AccessPolicy.POLICY_SET_AUDIT_REQUIREMENTS |
                    LSA_AccessPolicy.POLICY_SET_DEFAULT_QUOTA_LIMITS |
                    LSA_AccessPolicy.POLICY_TRUST_ADMIN |
                    LSA_AccessPolicy.POLICY_VIEW_AUDIT_INFORMATION |
                    LSA_AccessPolicy.POLICY_VIEW_LOCAL_INFORMATION
                    );
                //initialize a pointer for the policy handle
                IntPtr policyHandle = IntPtr.Zero;

                //these attributes are not used, but LsaOpenPolicy wants them to exists
                LSA_OBJECT_ATTRIBUTES ObjectAttributes = new LSA_OBJECT_ATTRIBUTES();
                ObjectAttributes.Length = 0;
                ObjectAttributes.RootDirectory = IntPtr.Zero;
                ObjectAttributes.Attributes = 0;
                ObjectAttributes.SecurityDescriptor = IntPtr.Zero;
                ObjectAttributes.SecurityQualityOfService = IntPtr.Zero;

                //get a policy handle
                uint resultPolicy = LsaOpenPolicy(ref systemName, ref ObjectAttributes, access, out policyHandle);
                winErrorCode = LsaNtStatusToWinError(resultPolicy);

                if (winErrorCode != 0)
                {
                    SetupLogger.LogInfo("OpenPolicy failed: " + winErrorCode);
                }
                else
                {
                    //Now that we have the SID an the policy,
                    //we can add rights to the account.
                    //initialize an unicode-string for the privilege name
                    LSA_UNICODE_STRING[] userRights = new LSA_UNICODE_STRING[1];
                    userRights[0] = new LSA_UNICODE_STRING();
                    userRights[0].Buffer = Marshal.StringToHGlobalUni(privilegeName);
                    userRights[0].Length = (UInt16)(privilegeName.Length * UnicodeEncoding.CharSize);
                    userRights[0].MaximumLength = (UInt16)((privilegeName.Length + 1) * UnicodeEncoding.CharSize);
                    long res;
                    if (add)
                    {
                        //add the right to the account
                        res = LsaAddAccountRights(policyHandle, sid, userRights, 1);
                    }
                    else
                    {
                        res = LsaRemoveAccountRights(policyHandle, sid, true, userRights, 1);
                    }
                    winErrorCode = LsaNtStatusToWinError(res);
                    if (winErrorCode != 0)
                    {
                        throw new Exception("LsaAddAccountRights failed: " + winErrorCode);
                    }

                    NativeMethods.LsaClose(policyHandle);
                }
                FreeSid(sid);
            }

            return winErrorCode;
        }


        /// <summary>
        /// The GetPrivateProfileString function retrieves a string from the specified section in an initialization file.
        /// </summary>
        /// <param name="lpAppName">[in] Pointer to a null-terminated string that specifies the name of the section containing the key name.</param>
        /// <param name="lpKeyName">[in] Pointer to the null-terminated string specifying the name of the key whose associated string is to be retrieved</param>
        /// <param name="lpDefault">[in] Pointer to a null-terminated default string</param>
        /// <param name="lpReturnedString">[out] Pointer to the buffer that receives the retrieved string</param>
        /// <param name="nSize">[in] Size of the buffer pointed to by the lpReturnedString parameter, in TCHARs</param>
        /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies the name of the initialization file</param>
        /// <returns>The return value is the number of characters copied to the buffer, not including the terminating null character</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern UInt32 GetPrivateProfileString(
            String lpAppName,
            String lpKeyName,
            String lpDefault,
            StringBuilder lpReturnedString,
            UInt32 nSize,
            String lpFileName
            );

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, ExactSpelling = false, SetLastError = true), CLSCompliant(false)]
        public static extern UInt32 GetPrivateProfileString(
            String lpAppName,
            String lpKeyName,
            String lpDefault,
            [In, Out, MarshalAs(UnmanagedType.LPArray)]
            byte[] lpReturnedString,
            UInt32 nSize,
            String lpFileName
            );

        /// <summary>
        /// The MoveFileEx function moves an existing file or directory.
        /// </summary>
        /// <param name="lpExistingFileName">[in] Pointer to a null-terminated string that names an existing file or directory on the local computer.</param>
        /// <param name="lpNewFileName">[in] Pointer to a null-terminated string that specifies the new name of lpExistingFileName on the local computer.</param>
        /// <param name="dwFlags">[in] Flags </param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveFileEx([MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName,
                                             [MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName,
                                             MoveFileFlags dwFlags);

        [Flags]
        public enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        /// <summary>
        /// The GetDriveType function determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.
        /// </summary>
        /// <param name="lpRootPathName">[in] Pointer to a null-terminated string that
        /// specifies the root directory of the disk to return information about.
        /// A trailing backslash is required. If this parameter is NULL, the function uses
        /// the root of the current directory. </param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern int GetDriveType(
            String lpRootPathName
            );

        public const int DRIVE_UNKNOWN = 0,
            DRIVE_NO_ROOT_DIR = 1,
            DRIVE_REMOVABLE = 2,
            DRIVE_FIXED = 3,
            DRIVE_REMOTE = 4,
            DRIVE_CDROM = 5,
            DRIVE_RAMDISK = 6;

        public enum InstallErrorLevel : long
        {
            Error_Success = 0,
            Error_CannotOpenInstallationPackage = 2,
            Error_Install_UserExit = 1602,
            Error_Install_Failed = 1603,
            Error_Success_Reboot_Initiated = 1641,
            Error_Success_Reboot_Required = 3010,
        };

                //InstallUIHandler is needed for the MsiSetExternalUI import
        public delegate int InstallUIHandler(
                                                IntPtr context,
                                                Int32 messageType,
                                                [MarshalAs(UnmanagedType.LPTStr)] string message);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern InstallUIHandler MsiSetExternalUI(
                                                [MarshalAs(UnmanagedType.FunctionPtr)]InstallUIHandler handler,
                                                int messageFilter,
                                                IntPtr context);

        /// <summary>
        /// Used by setup to find older installed versions of DPS
        /// </summary>
        /// <param name="szUpgradeCode">[in] Upgrade code</param>
        /// <param name="reserved">[in] Reserved parameter. Must be 0</param>
        /// <param name="iProductIndex">[in] Zero based index into the index of products</param>
        /// <param name="productBuf">[out] The buffer that will hold the product code</param>
        /// <returns></returns>
        [DllImport("msi.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern UInt32 MsiEnumRelatedProducts(
            String szUpgradeCode,
            int reserved,
            int iProductIndex,
            StringBuilder productBuf);

        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const int ERROR_SUCCESS = 0;

        /// <summary>
        /// The MsiGetProductInfo function returns product information for published and installed products.
        /// </summary>
        /// <param name="szProduct">[in] Specifies the product code for the product.</param>
        /// <param name="szProperty">[in] Specifies the property to be retrieved.</param>
        /// <param name="lpValueBuf">[out] Pointer to a buffer that receives the property value.</param>
        /// <param name="pcchValueBuf">[in, out] Pointer to a variable that specifies the size, in characters,
        /// of the buffer pointed to by the lpValueBuf parameter. On input, this is the full size of the buffer,
        /// including a space for a terminating null character. If the buffer passed in is too small,
        /// the count returned does not include the terminating null character.</param>
        /// <returns>int</returns>
        [DllImport("msi.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern UInt32 MsiGetProductInfo(String szProduct,
            String szProperty,
            StringBuilder lpValueBuf,
            ref UInt32 pcchValueBuf);

        /// <summary>
        /// Product version.
        /// </summary>
        /// <remarks>Component : Setup (kushaln)</remarks>
        public const String INSTALLPROPERTY_VERSIONSTRING = "VersionString";

        /// <summary>
        /// The MsiSetInternalUI function enables the installer's internal user interface.
        /// </summary>
        /// <param name="dwUILdwUILevelevel">[in] Specifies the level of complexity of the user interface.</param>
        /// <param name="winhandle">[in, out] Pointer to a window. This window becomes the owner of any user interface created. A pointer to the previous owner of the user interface is returned.
        /// If this parameter is null, the owner of the user interface does not change.</param>
        /// <returns>The previous user interface level is returned. If an invalid dwUILevel is passed, then INSTALLUILEVEL_NOCHANGE is returned.</returns>
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern InstallUiLevel MsiSetInternalUI(int dwUILevel, ref IntPtr winhandle);

        public enum InstallUiLevel : int
        {
            NoChange = 0,    // UI level is unchanged
            Default = 1,    // default UI is used
            None = 2,    // completely silent installation
            Basic = 3,    // simple progress and error handling
            Reduced = 4,    // authored UI, wizard dialogs suppressed
            Full = 5,    // authored UI with wizards, progress, errors
            EndDialog = 0x80, // display success/failure dialog at end of install
            ProgressOnly = 0x40, // display only progress dialog
        };

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern int MsiEnableLog(
                                                int LogMode,           // logging options
                                                [MarshalAs(UnmanagedType.LPWStr)] string LogFile,           // log file name
                                                int LogAttributes);    // Flush attribute

        public enum InstallLogModes : int
        {
            None = 0,
            FatalExit = (1 << ((int)0x00000000 >> 24)),
            Error = (1 << ((int)0x01000000 >> 24)),
            Warning = (1 << ((int)0x02000000 >> 24)),
            User = (1 << ((int)0x03000000 >> 24)),
            Info = (1 << ((int)0x04000000 >> 24)),
            FilesInUse = (1 << ((int)0x05000000 >> 24)),
            ResolveSource = (1 << ((int)0x06000000 >> 24)),
            OutOfDiskSpace = (1 << ((int)0x07000000 >> 24)),
            ActionStart = (1 << ((int)0x08000000 >> 24)),
            ActionData = (1 << ((int)0x09000000 >> 24)),
            CommonData = (1 << ((int)0x0B000000 >> 24)),
            PropertyDump = (1 << ((int)0x0A000000 >> 24)), // log only
            Verbose = (1 << ((int)0x0C000000 >> 24)), // log only
            LogOnError = (1 << ((int)0x0E000000 >> 24)), // log only
            Progress = (1 << ((int)0x0A000000 >> 24)), // external handler only
            Initialize = (1 << ((int)0x0C000000 >> 24)), // external handler only
            Terminate = (1 << ((int)0x0D000000 >> 24)), // external handler only
            ShowDialog = (1 << ((int)0x0E000000 >> 24)), // external handler only
        }

        public enum InstallLogAttributes // flag attributes for MsiEnableLog
        {
            Append = (1 << 0),
            FlushEachLine = (1 << 1),
        };

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern int MsiInstallProduct(
                                                [MarshalAs(UnmanagedType.LPWStr)] string PackagePath,    // package to install
                                                [MarshalAs(UnmanagedType.LPWStr)] string CommandLine);   // command line

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern int MsiConfigureProductEx(
                                                [MarshalAs(UnmanagedType.LPWStr)] string productCode,    // product code
                                                int installLevel,    // install level
                                                int installState,    // install state
                                                [MarshalAs(UnmanagedType.LPWStr)] string CommandLine);   // command line

        // MsiSetExternalUI message types (see INSTALLMESSAGE_ in MSDN)
        public const int mtActionData = 0x08000000;
        public const int mtActionStart = 0x09000000;
        public const int mtProgress = 0x0A000000;
        public const int mtFatalExit = 0x00000000; // premature termination, possibly fatal OOM
        public const int mtError = 0x01000000; // formatted error message
        public const int mtWarning = 0x02000000; // formatted warning message
        public const int mtUser = 0x03000000; // user request message
        public const int mtInfo = 0x04000000; // informative message for log
        public const int mtFilesInUse = 0x05000000; // list of files in use that need to be replaced
        public const int mtResolveSource = 0x06000000; // request to determine a valid source location
        public const int mtOutOfDiskSpace = 0x07000000; // insufficient disk space message
        public const int mtCommonData = 0x0B000000; // product info for dialog: language Id, dialog caption
        public const int mtInitialize = 0x0C000000; // sent prior to UI initialization, no string data
        public const int mtTerminate = 0x0D000000; // sent after UI termination, no string data
        public const int mtShowDialog = 0x0E000000; // sent prior to display or authored dialog or wizard

        /// <summary>
        /// The GetPrivateProfileInt function retrieves an int from the specified section in an initialization file.
        /// </summary>
        /// <param name="lpAppName">[in] Pointer to a null-terminated string that specifies the name of the section containing the key name.</param>
        /// <param name="lpKeyName">[in] Pointer to the null-terminated string specifying the name of the key whose associated string is to be retrieved</param>
        /// <param name="lpDefault">[in] Pointer to a null-terminated default value</param>
        /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies the name of the initialization file</param>
        /// <returns>The return value is the number of characters copied to the buffer, not including the terminating null character</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern int GetPrivateProfileInt(
            String lpAppName,
            String lpKeyName,
            int lpDefault,
            String lpFileName
            );

        /// <summary>
        /// The GetDiskFreeSpaceEx function retrieves information about the amount of space
        /// available on a disk volume: the total amount of space, the total amount of free space,
        /// and the total amount of free space available to the user associated with the calling
        /// thread.
        /// </summary>
        /// <param name="lpDirectoryName">[in] Pointer to a null-terminated string that specifies a directory on the disk of interest</param>
        /// <param name="lpFreeBytesAvailable">[out] Pointer to a variable that receives the total number of free bytes on the disk that are available to the user associated with the calling thread.</param>
        /// <param name="lpTotalNumberOfBytes">[out] Pointer to a variable that receives the total number of bytes on the disk that are available to the user associated with the calling thread.</param>
        /// <param name="lpTotalNumberOfFreeBytes">[out] Pointer to a variable that receives the total number of free bytes on the disk.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), CLSCompliant(false)]
        public static extern bool GetDiskFreeSpaceEx(
            String lpDirectoryName,
            ref UInt64 lpFreeBytesAvailable,
            ref UInt64 lpTotalNumberOfBytes,
            ref UInt64 lpTotalNumberOfFreeBytes
            );

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokenPrivilegeLuid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int LookupPrivilegeValue(
            [MarshalAs(UnmanagedType.LPWStr)] string host,
            [MarshalAs(UnmanagedType.LPWStr)] string name,
            ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int AdjustTokenPrivileges(
            IntPtr htok, bool disall, ref TokenPrivilegeLuid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int ExitWindowsEx(int flag, int reason);

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;
        internal const int SHTDN_REASON_MINOR_INSTALLATION = 0x00000002;

        public static void ExitWindows(int flag)
        {
            TokenPrivilegeLuid tokenPrivilegeLuid;
            IntPtr processHandle = GetCurrentProcess();
            SetupLogger.LogInfo("Exit Windows: GetCurrentProcess: processHandle: " + processHandle.ToString());
            IntPtr tokenHandle = IntPtr.Zero;
            int status = OpenProcessToken(processHandle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle);
            SetupLogger.LogInfo("Exit Windows: OpenProcessToken: status: " + status.ToString());
            tokenPrivilegeLuid.Count = 1;
            tokenPrivilegeLuid.Luid = 0;
            tokenPrivilegeLuid.Attr = SE_PRIVILEGE_ENABLED;
            status = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tokenPrivilegeLuid.Luid);
            SetupLogger.LogInfo("Exit Windows: LookupPrivilegeValue: status: " + status.ToString());
            status = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivilegeLuid, 0, IntPtr.Zero, IntPtr.Zero);
            SetupLogger.LogInfo("Exit Windows: AdjustTokenPrivileges: status: " + status.ToString());
            status = ExitWindowsEx(flag, SHTDN_REASON_MINOR_INSTALLATION);
            SetupLogger.LogInfo("Exit Windows: ExitWindowsEx: status: " + status.ToString());
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), CLSCompliant(false)]
        public struct LSA_OBJECT_ATTRIBUTES
        {
            public ulong Length;
            public IntPtr RootDirectory;
            public LSA_UNICODE_STRING ObjectName;
            public ulong Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), CLSCompliant(false)]
        public struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true, PreserveSig=true), CLSCompliant(false)]
        public static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING machineName,
            ref LSA_OBJECT_ATTRIBUTES objectAttr,
            UInt32 accessType,
            out IntPtr lsaPolicyHandle
            );

        public enum LsaPolicies : long
        {
            POLICY_CREATE_ACCOUNT = 0x0010L,
            POLICY_LOOKUP_NAMES = 0x0800L,
            GENERIC_READ = 0x080000000L,
            POLICY_VIEW_LOCAL_INFORMATION = 0X01L
        };

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 LsaNtStatusToWinError(UInt32 status);

        public enum POLICY_INFORMATION_CLASS
        {
            PolicyAuditLogInformation = 1,
            PolicyAuditEventsInformation,
            PolicyPrimaryDomainInformation,
            PolicyPdAccountInformation,
            PolicyAccountDomainInformation,
            PolicyLsaServerRoleInformation,
            PolicyReplicaSourceInformation,
            PolicyDefaultQuotaInformation,
            PolicyModificationInformation,
            PolicyAuditFullSetInformation,
            PolicyAuditFullQueryInformation,
            PolicyDnsDomainInformation
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true), CLSCompliant(false)]
        public static extern UInt32 LsaQueryInformationPolicy(
            IntPtr lsaPolicyHandle,
            POLICY_INFORMATION_CLASS informationClass,
            out IntPtr buffer
            );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), CLSCompliant(false)]
        public struct POLICY_PRIMARY_DOMAIN_INFO
        {
            public LSA_UNICODE_STRING DomainName;
            public IntPtr Sid;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true), CLSCompliant(false)]
        public static extern UInt32 LsaFreeMemory(IntPtr buff);


        public static IntPtr NullIntPtr;

        public const int
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
    }

    [System.Runtime.InteropServices.ComVisible(false), SuppressUnmanagedCodeSecurity()]
    [CLSCompliant(false)]
    public class UnsafeNativeMethods
    {
        /// <summary>
        /// Win32 LogonUser
        /// </summary>
        /// <param name="lpszUsername"></param>
        /// <param name="lpszDomain"></param>
        /// <param name="lpszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain,
             [In] IntPtr lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// Win32 CloseHandle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        /// <summary>
        /// This corresponds to the AUTHZ_ACCESS_REQUEST struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AuthzAccessRequest
        {
            public int DesiredAccess;
            public IntPtr PrincipalSelfSid;
            public IntPtr ObjectTypeList;
            public int ObjectTypeListLength;
            public IntPtr OptionalArguments;

            public AuthzAccessRequest(int desiredAccess)
            {
                this.DesiredAccess = desiredAccess;
                this.PrincipalSelfSid = IntPtr.Zero;
                this.ObjectTypeList = IntPtr.Zero;
                this.ObjectTypeListLength = 0;
                this.OptionalArguments = IntPtr.Zero;
            }
        }

        /// <summary>
        /// This corresponds to the AUTHZ_ACCESS_REPLY struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AuthzAccessReply
        {
            public int ResultListLength;
            public IntPtr GrantedAccessMask;
            public IntPtr SaclEvaluationResults;
            public IntPtr Error;
        }

        [DllImport("authz.dll", SetLastError = true)]
        public static extern bool AuthzAccessCheck(
            int flags,
            IntPtr authzClientContext,
            ref AuthzAccessRequest request,
            IntPtr auditInfo,
            [MarshalAs(UnmanagedType.LPArray)] byte[] securityDescriptor,
            IntPtr optionalSecurityDescriptorArray,
            int optionalSecurityDescriptorCount,
            ref AuthzAccessReply reply,
            IntPtr authzHandle);

        public enum AuthzInitializeResourceManagerFlags
        {
            Default = 0,
            AUTHZ_RM_FLAG_NO_AUDIT = 1,
            AUTHZ_RM_FLAG_INITIALIZE_UNDER_IMPERSONATION = 2,
        }

        [DllImport("authz.dll", SetLastError = true)]
        public static extern bool AuthzInitializeResourceManager(
            int flags,
            IntPtr accessCheckCallback,
            IntPtr computeDynamicGroupsCallback,
            IntPtr freeDynamicGroupsCallback,
            [MarshalAs(UnmanagedType.LPTStr)] string resourceManagerName,
            out IntPtr authzResourceManager);

        /// <summary>
        /// This corresponds to the LUID struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Luid
        {
            public int LowPart;
            public int HighPart;
        }

        public enum AuthzInitializeContextFromSidFlags
        {
            Default = 0,
            AuthzSkipTokenGroups = 2,
            AuthzRequireS4ULogon = 4,
        }

        [DllImport("authz.dll", SetLastError = true)]
        public static extern bool AuthzInitializeContextFromSid(
            [MarshalAs(UnmanagedType.I4)] AuthzInitializeContextFromSidFlags flags,
            [MarshalAs(UnmanagedType.LPArray)] byte[] userSid,
            IntPtr authzResourceManager,
            IntPtr expirationTime,
            Luid identifier,
            IntPtr dynamicGroupArgs,
            out IntPtr authzClientContext);

        /// <summary>
        /// Checks an access check reply for whether the permissions are granted
        /// </summary>
        /// <param name="accessReply"></param>
        /// <returns></returns>
        public static bool AuthzAccessIsGranted(ref AuthzAccessReply accessReply)
        {
            int[] error = new int[accessReply.ResultListLength];
            Marshal.Copy(accessReply.Error, error, 0, accessReply.ResultListLength);

            bool isGranted = Array.TrueForAll(error,
                delegate(int errorElement)
                {
                    return errorElement == NativeMethods.ERROR_SUCCESS;
                });
            return isGranted;
        }

        [DllImport("authz.dll", SetLastError = true)]
        public static extern bool AuthzFreeContext(
            IntPtr authzClientContext);

        [DllImport("authz.dll", SetLastError = true)]
        public static extern bool AuthzFreeResourceManager(
            IntPtr authzResourceManager);
    }
}

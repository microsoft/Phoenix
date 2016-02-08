using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;

using KryptoLib;

namespace CMP.Setup.Helpers
{
    class UserAccountHelper
    {
        public static string GetServiceAccount()
        {
            string userAccountName = null;
            if (SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag))
            {
                userAccountName = DnsHelper.GetLocalMachineAccount();
            }
            else
            {
                userAccountName = GetVmmServiceDomainAccount();
            }
            return userAccountName;
        }

        public static string GetVmmServiceDomainAccount()
        {
            string domainName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceDomainTag);
            string userName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserNameTag);
            return String.Format(SetupConstants.UserAccountTemplate, domainName, userName);
        }

        /// <summary>
        /// Get the account name given a Sid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetAccountNameFromSid(SecurityIdentifier id)
        {
            AppAssert.AssertNotNull(id);
            return id.Translate(typeof(NTAccount)).Value;
        }

        public static String GetBinarySidString(String accountName)
        {
            NTAccount ntAccount = new NTAccount(accountName);
            SecurityIdentifier id = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
            byte[] binarySid = UserAccountHelper.GetBinarySid(id);
            return "0x" + BitConverter.ToString(binarySid).Replace("-", string.Empty);
        }

        public static byte[] GetBinarySid(string sddlForm)
        {
            // Converts sid string in S-1-5-32-xxxx form to binary form
            if (string.IsNullOrEmpty(sddlForm))
            {
                return null;
            }
            SecurityIdentifier sid = new SecurityIdentifier(sddlForm);
            return GetBinarySid(sid);
        }

        public static byte[] GetBinarySid(SecurityIdentifier sid)
        {
            // Converts sid to binary form
            AppAssert.Assert(sid != null);
            if (sid == null)
            {
                throw new ArgumentNullException("sid");
            }
            byte[] sidbuff = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidbuff, 0);
            return sidbuff;
        }

        public static bool ValidateCredentials(string userName, string domain, SecureString password)
        {
            bool logonSuccess;
            IntPtr token = IntPtr.Zero;
            IntPtr passwordPtr = IntPtr.Zero;

            string traceStr = string.Format(@"ValidateCredentials - {0}\{1}", domain, userName);

            try
            {
                passwordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                logonSuccess = UnsafeNativeMethods.LogonUser(userName, domain, passwordPtr,
                    NativeMethods.LOGON32_LOGON_NETWORK, NativeMethods.LOGON32_PROVIDER_DEFAULT, ref token);
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    UnsafeNativeMethods.CloseHandle(token);
                }
                if (passwordPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);
                }
            }

            return logonSuccess;
        }

        /// <summary>
        /// This methods validates vmm service domain account.
        /// </summary>
        /// <returns>bool</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Once time, setup, nothing to worry"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Once time, setup, nothing to worry"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VirtualManager.SetupFramework.NativeMethods.DsUnBind(System.IntPtr@)", Justification = "Once time, setup, nothing to worry")]
        public static bool ValidateServiceAccount()
        {
            bool valid;
            valid = UserAccountHelper.CheckDomainAccountValid();
            if (valid)
            {
                valid = UserAccountHelper.CheckDomainAccountInAdminGrp();
            }

            return valid;
        }

        /// <summary>
        /// Check to see whether the domain account and its password are valid
        /// </summary>
        private static bool CheckDomainAccountValid()
        {
            string domainName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceDomainTag);
            string userName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserNameTag);
            SecureString password = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserPasswordTag);

            if (UserAccountHelper.ValidateCredentials(userName, domainName, password))
            {
                return true;
            }
            else
            {
                //throw SetupExceptionFactory.NewBackEndErrorException(ErrorCode.InvalidDomainAccount);
                SetupLogger.LogError("CheckDomainAccountValid(): Invalid domain account");
                return false;
            }
        }

        public static string EncryptStringUsingLocalCertificate(string inputString, string thumbprint)
        {
            string encryptedText = String.Empty;

            X509Certificate2 certificate = X509Krypto.FetchCertFromStoreImpl(StoreName.My, StoreLocation.LocalMachine, thumbprint);

            byte[] inputAsByteArray = new byte[inputString.Length * sizeof(char)];
            Buffer.BlockCopy(inputString.ToCharArray(), 0, inputAsByteArray, 0, inputAsByteArray.Length);

            byte[] outputByteArray = X509Krypto.EncryptImpl(inputAsByteArray, true, certificate);

            return Convert.ToBase64String(outputByteArray);
        }

        /// <summary>
        /// Check to see whether the domain account is under the local admin group
        /// comments: there is another way to call CheckTokenMembership() to check whether this domain account is in local admin grp
        /// </summary>
        private static bool CheckDomainAccountInAdminGrp()
        {
            string domainName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceDomainTag);
            string userName = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserNameTag);

            return UserAccountHelper.IsRoleFor(
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                String.Format(SetupConstants.UserAccountTemplate, domainName.ToLower(), userName.ToLower()));
        }

        private static TDest Translate<TSrc, TDest>(TSrc src)
            where TSrc : IdentityReference
            where TDest : IdentityReference
        {
            TDest dest = null;

            try
            {
                dest = (TDest)src.Translate(typeof(TDest));
            }
            catch (SystemException e)
            {
            }

            return dest;
        }

        public static bool IsADomainAccount(string userAccount)
        {
            if (String.IsNullOrEmpty(userAccount))
                return false;

            return IsADomainAccount(UserAccountHelper.GetNTAccountFromName(userAccount));
        }

        public static bool IsRoleFor(SecurityIdentifier adminGroupSID, string accountName)
        {
            SecurityIdentifier sid = null;
            if (UserAccountHelper.IsADomainAccount(accountName))
            {
                NTAccount ntAccount = UserAccountHelper.GetNTAccountFromName(accountName);
                sid = Translate<NTAccount, SecurityIdentifier>(ntAccount);
                if (sid == null)
                {
                    sid = new SecurityIdentifier(ntAccount.Value);
                }

                return AuthzAccessCheck(adminGroupSID, sid);
            }

            // Ignore the check for non-domain accounts.
            return true;
        }

        public static bool IsADomainAccount(NTAccount userAccount)
        {
            if (userAccount == null)
                return false;

            SecurityIdentifier sid;
            try
            {
                sid = UserAccountHelper.GetSecurityIdentifier(userAccount);
            }
            catch (Exception e)
            {
                return false;
            }

            if (UserAccountHelper.IsLocalAccount(sid) || (UserAccountHelper.IsBuiltInGroup(sid)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if sid is for local account
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Returns true if the account is local admin</returns>
        public static bool IsLocalAccount(SecurityIdentifier sid)
        {
            // Nobody should call this method with a null sid.
            Debug.Assert(sid != null);
            if (sid == null)
            {
                throw new ArgumentNullException("sid");
            }

            return sid.IsEqualDomainSid(UserAccountHelper.LocalAdminDomainSid);
        }

        public static bool IsBuiltInGroup(SecurityIdentifier sid)
        {
            // The SID of the builtin local groups is always like S-1-5-32-XXX
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^S-1-5-32-[0-9][0-9][0-9]$");
            return regex.IsMatch(sid.Value);
        }

        private static SecurityIdentifier LocalAdminDomainSid
        {
            get
            {
                SecurityIdentifier localAdminDomainSid = null;
                ManagementScope managementScope = null;
                ObjectQuery objectQuery = UserAccountHelper.GetLocalAccountQuery(out managementScope);
                using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(managementScope, objectQuery))
                {
                    // execute the query
                    using (ManagementObjectCollection objectCollection = objectSearcher.Get())
                    {
                        // retrieve the details
                        foreach (ManagementObject accounts in objectCollection)
                        {
                            string tempSid = (string)accounts.GetPropertyValue("SID");
                            if (tempSid.StartsWith("S-1-5-", StringComparison.OrdinalIgnoreCase) && tempSid.EndsWith("-500", StringComparison.OrdinalIgnoreCase))
                            {
                                localAdminDomainSid = new SecurityIdentifier(tempSid).AccountDomainSid;
                                break;
                            }
                        }
                    }
                }

                return localAdminDomainSid;
            }
        }

        private static ObjectQuery GetLocalAccountQuery(out ManagementScope managementScope)
        {
            managementScope = new ManagementScope(string.Format(@"\\{0}\root\cimv2", Environment.MachineName));
            ObjectQuery objectQuery = new ObjectQuery(@"Select * From Win32_UserAccount Where LocalAccount = TRUE");
            return objectQuery;
        }

        public static SecurityIdentifier GetSecurityIdentifier(string accountName)
        {
            AppAssert.Assert(!string.IsNullOrEmpty(accountName));

            // Check if the string is null or empty - these are not valid account names
            if (string.IsNullOrEmpty(accountName))
            {
                throw new IdentityNotMappedException();
            }

            // verify that the entered string is a valid account name
            return UserAccountHelper.GetSecurityIdentifier(new NTAccount(accountName.Trim()));
        }

        /// <summary>
        /// Get the sid string based on the NTAccount
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static SecurityIdentifier GetSecurityIdentifier(NTAccount account)
        {
            SecurityIdentifier sid = null;

            try
            {
                sid = account.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
            }
            catch (SystemException e)
            {
                throw new Exception("Could not locate security info for the account");
            }

            return sid;
        }

        /// <summary>
        /// Get an NTAccount object based on an account name
        /// </summary>
        /// <param name="accountName">a valid windows account name</param>
        /// <returns>NTAccount that represents the passed-in account name</returns>
        public static NTAccount GetNTAccountFromName(string accountName)
        {
            try
            {
                return new NTAccount(accountName.Trim());
            }
            catch (ArgumentException e)
            {
                throw new Exception("Not a domain account");
            }
        }

        private const int StandardAccess = 0x001FFFFF;
        private const int MaximumAllowed = 0x02000000;

        private static bool AuthzAccessCheck(SecurityIdentifier roleSid, SecurityIdentifier userSid)
        {
            IntPtr resourceManager = IntPtr.Zero;
            try
            {
                resourceManager = UserAccountHelper.CreateAuthzResourceManager();

                IntPtr clientContext = IntPtr.Zero;
                try
                {
                    clientContext = UserAccountHelper.CreateAuthzClientContext(userSid, resourceManager);

                    UnsafeNativeMethods.AuthzAccessReply accessReply = new UnsafeNativeMethods.AuthzAccessReply();
                    try
                    {
                        UserAccountHelper.InitializeAuthzAccessReply(ref accessReply);

                        UnsafeNativeMethods.AuthzAccessRequest accessRequest = new UnsafeNativeMethods.AuthzAccessRequest(
                            UserAccountHelper.MaximumAllowed);
                        byte[] roleSecurityDescriptorData = UserAccountHelper.GetRoleSecurityDescriptorData(
                            roleSid,
                            UserAccountHelper.StandardAccess);

                        if (!UnsafeNativeMethods.AuthzAccessCheck(
                            0,
                            clientContext,
                            ref accessRequest,
                            IntPtr.Zero,
                            roleSecurityDescriptorData,
                            IntPtr.Zero,
                            0,
                            ref accessReply,
                            IntPtr.Zero))
                        {
                            throw new Exception("Failed to get authorization information");
                        }

                        return UnsafeNativeMethods.AuthzAccessIsGranted(ref accessReply);
                    }
                    finally
                    {
                        UserAccountHelper.UninitializeAuthzAccessReply(ref accessReply);
                    }
                }
                finally
                {
                    if (clientContext != IntPtr.Zero)
                    {
                        if (!UnsafeNativeMethods.AuthzFreeContext(clientContext))
                        {
                            throw new Exception("Failed to get authorization information");
                        }
                    }
                }
            }
            finally
            {
                if (resourceManager != IntPtr.Zero)
                {
                    if (!UnsafeNativeMethods.AuthzFreeResourceManager(resourceManager))
                    {
                        throw new Exception("Failed to get authorization information");
                    }
                }
            }
        }

        /// <summary>
        /// Frees resources associated with an initialized access reply
        /// </summary>
        /// <param name="accessReply"></param>
        public static void UninitializeAuthzAccessReply(ref UnsafeNativeMethods.AuthzAccessReply accessReply)
        {
            if (accessReply.GrantedAccessMask != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(accessReply.GrantedAccessMask);
            }
            if (accessReply.SaclEvaluationResults != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(accessReply.SaclEvaluationResults);
            }
            if (accessReply.Error != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(accessReply.Error);
            }
        }

        private static byte[] GetRoleSecurityDescriptorData(SecurityIdentifier roleSid, int accessMask)
        {
            // Create a new ACL with one ACE.
            DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(false, false, 1);
            discretionaryAcl.AddAccess(AccessControlType.Allow, roleSid, accessMask,
                InheritanceFlags.None, PropagationFlags.None);

            // Create a new security descriptor.
            SecurityIdentifier owner = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            SecurityIdentifier group = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            GenericSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(
                false, false, ControlFlags.DiscretionaryAclPresent,
                owner, group, null, discretionaryAcl);

            // Marshal the security descriptor.
            byte[] securityDescriptorData = new byte[securityDescriptor.BinaryLength];
            securityDescriptor.GetBinaryForm(securityDescriptorData, 0);
            return securityDescriptorData;
        }

        /// <summary>
        /// Creates an unnamed resource manager
        /// Call CreateAuthzResourceManager to dispose
        /// </summary>
        /// <returns></returns>
        public static IntPtr CreateAuthzResourceManager()
        {
            IntPtr resourceManager;
            if (!UnsafeNativeMethods.AuthzInitializeResourceManager(
                (int)UnsafeNativeMethods.AuthzInitializeResourceManagerFlags.AUTHZ_RM_FLAG_NO_AUDIT, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, null,
                out resourceManager))
            {
                throw new Exception("Failed to get authorization information");
            }
            return resourceManager;
        }


        private static IntPtr CreateAuthzClientContext(SecurityIdentifier userSid, IntPtr resourceManager)
        {
            byte[] sidData = new byte[userSid.BinaryLength];
            userSid.GetBinaryForm(sidData, 0);

            UnsafeNativeMethods.Luid identifier = new UnsafeNativeMethods.Luid();
            IntPtr clientContext;
            if (!UnsafeNativeMethods.AuthzInitializeContextFromSid(
                0,
                sidData,
                resourceManager,
                IntPtr.Zero,
                identifier,
                IntPtr.Zero,
                out clientContext))
            {
                throw new Exception("Failed to get authorization information");
            }
            return clientContext;
        }

        /// <summary>
        /// Allocates resources for an access reply
        /// Call UninitializeAuthzAccessReply to free the resources
        /// </summary>
        /// <param name="accessReply"></param>
        public static void InitializeAuthzAccessReply(ref UnsafeNativeMethods.AuthzAccessReply accessReply)
        {
            accessReply.ResultListLength = 1;
            accessReply.GrantedAccessMask = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * accessReply.ResultListLength);
            accessReply.SaclEvaluationResults = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * accessReply.ResultListLength);
            accessReply.Error = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * accessReply.ResultListLength);
        }

    }
}

using System;
using CmpInterfaceModel;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    public class RegistryHelper
    {
        /// <summary>
        /// Error text
        /// </summary>
        public static string ErrorText = string.Empty;

        private RegistryHelper()
        {
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Open registry base key from local or remote machine
        /// </summary>
        /// <param name="hive">the hive name</param>
        /// <param name="machineName">machine name, or null</param>
        /// <returns>a Registry Key</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        /// 
        //*********************************************************************

        private static RegistryKey OpenBaseKey(string hive, string machineName)
        {
            RegistryKey registryKey = null;
            try
            {
                if (string.IsNullOrEmpty(machineName))
                {
                    // ROOTKEY [HKLM | HKCU | HKCR | HKU | HKCC]
                    switch (hive.ToUpper())
                    {
                        case "HKLM":
                        case "HKEY_LOCAL_MACHINE":
                            registryKey = Registry.LocalMachine;
                            break;
                        case "HKCU":
                        case "HKEY_CURRENT_USER":
                            registryKey = Registry.CurrentUser;
                            break;
                        case "HKCR":
                        case "HKEY_CLASSES_ROOT":
                            registryKey = Registry.ClassesRoot;
                            break;
                        case "HKU":
                        case "HKEY_USERS":
                            registryKey = Registry.Users;
                            break;
                        case "HKCC":
                        case "HKEY_CURRENT_CONFIG":
                            registryKey = Registry.CurrentConfig;
                            break;
                    }
                }
                else // Open key from remote computer
                {
                    // ROOTKEY [HKLM | HKCU | HKCR | HKU | HKCC]
                    switch (hive.ToUpper())
                    {
                        case "HKLM":
                        case "HKEY_LOCAL_MACHINE":
                            registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName);
                            break;
                        case "HKCU":
                        case "HKEY_CURRENT_USER":
                            registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, machineName);
                            break;
                        case "HKCR":
                        case "HKEY_CLASSES_ROOT":
                            registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot, machineName);
                            break;
                        case "HKU":
                        case "HKEY_USERS":
                            registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, machineName);
                            break;
                        case "HKCC":
                        case "HKEY_CURRENT_CONFIG":
                            registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentConfig, machineName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }

            return registryKey;
        }

        //*********************************************************************
        /// <summary>
        /// Get value from the registry, with default.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static object GetValue(RegistryKey registryKey, string name, object defaultValue)
        {
            try
            {
                if (null != registryKey)
                {
                    var o = registryKey.GetValue(name);
                    if (null != o)
                        return o;
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }

            return defaultValue;
        }

        //*********************************************************************
        /// <summary>
        /// Get a registry key.
        /// If writable, the registry path will be automatically created,
        /// and the key will be opened for updating.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="writeable">if set to <c>true</c> [writable].</param>
        /// <returns>Registry Key</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static RegistryKey GetRegistryKey(string path, bool writeable)
        {
            return GetRegistryKey(path, writeable, null);
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry key.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="writeable">if set to <c>true</c> [writable].</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry Key</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static RegistryKey GetRegistryKey(string path, bool writeable, string machineName)
        {
            RegistryKey regKey = null;
            var regPath = string.Empty;
            var subKeyName = string.Empty;

            try
            {
                // Get the RootKey
                var iPos = path.IndexOf(@"\");
                if (iPos >= 0)
                {
                    subKeyName = path.Substring(0, iPos);
                    regPath = path.Substring(iPos + 1);
                }

                var baseKey = RegistryHelper.OpenBaseKey(subKeyName, machineName);

                try // to open the whole key at once
                {
                    regKey = baseKey.OpenSubKey(regPath, writeable);
                    if ((null != regKey) || !writeable)
                        return regKey;
                }
                catch (System.Security.SecurityException)
                {
                    throw;
                }
                catch (Exception)
                {
                    regKey = null;
                }

                // If we reach this point, the registry path does not exist.
                regKey = baseKey;
                while ((null != regKey) && (regPath.Length != 0))
                {
                    // Get the next SubKey
                    iPos = regPath.IndexOf(@"\");
                    if (iPos >= 0)
                    {
                        subKeyName = regPath.Substring(0, iPos);
                        regPath = regPath.Substring(iPos + 1);
                    }
                    else
                    {
                        subKeyName = regPath;
                        regPath = string.Empty;
                    }

                    // Open the subkey
                    var subKey = regKey.OpenSubKey(subKeyName, writeable);

                    // If not found, and the key is marked writable, create this subkey.
                    if ((null == subKey) && (writeable))
                    {
                        subKey = regKey.CreateSubKey(subKeyName);
                    }

                    regKey.Close();
                    regKey = subKey;

                }
            }
            catch (System.Security.SecurityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }

            return regKey;
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry value.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static object GetRegistryValue(string path, string name, object defaultValue, string machineName)
        {
            // Reject invalid parameters
            if ((name.Length == 0) || (path.Length == 0))
            {
                ErrorText = "Invalid Registry path or name";
                return defaultValue;
            }

            try
            {
                ErrorText = string.Empty;
                var regKey = GetRegistryKey(path, false, machineName);
                return GetValue(regKey, name, defaultValue);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                //Recorder.Log("Exception in RegistryHelper.  " + ex.Message);
                return defaultValue;
            }
        }

        //*********************************************************************
        /// <summary>
        /// Gets the remote registry string.
        /// </summary>
        /// <param name="qualifiedName">Name of the qualified.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static string GetRemoteRegistryString(string qualifiedName, string defaultValue, string machineName)
        {
            var iPos = qualifiedName.LastIndexOf(@"\");
            if (iPos >= 0)
            {
                var path = qualifiedName.Substring(0, iPos);
                var name = qualifiedName.Substring(iPos + 1);
                return GetRegistryString(path, name, defaultValue, machineName);
            }

            ErrorText = "Invalid Registry QualifiedName";
            return defaultValue;
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry string.
        /// </summary>
        /// <param name="qualifiedName">Name of the qualified.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static string GetRegistryString(string qualifiedName, string defaultValue)
        {
            var iPos = qualifiedName.LastIndexOf(@"\");
            if (iPos >= 0)
            {
                var path = qualifiedName.Substring(0, iPos);
                var name = qualifiedName.Substring(iPos + 1);
                if (name.Length == 0)
                    name = "(Default)";
                return GetRegistryString(path, name, defaultValue, null);
            }

            ErrorText = "Invalid Registry QualifiedName";
            return defaultValue;
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static string GetRegistryString(string path, string name, string defaultValue)
        {
            return GetRegistryString(path, name, defaultValue, null);
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static string GetRegistryString(string path, string name, string defaultValue, string machineName)
        {
            return GetRegistryValue(path, name, (object)defaultValue, machineName).ToString();
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry string array.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry value array</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static string[] GetRegistryStringArray(string path, string name, string[] defaultValue, string machineName)
        {
            var o = GetRegistryValue(path, name, (object)defaultValue, machineName);
            if (o != null)
            {
                if (o.GetType() == typeof(string[]))
                    return (string[])o;
                if (o is string)
                    return o.ToString().Split(' ');
            }
            return defaultValue;
        }

        //*********************************************************************
        /// <summary>
        /// Sets the registry string.
        /// </summary>
        /// <param name="qualifiedName">Name of the qualified.</param>
        /// <param name="value">The value.</param>
        /// <returns>Registry value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static int SetRegistryString(string qualifiedName, string value)
        {
            var retval = -1;
            var iPos = qualifiedName.LastIndexOf(@"\");
            if (iPos >= 0)
            {
                ErrorText = string.Empty;
                var path = qualifiedName.Substring(0, iPos);
                var name = qualifiedName.Substring(iPos + 1);
                retval = SetRegistryString(path, name, value);
            }
            else
            {
                ErrorText = "Invalid Registry QualifiedName";
            }

            return retval;
        }

        //*********************************************************************
        /// <summary>
        /// Sets the registry string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>Status code, 1 = ok, -1 = failure</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static int SetRegistryString(string path, string name, string value)
        {
            var retval = -1;
            try
            {
                var regKey = GetRegistryKey(path, true);
                regKey.SetValue(name, value, RegistryValueKind.String);
                retval = 0;
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
            return retval;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static void SetRegistryDword(string path, string name, double value)
        {
            try
            {
                var regKey = GetRegistryKey(path, true);
                regKey.SetValue(name, value, RegistryValueKind.DWord);
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                throw new RegistryHelperException(
                    "Exception in SetRegistryDword : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// <summary>
        /// Sets the remote registry string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Status code, 1 = ok, -1 = failure</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static int SetRemoteRegistryString(string path, string name, string value, string machineName)
        {
            var retval = -1;
            try
            {
                var regKey = GetRegistryKey(path, true, machineName);
                regKey.SetValue(name, value, RegistryValueKind.String);
                retval = 0;
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
            return retval;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="machineName"></param>
        /// 
        //*********************************************************************

        public static void SetRemoteRegistryDword(string path, string name, double value, string machineName)
        {
            try
            {
                var regKey = GetRegistryKey(path, true, machineName);
                regKey.SetValue(name, value, RegistryValueKind.DWord);
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                throw new RegistryHelperException(
                    "Exception in SetRemoteRegistryDword : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// <summary>
        /// Gets the registry integer.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns>Registry Value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static int GetRegistryInteger(string path, string keyName, int defaultValue, string machineName)
        {
            var o = GetRegistryValue(path, keyName, defaultValue, machineName);
            return (int)o;
        }

        //*********************************************************************
        /// <summary>
        /// Get a Boolean value from the Registry
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns>Registry Value</returns>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static bool GetRegistryBool(string path, string name, bool defaultValue)
        {
            var result = false;
            var sDefault = defaultValue ? "True" : "False";

            if ("TRUE" == GetRegistryString(path, name, sDefault).ToUpper())
            {
                result = true;
            }
            return result;
        }

        //*********************************************************************
        /// <summary>
        /// Sets the registry bool.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <exception cref="RegistryHelperException">Thrown when exception occurs</exception>
        //*********************************************************************

        public static void SetRegistryBool(string path, string name, bool value)
        {
            var stringValue = value ? "True" : "False";

            SetRegistryString(path, name, stringValue);
        }
    }
    //*************************************************************************
    ///
    /// <summary>
    /// Thrown by members of the RegistryHelper class
    /// </summary>
    /// 
    //*************************************************************************
    [Serializable()]
    public class RegistryHelperException : Exception, ISerializable
    {
        /// <summary>
        /// Thrown by members of the OpsMgrDbAvailQuery class
        /// </summary>
        public RegistryHelperException()
        {
        }

        /// <summary>
        /// Thrown by members of the OpsMgrDbAvailQuery class
        /// </summary>
        /// <param name="message">Human readable text specific to this exception</param>
        public RegistryHelperException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Thrown by members of the OpsMgrDbAvailQuery class
        /// </summary>
        /// <param name="message">Human readable text specific to this exception</param>
        /// <param name="inner">The inner exception if available</param>
        public RegistryHelperException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}


//+--------------------------------------------------------------
//
//  Description: Registry utils
//
//---------------------------------------------------------------
using System;
using Microsoft.Win32;
using System.Collections.Generic;

namespace CMP.Setup.Helpers
{
    public class RegistryUtils
    {
        public static object ReadRegistryValue(string key, string valueName)
        {
            return ReadRegistryValue(key, valueName, null, true);
        }

        public static object ReadRegistryValue(string key, string valueName, object defaultValue)
        {
            return ReadRegistryValue(key, valueName, defaultValue, false);
        }

        public static object ReadRegistryValue(string key, string valueName, object defaultValue, bool throwOnError, bool ignoreArchitecture=false)
        {
            return ReadRegistryValue(key, valueName, defaultValue, throwOnError, RegistryHive.LocalMachine, ignoreArchitecture: ignoreArchitecture);
        }

        public static object ReadUserSpecificRegistryValue(string key, string valueName, object defaultValue, bool throwOnError)
        {
            return ReadRegistryValue(key, valueName, defaultValue, throwOnError, RegistryHive.CurrentUser);
        }

        private static object ReadRegistryValue(string key, string valueName, object defaultValue, bool throwOnError, RegistryHive registryHive, bool ignoreArchitecture=false)
        {
            Exception error;
            do
            {
                RegistryKey hiveKey = GetHiveKeyForProcess(registryHive, ignoreArchitecture);
                // Flip the value to indicate completion after at most two tries
                ignoreArchitecture = !ignoreArchitecture;
                RegistryKey regKey = hiveKey.OpenSubKey(key, false);

                try
                {
                    if (regKey == null)
                    {
                        // key missing exception
                        error = new Exception("Could not access registry");
                    }
                    else
                    {
                        object value = regKey.GetValue(valueName);
                        if (value == null)
                        {
                            // value missing exception
                            error = new Exception("Could not access registry value");
                        }
                        else
                        {
                            return value;
                        }
                    }
                }
                finally
                {
                    if (regKey != null)
                    {
                        regKey.Dispose();
                    }

                    hiveKey.Dispose();
                }
            }
            while (!ignoreArchitecture);

            if (throwOnError)
            {
                throw error;
            }
            else
            {
                return defaultValue;
            }
        }

        internal static RegistryKey GetHiveKeyForProcess(RegistryHive registryHive, bool read64BitAlways=false)
        {
            RegistryView registryView = RegistryView.Default;

            if (read64BitAlways)
            {
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    registryView = RegistryView.Registry64;
                }
            }

            return RegistryKey.OpenBaseKey(registryHive, registryView);
        }

        /// <summary>
        /// Writes a registry value under specified key
        /// </summary>
        /// <param name="root"></param>
        /// <param name="registryKeyPath"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="BackEndErrorException"></exception>
        public static void WriteRegistryValue(RegistryKey root, String registryKeyPath, String name, object value)
        {
            AppAssert.AssertNotNull(root, "root");
            AppAssert.AssertNotNull(registryKeyPath, "registryKeyPath");
            AppAssert.AssertNotNull(name, "name");
            AppAssert.AssertNotNull(value, "value");

            try
            {
                using (RegistryKey regkey = root.OpenSubKey(registryKeyPath, true))
                {
                    if (regkey == null)
                    {
                        throw new Exception("Cannot access registry key" + registryKeyPath);
                    }

                    regkey.SetValue(name, value);
                }
            }
            catch (System.IO.IOException ioe)
            {
                throw new Exception("Cannot access registry key" + registryKeyPath);
            }
            catch (System.Security.SecurityException se)
            {
                throw new Exception("Cannot access registry key" + registryKeyPath);
            }
            catch (UnauthorizedAccessException uae)
            {
                throw new Exception("Cannot access registry key" + registryKeyPath);
            }
        }

        /// <summary>
        /// Deletes a registry value under specified key
        /// </summary>
        /// <param name="root"></param>
        /// <param name="registryKeyPath"></param>
        /// <param name="name"></param>
        /// <exception cref="BackEndErrorException"></exception>
        public static void DeleteRegistryValue(RegistryKey root, String registryKeyPath, String name)
        {
            AppAssert.AssertNotNull(root, "root");
            AppAssert.AssertNotNull(registryKeyPath, "registryKeyPath");
            AppAssert.AssertNotNull(name, "name");

            try
            {
                using (RegistryKey regkey = root.OpenSubKey(registryKeyPath, true))
                {
                    if (regkey != null)
                    {
                        regkey.DeleteValue(name, false);
                    }
                }
            }
            catch (System.Security.SecurityException se)
            {
                throw new Exception("Cannot access registry key" + registryKeyPath);
            }
            catch (UnauthorizedAccessException uae)
            {
                throw new Exception("Cannot access registry key" + registryKeyPath);
            }
        }
    }
}

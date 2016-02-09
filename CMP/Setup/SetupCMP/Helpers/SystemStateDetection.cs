using CMP.Setup.SetupFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMP.Setup.Helpers
{
    class SystemStateDetection
    {
        /// <summary>
        /// Checks the installed components.
        /// </summary>
        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public static void CheckInstalledComponents()
        {
            // Check if CMP Service is installed
            if (CheckProductByUpgradeCode(SetupConstants.GetUpgradeCode(SetupFeatures.Server), true))
            {
                SetupLogger.LogInfo("CheckInstalledComponents: CMP server found");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ServerVersion, "1");
                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, SetupConstants.GetServerInstallPath());
            }

            // Check if Tenant Extension is installed
            if (CheckProductByUpgradeCode(SetupConstants.GetUpgradeCode(SetupFeatures.TenantExtension), true))
            {
                SetupLogger.LogInfo("CheckInstalledComponents: Tenant extension found");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantExtensionVersion, "1");
                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, SetupConstants.GetTenantExtensionInstallPath());
            }

            // Check if Extension Common Components are installed
            if (CheckProductByUpgradeCode(SetupConstants.GetUpgradeCode(SetupFeatures.ExtensionCommon), true))
            {
                SetupLogger.LogInfo("CheckInstalledComponents: Extension common components found");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommonVersion, "1");
                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, SetupConstants.GetExtensionCommonInstallPath());
            }

            // Determine if the VMM WebPortal msi is already installed.
            if (CheckProductByUpgradeCode(SetupConstants.GetUpgradeCode(SetupFeatures.AdminExtension), true))
            {
                SetupLogger.LogInfo("CheckInstalledComponents: Admin extension found");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminExtensionVersion, "1");
                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, SetupConstants.GetAdminExtensionInstallPath());
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="upgradeCode"></param>
        /// <param name="recordVMMDifferentVersionInstalled">If it is true, record that a different version of vNext feature is installed if there is one.</param>
        /// <returns></returns>
        public static bool CheckProductByUpgradeCode(String upgradeCode, bool recordVMMDifferentVersionInstalled)
        {
            bool foundAtleastOne = false; //flag to check if a valid version is found
            Version maxInstalled = new Version(0, 0, 0, 0); //set to VERSION_MIN initially

            List<string> productCodes = SystemStateDetection.CheckProductByUpgradeCode(upgradeCode, ref maxInstalled);
            foundAtleastOne = productCodes != null && productCodes.Count > 0;
            if (recordVMMDifferentVersionInstalled &&
                foundAtleastOne &&
                maxInstalled.CompareTo(System.Reflection.Assembly.GetEntryAssembly().GetName().Version) < 0) // only deal with upgrade with version before current verion; In Update Rollup, this version has to set to RTM version if SetupVM.exe is to be patched.
            {
                    // Upgrade not supported
                    SetupLogger.LogInfo("Incompatible version of VMM has been detected.");
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.VMMUnsupportedVersionInstalled, "1");
            }

            return foundAtleastOne;
        }

        /// <summary>
        /// Check installed products with Upgrade code
        /// </summary>
        /// <param name="upgradeCode"></param>
        /// <param name="foundAtleastOneVersion"></param>
        /// <param name="maxInstalledVersion"></param>
        /// <returns></returns>
        public static List<string> CheckProductByUpgradeCode(String upgradeCode, ref Version maxInstalledVersion)
        {
            //initializations
            List<string> result = new List<string>();
            string productCode = String.Empty;
            UInt32 error = 0;//for holding return values from native APIs
            StringBuilder queryVersion = new StringBuilder(SetupConstants.MaximumVersionStringLength);
            UInt32 bufferLength = (UInt32)queryVersion.Capacity;

            int index = 0;
            UInt32 enumRelatedProdResult = 0;
            StringBuilder productCodeBuffer = new StringBuilder(SetupConstants.MaximumUpgradeCodeLength);

            enumRelatedProdResult = CMP.Setup.Helpers.NativeMethods.MsiEnumRelatedProducts(upgradeCode, 0, index, productCodeBuffer);

            while (CMP.Setup.Helpers.NativeMethods.ERROR_SUCCESS == enumRelatedProdResult)
            {
                bufferLength = SetupConstants.MaximumVersionStringLength;
                error = CMP.Setup.Helpers.NativeMethods.MsiGetProductInfo(productCodeBuffer.ToString(),
                                                            CMP.Setup.Helpers.NativeMethods.INSTALLPROPERTY_VERSIONSTRING,
                                                            queryVersion,
                                                            ref bufferLength);
                if (0 == error)
                {
                    // success 
                    SetupLogger.LogInfo("CheckProductByUpgradeCode : found product with upgrade code {0} with version {1}", upgradeCode, queryVersion.ToString());
                    Version obtainedVersion = new Version(queryVersion.ToString());
                    SetupLogger.LogInfo("CheckProductByUpgradeCode: Installed Version {0}", queryVersion.ToString());
                    int res = obtainedVersion.CompareTo(maxInstalledVersion);
                    AppAssert.Assert(res != 0, "Cannot get two related products with same version! Something must be bad with your code");
                    if (res > 0)
                    {
                        maxInstalledVersion = obtainedVersion;
                    }

                    productCode = productCodeBuffer.ToString();
                    if (!result.Contains(productCode))
                    {
                        result.Add(productCode);
                    }
                }
                else
                {
                    //failed
                    SetupLogger.LogError("CheckProductByUpgradeCode : MsiGetProductInfo failed. Msi Error is {0}", error);
                }
                index++;
                enumRelatedProdResult = CMP.Setup.Helpers.NativeMethods.MsiEnumRelatedProducts(upgradeCode, 0, index, productCodeBuffer);
            }//end of while

            return result;
        }
    }
}

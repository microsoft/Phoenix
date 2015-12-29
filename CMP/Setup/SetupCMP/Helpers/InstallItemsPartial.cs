using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CMP.Setup.SetupFramework
{
    /// <summary>
    /// Addes additional members and functions to the InstallItemsInstallDataItem class
    /// </summary>
    public partial class InstallItemsInstallDataItem
    {
        private bool processItem;
        private bool installSuccessful;
        private InstallDataInputs itemWeAreInstallingEnumValue;
        private InstallDataInputs installTypeEnumValue;

        #region Enums
        /// <summary>
        /// 
        /// </summary>
        [FlagsAttribute]
        public enum InstallDataInputs
        {
            /// <summary>
            /// Not set
            /// </summary>
            None = 0,

            /// <summary>
            /// Used to show that we are installing
            /// </summary>
            Installing = 1,

            /// <summary>
            /// Used to show that we are uninstalling
            /// </summary>
            Uninstalling = 2,

            /// <summary>
            /// Used to show that this is progress only
            /// </summary>
            ProgressOnly = 64,

            /// <summary>
            /// Used to show that this is an executable install
            /// </summary>
            ExecutableInstall = 128,

            /// <summary>
            /// Used to show that this is a MSI install
            /// </summary>
            MicrosoftInstaller = 256,

            /// <summary>
            /// Used to show that methods will be executed during install
            /// </summary>
            CustomAction = 512,

            /// <summary>
            /// Used to show that this item is a Post Install Item
            /// </summary>
            PostInstallItem = 1024,

            /// <summary>
            /// Used to show that this item is not fatal to the install chain
            /// </summary>
            ItemNotFatal = 2048,

            /// <summary>
            /// Used to show that we are initializing progress
            /// </summary>
            InitializeProgress = 4096,

            /// <summary>
            /// Used to show that we are finializing progress
            /// </summary>
            FinalizeProgress = 8192,

        }

        public enum ParentItemType
        {
            [System.Xml.Serialization.XmlEnum("Server")]
            Server,
            [System.Xml.Serialization.XmlEnum("Client")]
            Client,
            [System.Xml.Serialization.XmlEnum("Portal")]
            Portal,
            [System.Xml.Serialization.XmlEnum("FinalConfiguration")]
            FinalConfiguration,
            [System.Xml.Serialization.XmlEnum("Upgrade")]
            Upgrade
        }
        #endregion

        public override string ToString()
        {
            // CONSIDER: Show more stuff 
            return DisplayTitle;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [process item].
        /// </summary>
        /// <value><c>true</c> if [process item]; otherwise, <c>false</c>.</value>
        /// <remarks/>
        [XmlIgnore]
        public bool ProcessItem
        {
            get
            {
                return this.processItem;
            }
            set
            {
                this.processItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [install successful].
        /// </summary>
        /// <value><c>true</c> if [install successful]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool InstallSuccessful
        {
            get
            {
                return this.installSuccessful;
            }
            set
            {
                this.installSuccessful = value;
            }
        }

        /// <summary>
        /// Gets the itemweareinstalling enum value.
        /// </summary>
        /// <value>The itemweareinstalling enum value.</value>
        [XmlIgnore]
        public InstallDataInputs ItemWeAreInstallingEnumValue
        {
            get
            {
                if (0 == (this.itemWeAreInstallingEnumValue | InstallDataInputs.None))
                {
                    InstallDataInputs temp = InstallDataInputs.None;

                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.Installing.ToString()))
                    {
                        temp = temp | InstallDataInputs.Installing;
                    }
                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.ExecutableInstall.ToString()))
                    {
                        temp = temp | InstallDataInputs.ExecutableInstall;
                    }
                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.MicrosoftInstaller.ToString()))
                    {
                        temp = temp | InstallDataInputs.MicrosoftInstaller;
                    }
                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.CustomAction.ToString()))
                    {
                        temp = temp | InstallDataInputs.CustomAction;
                    }
                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.PostInstallItem.ToString()))
                    {
                        temp = temp | InstallDataInputs.PostInstallItem;
                    }
                    if (this.itemWeAreInstallingField.Contains(InstallDataInputs.ItemNotFatal.ToString()))
                    {
                        temp = temp | InstallDataInputs.ItemNotFatal;
                    }
                    this.itemWeAreInstallingEnumValue = temp;
                }
                return this.itemWeAreInstallingEnumValue;
            }
            set
            {
                this.itemWeAreInstallingEnumValue = value;
            }
        }

        /// <summary>
        /// Gets the installtype enum value.
        /// </summary>
        /// <value>The installtype enum value.</value>
        [XmlIgnore]
        public InstallDataInputs InstallTypeEnumValue
        {
            get
            {
                if (0 == (this.installTypeEnumValue | InstallDataInputs.None))
                {
                    InstallDataInputs temp = InstallDataInputs.None;

                    if (this.installTypeField.Contains(InstallDataInputs.ExecutableInstall.ToString()))
                    {
                        temp = temp | InstallDataInputs.ExecutableInstall;
                    }
                    if (this.installTypeField.Contains(InstallDataInputs.MicrosoftInstaller.ToString()))
                    {
                        temp = temp | InstallDataInputs.MicrosoftInstaller;
                    }
                    if (this.installTypeField.Contains(InstallDataInputs.CustomAction.ToString()))
                    {
                        temp = temp | InstallDataInputs.CustomAction;
                    }
                    if (this.installTypeField.Contains(InstallDataInputs.PostInstallItem.ToString()))
                    {
                        temp = temp | InstallDataInputs.PostInstallItem;
                    }
                    this.installTypeEnumValue = temp;
                }
                return this.installTypeEnumValue;
            }
        }
    }
}
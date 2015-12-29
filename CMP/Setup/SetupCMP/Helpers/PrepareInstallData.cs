//---------------------------------------------------------------------------
// <copyright file="PrepareInstallData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This file contains PrepareInstallData class
// </summary>
//---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CMP.Setup.SetupFramework;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    public sealed class PrepareInstallData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepareInstallData"/> class.
        /// </summary>
        private PrepareInstallData()
        {
            //Do nothing
        }

        /// <summary>
        /// Prepares the install data items.
        /// </summary>
        public static void PrepareInstallDataItems()
        {
            ArrayList itemsToInstall = new ArrayList();
            foreach (KeyValuePair<string, InstallItemsInstallDataItem> valuePair in InstallDataItemRegistry.Instance.InstallDataItems)
            {
                //If there is a property in the property bag that matches the name of this InstallDataItem
                //then we should add it to the list to install.
                if (PropertyBagDictionary.Instance.PropertyExists(valuePair.Key))
                {
                    itemsToInstall.Add(valuePair.Value);
                }
                else
                {
                    SetupLogger.LogInfo("No match for {0} found.", valuePair.Key);
                }
            }

            // if this is an uninstall, we should reverse the order of everything except the postinstall guy... that must stay in the same place.
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                itemsToInstall.Reverse(0, itemsToInstall.Count-1);
            }

            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ItemsToInstall, itemsToInstall);

            SetupLogger.LogInfo("PrepareInstallDataItems: Install Item list:");
            SetupLogger.LogInfo("***************************");
            foreach (InstallItemsInstallDataItem dataItem in itemsToInstall)
            {
                SetupLogger.LogInfo(dataItem.DisplayTitle);
            }
            SetupLogger.LogInfo("***************************");
        }
    }
}

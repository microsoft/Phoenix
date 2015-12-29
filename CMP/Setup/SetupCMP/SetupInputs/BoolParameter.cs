// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Diagnostics;
using CMP.Setup.Helpers;

namespace CMP.Setup
{

    /// <summary>
    /// BoolParameter can read 1 or 0 from ini file
    /// </summary>
    public abstract class BoolParameter : InputParameter
    {
        public BoolParameter(String iniTag,
            bool mandatory,
            bool defaultValue)
            : base(iniTag, mandatory, defaultValue)
        {

        }

        /// <summary>
        /// Read the bool value from input ini file
        /// </summary>
        /// <param name="file">ini file</param>
        /// <returns></returns>
        protected override object LoadInputValueFromFile(String file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            int newInputValue = (int)CMP.Setup.Helpers.NativeMethods.GetPrivateProfileInt(SetupInputTags.SectionTag,
                this.iniTag,
                (int)-1,
                file);

            // if not defined in the file
            if (newInputValue == -1)
            {
                if (this.mandatory)
                {
                    // throw error
                    throw new Exception(
                        String.Format("The parameter {0} is missing", this.iniTag));
                }
                else
                {
                    return this.defaultValue;
                }
            }

            // defined in the file:
            if (newInputValue != 1 && newInputValue != 0)
            {
                throw new Exception(String.Format("{0} is neither 1 nor 0", this.iniTag));
            }

            return (newInputValue == 1);
        }
    }
}

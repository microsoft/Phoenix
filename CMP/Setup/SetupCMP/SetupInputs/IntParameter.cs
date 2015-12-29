// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Diagnostics;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    /// <summary>
    /// IntParameter can read an integer from ini file
    /// </summary>
    public abstract class IntParameter : InputParameter
    {
        public IntParameter(String iniTag,
            bool mandatory,
            int defaultValue) : base(iniTag, mandatory, defaultValue) 
        {
            
        }

        protected override object LoadInputValueFromFile(String file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            int newInputValue = (int)CMP.Setup.Helpers.NativeMethods.GetPrivateProfileInt(
                SetupInputTags.SectionTag,
                this.iniTag,
                (int)-1,
                file);

            if (newInputValue == -1)
            {
                if (this.mandatory)
                {
                    // throw error
                    throw new Exception(String.Format("The parameter {0} is missing", this.iniTag));
                }
                else
                {
                    newInputValue = (int)this.defaultValue;
                }
            }
            return newInputValue;
        }
    }
}

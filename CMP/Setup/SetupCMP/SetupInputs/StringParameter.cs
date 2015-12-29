// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Text;
using System.Diagnostics;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    /// <summary>
    /// StringParameter can read a string from ini file
    /// </summary>
    public abstract class StringParameter : InputParameter
    {   
        public StringParameter(String iniTag,
            bool mandatory,
            String defaultValue) : base(iniTag, mandatory, defaultValue) 
        {
            
        }

        protected override object LoadInputValueFromFile(String file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            StringBuilder valueBuffer = new StringBuilder(SetupInputsConstants.MaxStringLength);

            CMP.Setup.Helpers.NativeMethods.GetPrivateProfileString(SetupInputTags.SectionTag,
                this.iniTag,
                (string)defaultValue,
                valueBuffer,
                (uint)valueBuffer.MaxCapacity,
                file);

            if (this.mandatory == true)
            {
                if (valueBuffer == null || valueBuffer.Length == 0)
                {
                    // throw error
                    throw new Exception(String.Format("The parameter {0} is missing", this.iniTag));
                }
            }

            return (valueBuffer == null || valueBuffer.Length == 0) ? null : valueBuffer.ToString().Trim();
        }
    }
}

// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Text;
using System.Diagnostics;
using System.Security;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    public abstract class SecureStringParameter : InputParameter
    {
        public SecureStringParameter(String iniTag,
            bool mandatory,
            SecureString defaultValue)
            : base(iniTag, mandatory, defaultValue)
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
            
            SecureString secureValueBuffer = new SecureString();
            secureValueBuffer.Clear();
            foreach (char ch in valueBuffer.ToString())
            {
                secureValueBuffer.AppendChar(ch);
            }            
            
            return (secureValueBuffer == null || secureValueBuffer.Length == 0) ? null : secureValueBuffer;
        }
    }
}
using CMP.Setup.Helpers;
// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Diagnostics;
using System.Security;

namespace CMP.Setup
{
    /// <summary>
    /// root of all other input parameter classes.
    /// </summary>
    public abstract class InputParameter
    {
        protected object inputValue;
        protected object defaultValue;
        protected String iniTag;
        protected bool mandatory;
        protected bool valid;

        public InputParameter(String iniTag,
            bool mandatory,
            object defaultValue)
        {
            this.iniTag = iniTag;
            this.mandatory = mandatory;
            this.defaultValue = defaultValue;
            this.valid = false;
        }

        virtual public String IniTag
        {
            get
            {
                return iniTag;
            }
        }

        public void LoadInputValue(String file)
        {
            AppAssert.AssertNotNull(file, "file");
            object inputValue = LoadInputValueFromFile(file);
            if (inputValue != null)
            {
                SetInputValue(inputValue);
            }
        }

        protected abstract object LoadInputValueFromFile(String file);

        public void SetInputValue(object inputValue)
        {
            AppAssert.AssertNotNull(inputValue, "inputValue");

            Validate(inputValue);
            this.inputValue = inputValue;
            this.valid = true;
        }

        virtual public object InputValue
        {
            get
            {
                return this.inputValue;
            }
        }

        virtual public bool Valid
        {
            get
            {
                return this.valid;
            }
        }

        virtual public bool CanLogToFile
        {
            get
            {
                return true;
            }
        }

        public abstract void Validate(object newInputValue);

        virtual public void ResetToDefault()
        {
            // Initialise the input to default
            if (this.defaultValue != null)
            {
                SetInputValue(this.defaultValue);
            }
        }

        public static implicit operator int(InputParameter inputParameter)
        {
            return (int)inputParameter.InputValue;
        }

        public static implicit operator bool(InputParameter inputParameter)
        {
            return (bool)inputParameter.InputValue;
        }

        public static implicit operator String(InputParameter inputParameter)
        {
            return (String)inputParameter.InputValue;
        }

        public static implicit operator SecureString(InputParameter inputParameter)
        {
            SecureString secureString;
            if (inputParameter.InputValue is SecureString)
            {
                secureString = (SecureString)inputParameter.InputValue;
            }
            else
            {
                secureString = new SecureString();
                if (inputParameter != null && inputParameter.InputValue != null)
                {
                    char[] newInputValueChars = ((string)inputParameter).ToCharArray();

                    foreach (char inputValueChar in newInputValueChars)
                    {
                        secureString.AppendChar(inputValueChar);
                    }
                }
            }

            return secureString;
        }
    }
}

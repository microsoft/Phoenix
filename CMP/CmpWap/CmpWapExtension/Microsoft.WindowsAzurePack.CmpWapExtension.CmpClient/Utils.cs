//*********************************************************************
//*
//* File:
//* Author: Mark West (markwes@microsoft.com)
//*
//*********************************************************************

using System;
using System.Reflection;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    //*********************************************************************
    /// 
    /// <summary>
    /// Utility classes for the CMPApiClient
    /// </summary>
    /// 
    //*********************************************************************
    class Utils
    {
        //*********************************************************************
        ///
        /// <summary>
        /// Unwinds exception messages
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The unwound messages</returns>
        /// 
        //*********************************************************************

        public static string UnwindExceptionMessages(Exception ex)
        {
            var message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    message += " - " + ex.Message;
                }
            }

            return message;
        }
    }

    //*********************************************************************
    ///
    /// <summary>
    /// Utility class to hold the string value of an Enum object
    /// </summary>
    /// 
    //*********************************************************************
    /* to do: move this to a separate file */
    public class StringValue : System.Attribute
    {
        private string _value;

        /// <summary>
        /// Initializes object Value to the passed in string value
        /// </summary>
        /// <param name="value"></param>
        public StringValue(string value)
        {
            _value = value;
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// String value of StringValue
        /// </summary>
        /// 
        //*********************************************************************
        public string Value
        {
            get { return _value; }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the string value of an Enum object
        /// </summary>
        /// <param name="value">Enum object to get the string value of</param>
        /// <returns>String value of the Enum object</returns>
        /// 
        //*********************************************************************

        public static string GetStringValue(Enum value)
        {
            string output = null;
            var type = value.GetType();

            //Check first in our cached results...

            //Look for our 'StringValueAttribute' 

            //in the field's custom attributes

            var fi = type.GetField(value.ToString());
            var attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}


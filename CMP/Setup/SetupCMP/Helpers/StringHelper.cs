//+--------------------------------------------------------------
//
//  Description: Functions to help in dealing with String objects
//
//---------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CMP.Setup.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Format a list of parameters for user display, e.g.
        /// {1,2,3} -> "1, 2, 3"
        /// </summary>
        /// <param name="parameterStrings"></param>
        /// <returns></returns>
        public static string ListToCommaSeparatedString(IEnumerable<string> parameterStrings)
        {
            string separator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            return StringHelper.FormatParameterListUsingDelimiter(parameterStrings, string.Format("{0} ", separator));
        }

        /// <summary>
        /// This method converts a list of string items into a 'delimeter' separated string.
        /// E.g. {a,b,c,d} with "|" as delimiter ==> a|b|c|d
        ///      {a,b,c,d} with ", " as delimiter ==> a, b, c, d
        /// If any of the items in the array is empty or null string, this method would skip them.
        /// E.g. {a,null,c} with "|" ==> a|c
        /// Usages:
        /// a) It could be use to format virtual machine names e.g. "vm-1, vm2, vm-xp-3" to show on UI.
        /// b) It could be used to generate pipe separated monad command line input parameter list.
        ///
        /// NOTE: Please specify a delimiter included in the resources if you are planning to show the string on the UI.
        /// This would avoid any localization issues.
        /// </summary>
        public static string FormatParameterListUsingDelimiter(IEnumerable<string> items, string delimiter)
        {
            StringBuilder builder = new StringBuilder();

            if (items != null)
            {
                foreach (string item in items)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    if (builder.Length > 0)
                    {
                        builder.Append(delimiter);
                    }
                    builder.Append(item.Trim());
                }
            }

            return builder.ToString();
        }
    }
}

//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//
//*****************************************************************************

using System;
using System.Text;
using System.Xml;
using System.IO;

namespace AzureAdminClientLib
{
    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    public static class Util
    {
        /// <summary>
        /// Converts a UTF-8 string to a Base-64 version of the string.
        /// </summary>
        /// <param name="s">The string to convert to Base-64.</param>
        /// <returns>The Base-64 converted string.</returns>
        public static string ToB64(this string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a Base-64 encoded string to UTF-8.
        /// </summary>
        /// <param name="s">The string to convert from Base-64.</param>
        /// <returns>The converted UTF-8 string.</returns>
        public static string FromB64(this string s)
        {
            var bytes = Convert.FromBase64String(s);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static string PrettyPrintXml(XmlDocument xDoc)
        {
            var niceString = new StringBuilder();
            var strWriter = new StringWriter(niceString);
            var xmlWriter = new XmlTextWriter(strWriter) 
                {Formatting = Formatting.Indented};

            xDoc.WriteTo(xmlWriter);

            xmlWriter.Close();
            strWriter.Close();

            return niceString.ToString();
        }
    }
}

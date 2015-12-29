using CMP.Setup.Helpers;
//---------------------------------------------------------------------------
// <copyright file="PathHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This file contains PathHelper class
// </summary>
//---------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;

namespace CMP.Setup
{
    /// <summary>
    /// Summary description for PathHelper.
    /// </summary>
    public class PathHelper
    {
        private PathHelper()
        {
        }

        /// <summary>
        /// Given a path, return the root of the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true, if path has a root, false if path does not have a root</returns>
        public static bool IsPathRooted(String path)
        {
            AppAssert.AssertNotNull(path, "path");
            AppAssert.Assert(path.Length > 0, "Path is empty");
            
            if (path.Length > 1 && path[1] == Path.VolumeSeparatorChar)
            {
                if (path.Length > 2 && path[2] == Path.DirectorySeparatorChar)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Given a path, return the root of the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>"\" terminated root</returns>
        public static String GetPathRoot(String path)
        {
            AppAssert.AssertNotNull(path, "path");
            AppAssert.Assert(path.Length > 0, "Path is empty");
            AppAssert.Assert(path[0] != Path.DirectorySeparatorChar, "Path starts with \\");

            if (IsPathRooted(path))
            {
                return path.Substring(0, 3);
            }
            else
            {
                throw new ArgumentException("path");
            }
        }

        /// <summary>
        /// Adds quotes (") to a string at beginning and end if not already quoted
        /// </summary>
        /// <param name="stringToQuote"></param>
        /// <returns>Quoted string.</returns>
        public static string QuoteString(string stringToQuote)
        {
            AppAssert.Assert(null != stringToQuote, "Null string passed to stringToQuote");
            string doubleQuote = "\"";
            if (!(stringToQuote.StartsWith(doubleQuote) && stringToQuote.EndsWith(doubleQuote)))
            {
                return String.Format("{0}{1}{0}", doubleQuote, stringToQuote);
            }
            else
            {
                return stringToQuote;
            }
        }

        /// <summary>
        /// Iterates through the given path and returns the existing valid parent folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String GetExistingAncestor(String path)
        {
            AppAssert.AssertNotNull(path, "path");

            // Check if the path exists
            if (Directory.Exists(path))
            {
                SetupLogger.LogInfo("InstallLocationPage.SetPathInBrowserDialog : The path {0} exists, returning", path);
                return path;
            }

            String ancestor = null;
            int index = path.LastIndexOf(Path.DirectorySeparatorChar);
            while (index >= 0)
            {
                ancestor = path.Substring(0, index);
                SetupLogger.LogInfo("InstallLocationPage.SetPathInBrowserDialog : Check if the ancestor {0} exists", ancestor);
                if (Directory.Exists(ancestor))
                {
                    SetupLogger.LogInfo("InstallLocationPage.SetPathInBrowserDialog : The ancestor {0} exists, returning", ancestor);
                    return (ancestor + Path.DirectorySeparatorChar);
                }

                index = ancestor.LastIndexOf(Path.DirectorySeparatorChar);
            }
            return null;
        }
    }
}

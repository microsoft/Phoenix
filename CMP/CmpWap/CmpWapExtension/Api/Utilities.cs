//*****************************************************************************
// File: Utilities.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains utilities methids
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    /// <remarks>
    ///     This class contains utilities methids
    /// </remarks>
    public class Utilities
    {
        //*********************************************************************
        ///
        /// <summary>
        /// This method unwinds exception messages
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The unwound messages</returns>
        /// 
        //*********************************************************************

        public static string UnwindExceptionMessages( Exception ex )
        {
            string message = ex.Message;

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
}

//*****************************************************************************
// File: ILogger
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: An interface for logging.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Logging
{
    public interface ILogger
    {
        void Log(Exception ex, EventLogEntryType type,
            int id, short category);
        void Log(string message, EventLogEntryType type,
            int id, short category);
    }
}
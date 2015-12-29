//*****************************************************************************
// File: Logger.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods for logging.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Logging
{
    public class Logger:ILogger
    {


        static EventLog _eventLog;

        /// <summary></summary>
        public static EventLog eLog
        {
            set { _eventLog = value; }
            get
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application");
                        _eventLog.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="ex"></param>
        ///  <param name="type"></param>
        /// <param name="id"></param>
        ///  <param name="category"></param>
        ///  
        //*********************************************************************
        public void Log(Exception ex, EventLogEntryType type,
            int id, short category)
        {
            try
            {
                if (null != eLog)
                    eLog.WriteEntry(CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
        public void Log(string message, EventLogEntryType type,
            int id, short category)
        {
            try
            {
                if (null != eLog)
                    eLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
    }
}
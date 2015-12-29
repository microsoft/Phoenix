//+--------------------------------------------------------------
//  Copyright(c) Microsoft Corporation, 2002-2006
//
//  Description: StackTraceInfo
//
//---------------------------------------------------------------

namespace CMP.Setup.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Text;

    /// <summary>
    /// StackTraceInfo
    /// </summary>
    public sealed class StackTraceInfo
    {
        //disable the ctor
        private StackTraceInfo()
        {
        }

        /// <summary>
        /// Dumps the stack trace.
        /// </summary>
        /// <param name="skipFrames">the number of frames to skip</param>
        /// <returns>stack trace</returns>
        static public string DumpStackTrace(int skipFrames)
        {
            StringBuilder str = new StringBuilder();

            StackTrace stackTrace = new StackTrace(skipFrames, true);
            for(int i=0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);

                str.AppendFormat("FileName:{0}; Method:{1}(); lineNo:{2}; ilOffset:{3}.\r\n",
                                            frame.GetFileName(),
                                            frame.GetMethod().Name,
                                            frame.GetFileLineNumber(),
                                            frame.GetILOffset());
            }

            return str.ToString();
        }

        /// <summary>
        /// Dumps the current line info
        /// </summary>
        /// <param name="skipFrames">the number of frames to skip</param>
        /// <returns></returns>
        static public string DumpLineInfo(int skipFrames)
        {
            StackTrace stackTrace = new StackTrace(skipFrames, true);
            StackFrame frame = stackTrace.GetFrame(0);

            return string.Format("(FileName:{0}; Method:{1}(); lineNo:{2}; ilOffset:{3})",
                frame.GetFileName(),
                frame.GetMethod().Name,
                frame.GetFileLineNumber(),
                frame.GetILOffset());
        }
    }
}

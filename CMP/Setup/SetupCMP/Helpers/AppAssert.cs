//+--------------------------------------------------------------
//
//  Description: AppAssert
//
//---------------------------------------------------------------

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CMP.Setup.Helpers
{
    /// <summary>
    /// This exception signifies an assertion failure.
    /// It should never be caught.
    /// </summary>
    [Serializable]
    public class AssertionFailedException : ApplicationException
    {
        private const string LineInfoName = "AssertionLineInfo";
        private const string StackTraceName = "AssertionStackTrace";

        private string lineInfo;
        private string stackTrace;

        public AssertionFailedException(string message, string lineInfo, string stackTrace) :
            base(message)
        {
            this.lineInfo = lineInfo;
            this.stackTrace = stackTrace;
        }

            /// <summary>
        /// DeSerialization constructor
        /// </summary>
        protected AssertionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.lineInfo = info.GetString(AssertionFailedException.LineInfoName);
            this.stackTrace = info.GetString(AssertionFailedException.StackTraceName);
        }

        public override string ToString()
        {
            StringBuilder description = new StringBuilder();
            description.AppendFormat("{0}\r\n", this.Message);
            description.AppendFormat("ASSERT: {0}\r\n", this.lineInfo);
            description.AppendFormat("STACK TRACE:\r\n{0}", this.stackTrace);
            return description.ToString();
        }
    }

    /// <summary>
    /// Checks for a condition and displays a assert if the condition is false.
    /// </summary>
    public static class AppAssert
    {
        /// <summary>
        /// Asserts that the passed object is not null.  Issues message if it is null.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <param name="parameterName">The parameter name</param>
        [Conditional("DEBUG")]
        static public void AssertNotNull(object parameter, string parameterName)
        {
            AppAssert.InternalAssert(3, parameter != null,
                "The parameter '{0}' is null.", parameterName);
        }

        /// <summary>
        /// Asserts that the passed object is not null.  Issues assert if it is null.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        [Conditional("DEBUG")]
        static public void AssertNotNull(object parameter)
        {
            AppAssert.InternalAssert(3, parameter != null, "Null argument");
        }

        /// <summary>
        /// Asserts the condition, issues assert if condition is false
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="message">Format string of message</param>
        /// <param name="formatParameters">message parameters</param>
        [Conditional("DEBUG")]
        static public void Assert(bool condition, string format, params object[] formatArgs)
        {
            AppAssert.InternalAssert(3, condition, format, formatArgs);
        }

        /// <summary>
        /// Asserts the condition, issues assert if condition is false
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="message">Format string of message</param>
        [Conditional("DEBUG")]
        static public void Assert(bool condition, string message)
        {
            AppAssert.InternalAssert(3, condition, message);
        }

        /// <summary>
        /// Asserts the condition, issues assert if condition is false
        /// </summary>
        /// <param name="condition">condition</param>
        [Conditional("DEBUG")]
        static public void Assert(bool condition)
        {
            AppAssert.InternalAssert(3, condition, "Assertion failed");
        }

        /// <summary>
        /// Asserts that a condition has failed.
        /// </summary>
        /// <param name="message">Format string of message</param>
        /// <param name="formatParameters">message parameters</param>
        [Conditional("DEBUG")]
        static public void Fail(string message = null, params object[] formatArgs)
        {
            message = message ?? "Assertion failed";
            AppAssert.InternalAssert(3, false, message, formatArgs);
        }

        [Conditional("DEBUG")]
        static private void InternalAssert(int stackFrameIdx, bool condition, string format,
            params object[] formatArgs)
        {
            if (!condition)
            {
                // Create an exception that contains information for diagnosis.
                Exception assertion = new AssertionFailedException(
                    string.Format(format, formatArgs),
                    StackTraceInfo.DumpLineInfo(stackFrameIdx),
                    StackTraceInfo.DumpStackTrace(stackFrameIdx));

                // If a debugger is attached, break in
                if (Debugger.IsAttached)
                {
                    //
                    // This is a debugging aid when an assertion has been reached while you are in the process of debugging.
                    // Go back down the call stack 2 frames to see what has caused the assertion.
                    // To ignore the assertion and let the program crash select to continue (F5).
                    //
                    Debugger.Break();
                }

                // Halt the product with an unhandled exception.
                throw assertion;
            }
        }

        [Conditional("DEBUG")]
        static private void InternalAssert(int stackFrameIdx, bool condition, string text)
        {
            if (!condition)
            {
                // Create an exception that contains information for diagnosis.
                Exception assertion = new AssertionFailedException(
                    text,
                    StackTraceInfo.DumpLineInfo(stackFrameIdx),
                    StackTraceInfo.DumpStackTrace(stackFrameIdx));

                // If a debugger is attached, break in
                if (Debugger.IsAttached)
                {
                    //
                    // This is a debugging aid when an assertion has been reached while you are in the process of debugging.
                    // Go back down the call stack 2 frames to see what has caused the assertion.
                    // To ignore the assertion and let the program crash select to continue (F5).
                    //
                    Debugger.Break();
                }

                // Halt the product with an unhandled exception.
                throw assertion;
            }
        }

        [Conditional("DEBUG")]
        static private void InternalAssert(int stackFrameIdx, bool condition, string format, object arg0)
        {
            if (!condition)
            {
                // Create an exception that contains information for diagnosis.
                Exception assertion = new AssertionFailedException(
                    string.Format(format, arg0),
                    StackTraceInfo.DumpLineInfo(stackFrameIdx),
                    StackTraceInfo.DumpStackTrace(stackFrameIdx));

                // If a debugger is attached, break in
                if (Debugger.IsAttached)
                {
                    //
                    // This is a debugging aid when an assertion has been reached while you are in the process of debugging.
                    // Go back down the call stack 2 frames to see what has caused the assertion.
                    // To ignore the assertion and let the program crash select to continue (F5).
                    //
                    Debugger.Break();
                }

                // Halt the product with an unhandled exception.
                throw assertion;
            }
        }
    
    
    
    }
}

namespace Phoenix.Test.UI.Framework.Logging
{
    using System;
    using System.IO;

    public static class Log
    {
        /// <summary>
        /// Creates an information log message.
        /// </summary>
        public static void Information(string message, params object[] args)
        {
            Write(LogType.Information, null, message, args);
        }
        
        /// <summary>
        /// Creates a warning log message.
        /// </summary>
        public static void Warning(string message, params object[] args)
        {
            Write(LogType.Warning, null, message, args);
        }
        
        /// <summary>
        /// Creates an error (e.g. application crash) log message.
        /// </summary>
        public static void Error(string message, params object[] args)
        {
            Write(LogType.Error, null, message, args);
        }

        /// <summary>
        /// Creates an error (e.g. application crash) log message, also logging the provided exception.
        /// </summary>
        public static void Error(Exception ex, string message, params object[] args)
        {
            Write(LogType.Error, ex, message, args);
        }
        
        /// <summary>
        /// Writes a log message for the <see cref="LogType"/>, and if the provided Exception is not null,
        /// appends this exception to the message.
        /// </summary>
        private static void Write(LogType errorType, Exception ex, string message, params object[] args)
        {
            message = string.Format(message, args);

            message = string.Format("{0}[{1}]: {2}",
                errorType.ToString(),
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"),
                message);

            if (ex != null)
            {
                message = string.Concat(message, Environment.NewLine, ex.ToString());
            }

            Console.WriteLine(message);
        }

        private enum LogType
        {
            /// <summary>
            /// Information message, a debug message.
            /// </summary>
            Information,
            /// <summary>
            /// A warning log message for when something has failed unexpectedly.
            /// </summary>
            Warning,
            /// <summary>
            /// An error log message, for an unexpected message.
            /// </summary>
            Error,
            /// <summary>
            /// Information output only when DEBUG defined.
            /// </summary>
            Debug,
        }
    }

    
}

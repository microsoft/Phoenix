//+--------------------------------------------------------------
//  Description:This file contains SetupLogger class implementing 
//              the logging functionality for Setup.
//---------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Security;

namespace CMP.Setup.Helpers
{
    /// <summary>
    /// Logging level enumeration
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log errors
        /// </summary>
        Error,

        /// <summary>
        /// Log warnings
        /// </summary>
        Warning,

        /// <summary>
        /// Log information
        /// </summary>
        Info,

        /// <summary>
        /// Log everything
        /// </summary>
        Verbose,
    }

    /// <summary>
    /// Implements logging for setup.
    /// </summary>
    public class SetupLogger
    {
        /// <summary>
        /// Path of log file
        /// </summary>
        private static String logFilePath;

        /// <summary>
        /// Stream writer pointing to the file stream
        /// </summary>
        private static StreamWriter logger;

        /// <summary>
        /// Private object to lock on
        /// </summary>
        private static Object lockObject;

        /// <summary>
        /// Current logging level
        /// </summary>
        private static LogLevel logLevel;

        /// <summary>
        /// Private constructor since we don't want to allow instantiation
        /// </summary>
        private SetupLogger()
        {

        }

        /// <summary>
        /// Initialize the logger
        /// </summary>
        /// <param name="logFilePath"></param>
        public static void Initialize(String logFilePath)
        {
            if (logFilePath == null)
            {
                throw new ArgumentNullException("logFilePath");
            }

            Initialize(logFilePath, LogLevel.Verbose, false);

        }

        /// <summary>
        /// Initialize the logger
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <param name="logLevel"></param>
        public static void Initialize(String logFilePath, LogLevel logLevel)
        {
            if (logFilePath == null)
            {
                throw new ArgumentNullException("logFilePath");
            }

            Initialize(logFilePath, logLevel, false);
        }

        /// <summary>
        /// Initializes the logger
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <param name="logLevel"></param>
        /// <param name="append"></param>
        public static void Initialize(String logFilePath, LogLevel logLevel, bool append)
        {
            if (logFilePath == null)
            {
                throw new ArgumentNullException("logFilePath");
            }

            SetupLogger.logFilePath = logFilePath;
            SetupLogger.logLevel = logLevel;

            try
            {
                // Create the private lock object
                lockObject = new Object();

                // Create a file to write to.
                logger = new StreamWriter(logFilePath, append, Encoding.Unicode);
                logger.AutoFlush = true;
            }
            catch (ArgumentException argumentException)
            {
                throw new Exception("Log file name invalid");
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                throw new Exception("Log file creation failed");
            }
            catch (SecurityException securityException)
            {
                throw new Exception("Log file creation failed");
            }
            catch (PathTooLongException pathTooLongException)
            {
                throw new Exception("Log file name too long");
            }
            catch (IOException ioException)
            {
                throw new Exception("Log file creation failed");
            }
        }

        /// <summary>
        /// Open the file in append mode.
        /// </summary>
        public static void OpenAppend()
        {
            if (logFilePath == null)
            {
                throw new Exception("logFilePath");
            }
            if (logger != null)
            {
                throw new Exception("Logger is already initialized");
            }

            Initialize(SetupLogger.logFilePath, SetupLogger.logLevel, true);
        }

        /// <summary>
        /// Close the logger
        /// </summary>
        public static void Close()
        {
            if (logger != null)
            {
                logger.Flush();
                logger.Close();
                logger = null;
            }
        }

        /// <summary>
        /// Read/Write property for getting/ setting log level
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                return logLevel;
            }
            set
            {
                logLevel = value;
            }
        }

        #region Static Logging Methods

        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            LogException(exception, "");
        }

        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogException(Exception exception, String message, params object[] args)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (logger != null && logLevel >= LogLevel.Error)
            {
                String title = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + "Exception" + " : ";
                String traceMessage = String.Format(message, args) + " => " + exception.ToString();
                lock (lockObject)
                {
                    logger.WriteLine(title + traceMessage);
                }
            }
        }

        /// <summary>
        /// Log the exception if the condition is true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="exception"></param>
        public static void LogExceptionIf(bool condition, Exception exception)
        {
            LogExceptionIf(condition, exception, "");
        }

        /// <summary>
        /// Log the exception if the condition is true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogExceptionIf(bool condition, Exception exception, String message, params object[] args)
        {
            if (condition)
            {
                LogException(exception, message, args);
            }
        }

        /// <summary>
        /// Log an Info message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInfo(String message, params object[] args)
        {
            if (message == null)
            {
                return;
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (logger != null && logLevel >= LogLevel.Info)
            {
                String title = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + "Info" + " : ";
                lock (lockObject)
                {
                    logger.WriteLine(title + String.Format(message, args));
                }
            }
        }

        /// <summary>
        /// Write an Info message if the condition is true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInfoIf(bool condition, String message, params object[] args)
        {
            if (condition)
            {
                LogInfo(message, args);
            }
        }

        /// <summary>
        /// Log values of variable e.g. user inputs.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void LogData(String name, String value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger reference is null", "Logger is not initialized");
            }

            if (logger != null && logLevel >= LogLevel.Verbose)
            {
                String title = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + "Data" + " : ";
                String message = String.Format("{0} = {1}", name, value);
                lock (lockObject)
                {
                    logger.WriteLine(title + message);
                }
            }
        }

        /// <summary>
        /// Log error messages
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(String message)
        {
            if (message == null)
            {
                return;
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger reference is null", "Logger is not initialized");
            }

            if (logger != null && logLevel >= LogLevel.Error)
            {
                String title = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + "Error" + " : ";
                lock (lockObject)
                {
                    logger.WriteLine(title + message);
                }
            }
        }

        /// <summary>
        /// Log error messages after formatting
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(String message, params object[] args)
        {
            if (message == null)
            {
                return;
            }

            LogError(String.Format(message, args));
        }

        /// <summary>
        /// Log warning messages
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(String message)
        {
            if (message == null)
            {
                return;
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger reference is null", "Logger is not initialized");
            }

            if (logger != null && logLevel >= LogLevel.Warning)
            {
                String title = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + "Warning" + " : ";
                lock (lockObject)
                {
                    logger.WriteLine(title + message);
                }
            }
        }

        /// <summary>
        /// Log warning messages after formatting
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(String message, params object[] args)
        {
            if (message == null)
            {
                return;
            }

            LogWarning(String.Format(message, args));
        }

        #endregion
    }
}

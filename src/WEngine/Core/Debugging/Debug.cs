using System;
using System.Collections.Generic;

namespace WEngine
{
    /// <summary>
    /// All the utilies for debugging.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The list of all loggers loaded.
        /// </summary>
        private static List<Logger> _Loggers = new List<Logger>(1);
        /// <summary>
        /// The thread locker for <see cref="Loggers"/>.
        /// </summary>
        private static object _LoggersLockers = new object();

        /// <summary>
        /// Add a debug logger.
        /// </summary>
        /// <param name="logger">The logger object</param>
        public static void AddLogger(Logger logger)
        {
            if (logger == null)
            {
                Debug.LogWarning("<Debug.cs:33> Cannot add a null logger to Debug !");
            }

            lock (_LoggersLockers)
            {
                _Loggers.Add(logger);
            }
        }

        /// <summary>
        /// Remove a debug logger.
        /// </summary>
        /// <param name="logger">The logger to remove.</param>
        public static void RemoveLogger(Logger logger)
        {
            lock (_LoggersLockers)
            {
                _Loggers.Remove(logger);
            }
        }

        /// <summary>
        /// Log an Info/Verbose object.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public static void Log(object message)
        {
            lock (_LoggersLockers)
            {
                foreach (Logger logger in _Loggers)
                {
                    logger.Log(message ?? "Null");
                }
            }
        }

        /// <summary>
        /// Log a Warning object.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public static void LogWarning(object message)
        {
            lock (_LoggersLockers)
            {
                foreach (Logger logger in _Loggers)
                {
                    logger.LogWarning(message ?? "Null");
                }
            }
        }

        /// <summary>
        /// Log an Error object.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public static void LogError(object message)
        {
            lock (_LoggersLockers)
            {
                foreach (Logger logger in _Loggers)
                {
                    logger.LogError(message ?? "Null");
                }
            }
        }

        /// <summary>
        /// Log an Exception object.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        public static void LogException(Exception exception)
        {
            lock (_LoggersLockers)
            {
                foreach (Logger logger in _Loggers)
                {
                    logger.LogException(exception);
                }
            }
        }
    }
}

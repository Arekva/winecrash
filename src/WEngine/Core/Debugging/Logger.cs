using System;

namespace WEngine
{
    /// <summary>
    /// The callback delegate used by the <see cref="Logger"/> to log objects. Mostly used by <see cref="Debug"/>.
    /// <br>Use <see cref="Logger.Logger(LoggerCallback, LoggerCallback, LoggerCallback, LoggerCallback)"/></br> to set new callbacks.
    /// </summary>
    /// <param name="message"></param>
    public delegate void LoggerCallback(object message);

    /// <summary>
    /// Allows custom logging callbacks for Info/Verbose, Warnings, Errors and Exceptions. Mostly used by <see cref="Debug"/>
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// The <see cref="Log(object)"/> callback.
        /// </summary>
        private LoggerCallback _InfoCallback;
        /// <summary>
        /// The <see cref="LogWarning(object)(object)"/> callback.
        /// </summary>
        private LoggerCallback _WarningCallback;
        /// <summary>
        /// The <see cref="LogError(object)"/> callback.
        /// </summary>
        private LoggerCallback _ErrorCallback;
        /// <summary>
        /// The <see cref="LogException(object)"/> callback.
        /// </summary>
        private LoggerCallback _ExceptionCallback;

        /// <summary>
        /// Is <see cref="WarningCallback"/> set.
        /// </summary>
        private bool _WarningSet = false;

        /// <summary>
        /// Is <see cref="ErrorCallback"/> set.
        /// </summary>
        private bool _ErrorSet = false;

        /// <summary>
        /// Is <see cref="ExceptionCallback"/> set.
        /// </summary>
        private bool _ExceptionSet = false;


        /// <summary>
        /// Allows custom logging callbacks for Info/Verbose, Warnings, Errors and Exceptions. Mostly used by <see cref="Debug"/>.
        /// </summary>
        /// <param name="informationCallback">The Info/Verbose callback, must be set.</param>
        /// <param name="warningCallback">The Warning callback, can be null.</param>
        /// <param name="errorCallback">The Error callback, can be null.</param>
        /// <param name="exceptionCallback">The Exception callback, can be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Logger(
            LoggerCallback informationCallback, 
            LoggerCallback warningCallback = null, 
            LoggerCallback errorCallback = null, 
            LoggerCallback exceptionCallback = null)
        {
            this._InfoCallback = informationCallback ?? throw new ArgumentNullException(nameof(informationCallback), "At least the information callback must be set.");

            if (warningCallback != null)
            {
                this._WarningCallback = warningCallback;
                this._WarningSet = true;
            }
            if (errorCallback != null)
            {
                this._ErrorCallback = errorCallback;
                this._ErrorSet = true;
            }
            if (exceptionCallback != null)
            {
                this._ExceptionCallback = exceptionCallback;
                this._ExceptionSet = true;
            }
        }

        /// <summary>
        /// Set new <see cref="LoggerCallback"/>s for this logger.
        /// </summary>
        /// <param name="types">The logger types to change/add.</param>
        /// <param name="callback">The callback used to log those types.</param>
        public void SetCallbacks(LogCallbackTypes types, LoggerCallback callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback), "The callback must not be null.");
            
            if (types.HasFlag(LogCallbackTypes.Info))
            {
                this._InfoCallback = callback;
            }
            
            if (types.HasFlag(LogCallbackTypes.Warning))
            {
                this._WarningCallback = callback;
                _WarningSet = true;
            }
            
            if (types.HasFlag(LogCallbackTypes.Error))
            {
                this._ErrorCallback = callback;
                _ErrorSet = true;
            }
            
            if (types.HasFlag(LogCallbackTypes.Exception))
            {
                this._ExceptionCallback = callback;
                _ExceptionSet = true;
            }
        }
        /// <summary>
        /// Remove a one or multiple callbacks from this logger.
        /// </summary>
        /// <param name="callbacks">The set or callbacks to remove.</param>
        public void UnsetCallbacks(LogCallbackTypes callbacks)
        {
            if (callbacks.HasFlag(LogCallbackTypes.Warning))
            {
                this._WarningCallback = null;
                _WarningSet = false;
            }
            
            if (callbacks.HasFlag(LogCallbackTypes.Error))
            {
                this._ErrorCallback = null;
                _ErrorSet = false;
            }
            
            if (callbacks.HasFlag(LogCallbackTypes.Exception))
            {
                this._ExceptionCallback = null;
                _ExceptionSet = false;
            }
        }
        
        /// <summary>
        /// Log an info/verbose object into the logger.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public void Log(object message)
        {
            _InfoCallback.Invoke(message);
        }
        /// <summary>
        /// Logs a warning object into the logger. If there is no Warning callback, it will be logged as Info.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public void LogWarning(object message)
        {
            if (_WarningSet)
            {
                _WarningCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
        /// <summary>
        /// Logs an error object into the logger. If there is no Error callback, it will be logged as Info.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public void LogError(object message)
        {
            if (_ErrorSet)
            {
                _ErrorCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
        /// <summary>
        /// Logs an exeption object into the logger. If there is no Exception callback, it will be logged as Info.
        /// </summary>
        /// <param name="message">The object to log.</param>
        public void LogException(object message)
        {
            if (_ExceptionSet)
            {
                _ExceptionCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
    }
}

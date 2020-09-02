using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class Logger
    {
        public delegate void LoggerCallback(object message);

        private LoggerCallback LogInfoCallback;
        private LoggerCallback LogWarningCallback;
        private bool WarningCallbackSet = false;
        private LoggerCallback LogErrorCallback;
        private bool ErrorCallbackSet = false;
        private LoggerCallback LogExceptionCallback;
        private bool ExceptionCallbackSet = false;

        public Logger(
            LoggerCallback informationCallback, 
            LoggerCallback warningCallback = null, 
            LoggerCallback errorCallback = null, 
            LoggerCallback exceptionCallback = null)
        {
            this.LogInfoCallback = informationCallback ?? throw new ArgumentNullException(nameof(informationCallback), "At least the information callback must be set.");

            if (warningCallback != null)
            {
                this.LogWarningCallback = warningCallback;
                WarningCallbackSet = true;
            }
            if (errorCallback != null)
            {
                this.LogErrorCallback = errorCallback;
                ErrorCallbackSet = true;
            }
            if (exceptionCallback != null)
            {
                this.LogExceptionCallback = exceptionCallback;
                ExceptionCallbackSet = true;
            }
        }

        public void SetCallbacks(LogCallbackTypes types, LoggerCallback callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback), "The callback must be set.");
            
            if (types.HasFlag(LogCallbackTypes.Info))
            {
                this.LogInfoCallback = callback;
            }
            
            if (types.HasFlag(LogCallbackTypes.Warning))
            {
                this.LogWarningCallback = callback;
                WarningCallbackSet = true;
            }
            
            if (types.HasFlag(LogCallbackTypes.Error))
            {
                this.LogErrorCallback = callback;
                ErrorCallbackSet = true;
            }
            
            if (types.HasFlag(LogCallbackTypes.Exception))
            {
                this.LogExceptionCallback = callback;
                ExceptionCallbackSet = true;
            }
        }

        public void UnsetCallbacks(LogCallbackTypes callbacks)
        {
            if (callbacks.HasFlag(LogCallbackTypes.Warning))
            {
                this.LogWarningCallback = null;
                WarningCallbackSet = false;
            }
            
            if (callbacks.HasFlag(LogCallbackTypes.Error))
            {
                this.LogErrorCallback = null;
                ErrorCallbackSet = false;
            }
            
            if (callbacks.HasFlag(LogCallbackTypes.Exception))
            {
                this.LogExceptionCallback = null;
                ExceptionCallbackSet = false;
            }
        }
        
        public void Log(object message)
        {
            LogInfoCallback.Invoke(message);
        }
        public void LogWarning(object message)
        {
            if (WarningCallbackSet)
            {
                LogWarningCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
        public void LogError(object message)
        {
            if (ErrorCallbackSet)
            {
                LogErrorCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
        public void LogException(object message)
        {
            if (ErrorCallbackSet)
            {
                LogExceptionCallback.Invoke(message);
            }

            else
            {
                Log(message);
            }
        }
    }
}

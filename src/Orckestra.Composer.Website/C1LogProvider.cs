using Orckestra.Composer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orckestra.Composer.Website
{
    public class C1LogProvider : ILogProvider
    {
        public static ILogProvider Instance { get; private set; } = new C1LogProvider();

        private C1LogProvider()
        {
        }

        public Logger GetLogger(string name)
        {
            return LogInternal;
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        private bool LogInternal(LogLevel logLevel, Func<string> messageFunc, Exception exception, object[] formatParameters)
        {            
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    HandleLoggerException(messageFunc, exception, formatParameters, Composite.Core.Log.LogCritical, Composite.Core.Log.LogCritical);
                    break;
                case LogLevel.Error:
                    HandleLoggerException(messageFunc, exception, formatParameters, Composite.Core.Log.LogError, Composite.Core.Log.LogError);
                    break;
                case LogLevel.Warn:
                    HandleLoggerException(messageFunc, exception, formatParameters, Composite.Core.Log.LogWarning, Composite.Core.Log.LogWarning);
                    break;
                case LogLevel.Info:
                    HandleLoggerException(messageFunc, exception, formatParameters, Composite.Core.Log.LogInformation, Composite.Core.Log.LogInformation);
                    break;
                default:
                    HandleLoggerException(messageFunc, exception, formatParameters, Composite.Core.Log.LogVerbose, Composite.Core.Log.LogVerbose);
                    break;
            }
            
            return true;
        }

        private void HandleLoggerException(
            Func<string> messageFunc, 
            Exception exception, 
            object[] formatParameters,
            Action<string, string> logger,
            Action<string, string, object[]> loggerWithParameters)        
        {
            if (messageFunc != null)
            {
                var message = messageFunc();
                if (exception != null)
                {                    
                    message = messageFunc() + Environment.NewLine + exception.ToString();
                }

                if (formatParameters != null && formatParameters.Any())
                {                    
                    loggerWithParameters("Composer", message, formatParameters);
                }
                else
                {                    
                    logger("Composer", message);
                }
            }           
        }
    }    
}
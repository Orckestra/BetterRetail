using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Hangfire
{
    public class HangfireLogger : ILog
    {
        private static Orckestra.Composer.Logging.ILog Logger = Orckestra.Composer.Logging.LogProvider.GetCurrentClassLogger();        

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            return Logger.Log(MapLogLevel(logLevel), messageFunc, exception);            
        }

        private static Orckestra.Composer.Logging.LogLevel MapLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return Logging.LogLevel.Debug;
                case LogLevel.Error:
                    return Logging.LogLevel.Error;
                case LogLevel.Fatal:
                    return Logging.LogLevel.Fatal;
                case LogLevel.Info:
                    return Logging.LogLevel.Info;
                case LogLevel.Trace:
                    return Logging.LogLevel.Trace;
                case LogLevel.Warn:
                    return Logging.LogLevel.Warn;
                default:
                    throw new ArgumentException($"LogLevel {logLevel.ToString()} not supported.", nameof(logLevel));
            }
        }
    }
}

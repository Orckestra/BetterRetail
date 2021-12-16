using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Orckestra.Composer.Website.App_Insights.AppInsightsListener
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    internal sealed class AppInsightsListener : CustomTraceListener
    {
        public override void WriteLine(string message) => Write(message);

        public override void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || !IsC1FunctionException(message, out string functionName)) return;

            var telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);

            if (!telemetryClient.IsEnabled()) return;

            DependencyTelemetry dependencyTelemetry = new DependencyTelemetry()
            {
                Type = TelemetryInitializer.C1Function,
                Success = false
            };

            if (!string.IsNullOrWhiteSpace(functionName))
            {
                dependencyTelemetry.Properties.Add(TelemetryInitializer.C1Function, functionName);
            }

            using (var operation = telemetryClient.StartOperation(dependencyTelemetry))
            {
                var title = GetPropertyValue(message, "Title");
                var severityLevel = GetSeverityLevel(message);
                var exceptionTypeName = GetExceptionTypeName(message) ?? nameof(Exception);
                var callStackContent = GetCallStackContent(message);

                ExceptionDetailsInfo exceptionDetailsInfo = new ExceptionDetailsInfo(
                    0,
                    0,
                    exceptionTypeName,
                    $"{severityLevel}: {title}",
                    !string.IsNullOrWhiteSpace(callStackContent),
                    callStackContent,
                    new List<StackFrame>());

                var telemetryExceptionProperties = new Dictionary<string, string>();

                var propertiesToProcess = new List<string>()
                {
                    "Category", 
                    "Priority", 
                    "EventId", 
                    "Title", 
                    "Machine", 
                    "App Domain", 
                    "ProcessId", 
                    "Process Name", 
                    "Thread Name", 
                    "Win32 ThreadId"
                };


                foreach (var el in propertiesToProcess)
                {
                    AddTelemetryExceptionProperty(message, el, telemetryExceptionProperties);
                }

                var exception = new ExceptionTelemetry(
                    new List<ExceptionDetailsInfo>() { exceptionDetailsInfo }, 
                    severityLevel, 
                    null,
                    telemetryExceptionProperties, 
                    new Dictionary<string, double>());

                telemetryClient.TrackException(exception);
            }
        }

        private bool IsC1FunctionException(string message, out string functionName)
        {
            functionName = null;
            Regex regex = new Regex("^Title:.+? Function: (.+?)$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = regex.Match(message);
            if (!match.Success) return false;

            functionName = match.Groups[1].Value.Trim();
            return true;
        }

        private SeverityLevel GetSeverityLevel(string message)
        {
            SeverityLevel severityLevel = SeverityLevel.Error;
            Regex regex = new Regex($"Severity: (.+?)$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = regex.Match(message);
            if (!match.Success) return severityLevel;

            switch (match.Groups[1].Value.Trim())
            {
                case "Fine":
                case "Info":
                    severityLevel = SeverityLevel.Information;
                    break;
                case "Fatal":
                    severityLevel = SeverityLevel.Critical;
                    break;
                case "Warning":
                    severityLevel = SeverityLevel.Warning;
                    break;
                case "Debug":
                    severityLevel = SeverityLevel.Verbose;
                    break;
                case "Error":
                    severityLevel = SeverityLevel.Error;
                    break;
            }
            return severityLevel;
        }

        private string GetExceptionTypeName(string message)
        {
            Regex regex = new Regex("^Message: (.+?):", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = regex.Match(message);
            return !match.Success ? null : match.Groups[1].Value.Trim();
        }

        private string GetCallStackContent(string message)
        {
            var keyWord = "Extended Properties:";
            var stackIndex = message.IndexOf(keyWord);
            return stackIndex < 0 ? null : message.Remove(0, stackIndex + keyWord.Length).Trim();
        }

        private string GetPropertyValue(string message, string propertyName)
        {
            Regex regex = new Regex($"^{propertyName}:(.+?)$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = regex.Match(message);
            return !match.Success ? null : match.Groups[1].Value.Trim();
        }

        private void AddTelemetryExceptionProperty(string message, string propertyName, Dictionary<string, string> exceptionProperties)
        {
            var value = GetPropertyValue(message, propertyName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                exceptionProperties.Add(propertyName, value);
            }
        }
    }
}
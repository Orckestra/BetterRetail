using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Text;

namespace Orckestra.Composer.Website.App_Insights.AppInsightsListener
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    internal sealed class AppInsightsListener : CustomTraceListener
    {
        internal const string C1Function = nameof(C1Function);

        private static readonly RegexOptions _regexOptions 
            = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline;

        private static readonly HashSet<string> _callStackLinesToIgnore = new HashSet<string>()
        {
            "--- End of stack trace from previous location where exception was thrown ---",
            "at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()",
            "at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)",
            "at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()",
            "at System.Threading.Tasks.Task.Execute()"
        };

        private static readonly Dictionary<C1MessageProps, Regex> _rd = new Dictionary<C1MessageProps, Regex>()
        {
            { C1MessageProps.FunctionName, new Regex("^Title:.+? Function: (.+?)$", _regexOptions)},
            { C1MessageProps.C1Severity, new Regex($"Severity: (.+?)$", _regexOptions)},
            { C1MessageProps.ExceptionType, new Regex("^Message: (.+?):", _regexOptions)},
            { C1MessageProps.Priority, new Regex($"^Priority:(.+?)$", _regexOptions)},
            { C1MessageProps.Machine, new Regex($"^Machine:(.+?)$", _regexOptions)},
            { C1MessageProps.AppDomain, new Regex($"^App Domain:(.+?)$", _regexOptions)},
            { C1MessageProps.ProcessId, new Regex($"^ProcessId:(.+?)$", _regexOptions)}
        };

        private enum C1MessageProps
        {
            FunctionName,
            C1Severity,
            ExceptionType,
            Priority,
            Machine,
            AppDomain,
            ProcessId
        }

        public override void WriteLine(string message) => Write(message);

        public override void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || !IsC1FunctionMessage(message)) return;

            var telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);
            if (!telemetryClient.IsEnabled()) return;

            var c1Severity = GetPropertyValue(message, _rd[C1MessageProps.C1Severity]);
            var aiSeveriry = GetAISeverity(c1Severity);

            var callStackContent = GetCallStackContent(message);

            var exceptionType = GetPropertyValue(message, _rd[C1MessageProps.ExceptionType]);
            var functionName = GetPropertyValue(message, _rd[C1MessageProps.FunctionName]);
            var priority = GetPropertyValue(message, _rd[C1MessageProps.Priority]);
            var machine = GetPropertyValue(message, _rd[C1MessageProps.Machine]);
            var appDomain = GetPropertyValue(message, _rd[C1MessageProps.AppDomain]);
            var processId = GetPropertyValue(message, _rd[C1MessageProps.ProcessId]);

            DependencyTelemetry dependencyTelemetry = new DependencyTelemetry()
            {
                Type = "C1 Function",
                Success = aiSeveriry == SeverityLevel.Information
            };

            dependencyTelemetry.Properties.Add(C1Function, functionName);

            var telemetryExceptionProperties = new Dictionary<string, string>()
            {
                {"Function Name", functionName },
                {"Priority",  priority},
                {"Machine", machine },
                {"App Domain", appDomain },
                {"Process ID", processId }
            };

            using (var operation = telemetryClient.StartOperation(dependencyTelemetry))
            {
                ExceptionDetailsInfo exceptionDetailsInfo = new ExceptionDetailsInfo(
                    0,
                    0,
                    exceptionType,
                    $"{aiSeveriry}: {functionName}",
                    !string.IsNullOrWhiteSpace(callStackContent),
                    callStackContent,
                    new List<StackFrame>());

                var exception = new ExceptionTelemetry(
                    new List<ExceptionDetailsInfo>() { exceptionDetailsInfo },
                    aiSeveriry, 
                    null,
                    telemetryExceptionProperties, 
                    new Dictionary<string, double>());

                telemetryClient.TrackException(exception);
            }
        }

        public bool IsC1FunctionMessage(string message) => _rd[C1MessageProps.FunctionName].IsMatch(message);

        private string GetPropertyValue(string message, Regex regex)
        {
            var match = regex.Match(message);
            return !match.Success ? null : match.Groups[1].Value.Trim();
        }

        private SeverityLevel GetAISeverity(string c1Severity)
        {
            SeverityLevel severityLevel;
            switch (c1Severity)
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
                default:
                    severityLevel = SeverityLevel.Error;
                    break;
            }
            return severityLevel;
        }

        private string GetCallStackContent(string message)
        {
            var keyWord = "Extended Properties:";
            var stackIndex = message.IndexOf(keyWord);
            if (stackIndex < 0) return null;

            message = message.Remove(0, stackIndex + keyWord.Length).Trim();

            StringBuilder sb = new StringBuilder();
            foreach(string line in message.Split('\n'))
            {
                var lineTrimed = line.Trim();
                if (_callStackLinesToIgnore.Contains(lineTrimed)) continue;
                sb.AppendLine(lineTrimed);
            }
            return sb.ToString();
        }
    }
}
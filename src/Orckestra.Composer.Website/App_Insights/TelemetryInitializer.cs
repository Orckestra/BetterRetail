using System;
using System.Configuration;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Orckestra.Composer.Website.App_Insights
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        public const string C1Function = nameof(C1Function);

        public void Initialize(ITelemetry telemetry)
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

            if (!(telemetry is OperationTelemetry operationTelemetry)) return;
            var variation = (ConfigurationManager.AppSettings["Variation"] ?? "").ToUpperInvariant();
            var operationPrefix = $"WFE{variation}";

            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = operationPrefix;
            }

            var operationName = telemetry.Context.Operation.Name;
            if (operationName.StartsWith(operationPrefix)) return;

            var context = System.Web.HttpContext.Current;
            var requestTypeKey = ComposerOvertureClient.RefAppRequestTypeKey;

            if (operationTelemetry.Properties.ContainsKey(C1Function))
            {
                operationName = operationTelemetry.Properties[C1Function];
            }
            else if (context != null && context.Items.Contains(requestTypeKey))
            {
                var requestType = (Type)context.Items[requestTypeKey];
                operationName = $"{operationPrefix} {requestType.Name}";
            }

            operationTelemetry.Name = operationName;
            operationTelemetry.Context.Operation.Name = operationName;
        }
    }
}
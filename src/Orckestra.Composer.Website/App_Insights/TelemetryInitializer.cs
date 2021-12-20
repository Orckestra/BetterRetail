using System.Configuration;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using static Orckestra.Composer.ActionFilters.AppInsightsActionFilter;

namespace Orckestra.Composer.Website.App_Insights
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        static TelemetryInitializer()
        {
            string aiKeyFromAppSettings = ConfigurationManager.AppSettings["InstrumentationKey"];
            if (!string.IsNullOrWhiteSpace(aiKeyFromAppSettings))
            {
                TelemetryConfiguration.Active.InstrumentationKey = aiKeyFromAppSettings;
            }
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is OperationTelemetry operationTelemetry)) return;
            var variation = (ConfigurationManager.AppSettings["Variation"] ?? "").ToUpperInvariant();
            var operationPrefix = $"WFE{variation}";

            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = operationPrefix;
            }

            var operationName = telemetry.Context.Operation.Name;
            if (operationName.StartsWith(operationPrefix)) return;

            if (operationTelemetry.Properties.ContainsKey(AppInsightsListener.AppInsightsListener.C1Function))
            {
                operationName = operationTelemetry.Properties[AppInsightsListener.AppInsightsListener.C1Function];
            }
            else
            {
                var context = System.Web.HttpContext.Current;
                var controllerName = context.Items[AIAFControllerKey]?.ToString();
                var actionName = context.Items[AIAFActionKey]?.ToString();

                operationName = !string.IsNullOrWhiteSpace(controllerName) && !string.IsNullOrWhiteSpace(actionName)
                    ? $"{operationPrefix} {controllerName}.{actionName}"
                    : $"{operationPrefix} {operationName}";
            }

            operationTelemetry.Name = operationName;
            operationTelemetry.Context.Operation.Name = operationName;
        }
    }
}
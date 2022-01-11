using System.Configuration;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using static Orckestra.Composer.ActionFilters.AppInsightsActionFilter;

namespace Orckestra.Composer.Website.App_Insights
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        private static readonly string _operationPrefix 
            = $"WFE{(ConfigurationManager.AppSettings["Variation"] ?? "").ToUpperInvariant()}";

        static TelemetryInitializer()
        {
            string aiKeyFromAppSettings = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            if (!string.IsNullOrWhiteSpace(aiKeyFromAppSettings))
            {
                TelemetryConfiguration.Active.InstrumentationKey = aiKeyFromAppSettings;
            }
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is OperationTelemetry operationTelemetry)) return;

            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = _operationPrefix;
            }

            var operationName = telemetry.Context.Operation.Name;
            if (operationName != null && operationName.StartsWith(_operationPrefix)) return;
            if (operationTelemetry.Properties.ContainsKey(AppInsightsListener.C1Function))
            {
                operationName = operationTelemetry.Properties[AppInsightsListener.C1Function];
            }
            else
            {
                var context = System.Web.HttpContext.Current;
                var controllerName = context.Items[AIAFControllerKey]?.ToString();
                var actionName = context.Items[AIAFActionKey]?.ToString();

                operationName = !string.IsNullOrWhiteSpace(controllerName) && !string.IsNullOrWhiteSpace(actionName)
                    ? $"{_operationPrefix} {controllerName}.{actionName}"
                    : $"{_operationPrefix} {operationName}";
            }

            operationTelemetry.Name = operationName;
            operationTelemetry.Context.Operation.Name = operationName;
        }
    }
}
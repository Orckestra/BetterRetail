using Autofac.Integration.WebApi;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Orckestra.Composer.ActionFilters
{
    public class AppInsightsActionFilter : IAutofacActionFilter
    {
        public static string AIAFControllerKey = nameof(AIAFControllerKey);
        public static string AIAFActionKey = nameof(AIAFActionKey);

        public void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) { }

        public void OnActionExecuting(HttpActionContext actionContext)
        {
            var actionDescriptor = actionContext?.ActionDescriptor;
            if (actionDescriptor == null) return;

            var controllerName = actionDescriptor.ControllerDescriptor?.ControllerName;
            var actionName = actionDescriptor.ActionName;

            if (string.IsNullOrWhiteSpace(controllerName) || string.IsNullOrWhiteSpace(actionName)) return;

            var context = System.Web.HttpContext.Current;
            if (context == null) return;

            if (!context.Items.Contains(AIAFControllerKey))
            {
                context.Items[AIAFControllerKey] = controllerName;
            }

            if (!context.Items.Contains(AIAFActionKey))
            {
                context.Items[AIAFActionKey] = actionName;
            }
        }
    }
}
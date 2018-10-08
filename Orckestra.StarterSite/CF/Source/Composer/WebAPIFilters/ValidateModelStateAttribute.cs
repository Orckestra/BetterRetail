using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Orckestra.Composer.WebAPIFilters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                throw new HttpResponseException(BuildErrorMessage(actionContext.Request, modelState));
            }

            base.OnActionExecuting(actionContext);
        }

        private HttpResponseMessage BuildErrorMessage(HttpRequestMessage request, ModelStateDictionary modelState)
        {
            var error = new HttpError(modelState, true);
            var msg = request.CreateResponse(HttpStatusCode.BadRequest, error);
            return msg;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Orckestra.Composer.WebAPIFilters
{
    /// <summary>
    /// Ensures that all AJAX requests are from Composer.
    /// </summary>
    public sealed class JQueryOnlyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IsValidRequestHeader(actionContext.Request.Headers))
            {
                var error = new HttpError("Bad request.");
                var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
                throw new HttpResponseException(response);
            }

            base.OnActionExecuting(actionContext);
        }

        private bool IsValidRequestHeader(HttpRequestHeaders headers)
        {
            return headers.TryGetValues("X-Requested-With", out IEnumerable<string> headerTokens) ? headerTokens.Any() : false;
        }
    }
}

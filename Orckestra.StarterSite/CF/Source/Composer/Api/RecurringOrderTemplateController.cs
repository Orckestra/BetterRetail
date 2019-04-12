using Orckestra.Composer.Parameters;
using Orckestra.Composer.Requests;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class RecurringOrderTemplateController : ApiController
    {
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public RecurringOrderTemplateController(
            IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService,
            IComposerContext composerContext)
        {
            if (recurringOrderTemplatesViewService == null) { throw new ArgumentNullException(nameof(recurringOrderTemplatesViewService)); }
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }

            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService;
            ComposerContext = composerContext;
        }

        [HttpGet]
        [Route("getrecurringordertemplates")]
        public virtual async Task<IHttpActionResult> GetRecurringOrderTemplates()
        {
            var vm = await RecurringOrderTemplatesViewService.GetRecurringOrderTemplatesAsync(new GetRecurringOrderTemplatesParam {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl =  RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(vm);
        }

        //[HttpPut]
        //[Route("lineitemquantity")]
        //public virtual async Task<IHttpActionResult> UpdateRecurringOrderTemplateLineItemQuantity(UpdateRecurringOrderLineItemQuantityRequest request)
        //{
        //    if (request == null) { return BadRequest("Missing Request Body"); }
        //    if (!ModelState.IsValid) { return BadRequest(ModelState); }

        //    var param = new UpdateRecurringOrderTemplateLineItemQuantityParam
        //    {
        //        RecurringLineItemId = request.RecurringLineItemId,
        //        Quantity = request.Quantity,
        //        ScopeId = ComposerContext.Scope,
        //        CustomerId = ComposerContext.CustomerId,
        //        CultureInfo = ComposerContext.CultureInfo,
        //        BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
        //    };

        //    var results = await RecurringOrderTemplatesViewService.UpdateRecurringOrderTemplateLineItemQuantity(param).ConfigureAwait(false);

        //    return Ok(results);
        //}
    }
}

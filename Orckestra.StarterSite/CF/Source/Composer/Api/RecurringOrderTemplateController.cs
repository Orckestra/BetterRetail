using Orckestra.Composer.Parameters;
using Orckestra.Composer.Request;
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

        [HttpPut]
        [Route("lineitemquantity")]
        public virtual async Task<IHttpActionResult> UpdateRecurringOrderTemplateLineItemQuantity(UpdateRecurringOrderLineItemQuantityRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var param = new UpdateRecurringOrderTemplateLineItemQuantityParam
            {
                RecurringLineItemId = request.RecurringLineItemId,
                Quantity = request.Quantity,
                ScopeId = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            };

            var results = await RecurringOrderTemplatesViewService.UpdateRecurringOrderTemplateLineItemQuantityAsync(param).ConfigureAwait(false);

            return Ok(results);
        }


        [HttpDelete]
        [Route("lineitem/{lineItemId}")]
        public virtual async Task<IHttpActionResult> RemoveRecurringLineItem([FromUri]string lineItemId)
        {
            if (string.IsNullOrWhiteSpace(lineItemId)) { return BadRequest("Invalid lineItemId"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var results = await RecurringOrderTemplatesViewService.RemoveRecurringOrderTemplateLineItemAsync(new RemoveRecurringOrderTemplateLineItemParam()
            {
                Culture = ComposerContext.CultureInfo,
                ScopeId = ComposerContext.Scope,
                LineItemId = lineItemId,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpDelete]
        [Route("lineitems/byIds")]
        public virtual async Task<IHttpActionResult> RemoveRecurringLineItems(RemoveRecurringOrderTemplateLineItemsRequest request)
        {
            if (request.LineItemsIds.Count == 0) { return BadRequest("Invalid lineItemsIds"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var results = await RecurringOrderTemplatesViewService.RemoveRecurringOrderTemplatesLineItemsAsync(new RemoveRecurringOrderTemplateLineItemsParam()
            {
                Culture = ComposerContext.CultureInfo,
                ScopeId = ComposerContext.Scope,
                LineItemsIds = request.LineItemsIds,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(results);
        }

        [HttpPut]
        [Route("lineItem")]
        public virtual async Task<IHttpActionResult> UpdateRecurringOrderTemplateLineItem(UpdateRecurringOrderTemplateLineItemRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            var results = await RecurringOrderTemplatesViewService.UpdateRecurringOrderTemplateLineItemAsync(new UpdateRecurringOrderTemplateLineItemParam {
                ScopeId = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),

                BillingAddressId = request.BillingAddressId,
                LineItemId = request.LineItemId,
                NextOccurence = request.NextOccurence,
                PaymentMethodId = request.PaymentMethodId,
                RecurringOrderFrequencyName = request.RecurringOrderFrequencyName,
                ShippingAddressId = request.ShippingAddressId,
                ShippingMethodName = request.ShippingMethodName,
                ShippingProviderId = request.ShippingProviderId
            }).ConfigureAwait(false);

            return Ok(results);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Request;
using Orckestra.Composer.Requests;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class RecurringOrderTemplateController : ApiController
    {
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public RecurringOrderTemplateController(IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService, IComposerContext composerContext)
        {
            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService ?? throw new ArgumentNullException(nameof(recurringOrderTemplatesViewService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        [HttpGet]
        [ActionName("getrecurringordertemplates")]
        public virtual async Task<IHttpActionResult> GetRecurringOrderTemplates()
        {
            var vm = await RecurringOrderTemplatesViewService.GetRecurringOrderTemplatesViewModelAsync(new GetRecurringOrderTemplatesParam 
            {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl =  RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(vm);
        }

        [HttpPut]
        [ActionName("lineitemquantity")]
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
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> RemoveRecurringLineItem([FromBody]RemoveRecurringOrderTemplateLineItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LineItemId)) { return BadRequest("Invalid lineItemId"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var results = await RecurringOrderTemplatesViewService.RemoveRecurringOrderTemplateLineItemAsync(new RemoveRecurringOrderTemplateLineItemParam
            {
                Culture = ComposerContext.CultureInfo,
                ScopeId = ComposerContext.Scope,
                LineItemId = request.LineItemId,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpDelete]
        [ActionName("lineitems/byIds")]
        public virtual async Task<IHttpActionResult> RemoveRecurringLineItems(RemoveRecurringOrderTemplateLineItemsRequest request)
        {
            if (request.LineItemsIds.Count == 0) { return BadRequest("Invalid lineItemsIds"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var results = await RecurringOrderTemplatesViewService.RemoveRecurringOrderTemplatesLineItemsAsync(new RemoveRecurringOrderTemplateLineItemsParam
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
        [ActionName("lineItem")]
        public virtual async Task<IHttpActionResult> UpdateRecurringOrderTemplateLineItem(UpdateRecurringOrderTemplateLineItemRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            var results = await RecurringOrderTemplatesViewService.UpdateRecurringOrderTemplateLineItemAsync(new UpdateRecurringOrderTemplateLineItemParam 
            {
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

        [HttpPost]
        [ActionName("getrecurringordertemplatedetails")]
        public virtual async Task<IHttpActionResult> GetRecurringOrderTemplateDetails(GetRecurringOrderTemplateDetailsRequest request)
        {
            if(request == null) { return BadRequest("Missing Request Body"); }
            if (string.IsNullOrEmpty(request.RecurringOrderTemplateId)) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            Guid.TryParse(request.RecurringOrderTemplateId, out Guid guid);

            var vm = await RecurringOrderTemplatesViewService.GetRecurringOrderTemplateDetailViewModelAsync(new GetRecurringOrderTemplateDetailParam
            {
                RecurringOrderLineItemId = guid,
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(vm);
        }
    }
}
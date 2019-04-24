using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class RecurringCartController : ApiController    
    {
        protected IRecurringOrderCartsViewService RecurringOrderCartsService { get; }
        protected IComposerContext ComposerContext { get; }
        protected IPaymentViewService PaymentViewService { get; }
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesService { get; }

        public RecurringCartController(
            IRecurringOrderCartsViewService recurringOrderCarstService,
            IComposerContext composerContext,
            IPaymentViewService paymentViewService,
            IRecurringOrderTemplatesViewService recurringOrderTemplatesService)
        {
            if (recurringOrderCarstService == null) throw new ArgumentNullException(nameof(recurringOrderCarstService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(recurringOrderCarstService)));
            if (composerContext == null) throw new ArgumentNullException(nameof(composerContext), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(composerContext)));
            if (paymentViewService == null) throw new ArgumentNullException(nameof(paymentViewService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(paymentViewService)));
            if (recurringOrderTemplatesService == null) throw new ArgumentNullException(nameof(recurringOrderTemplatesService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(recurringOrderTemplatesService)));

            RecurringOrderCartsService = recurringOrderCarstService;
            ComposerContext = composerContext;
            PaymentViewService = paymentViewService;
            RecurringOrderTemplatesService = recurringOrderTemplatesService;
        }

        [HttpGet]
        [Route("getrecurringordercarts")]
        public virtual async Task<IHttpActionResult> GeRecurringOrderCartsByUser()
        {
            //This call manages products/variants that have been deleted in templates.
            //When cleaning those templates, it should clean the carts too.
            //In most cases, generating the templates is not a big load, if it's a problem, create a new call in 
            //RecurringOrderTemplateViewModelFactory to only check templates are fine and clean up if not.
            var templatesVm = await RecurringOrderTemplatesService.GetRecurringOrderTemplatesViewModelAsync(new GetRecurringOrderTemplatesParam
            {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            var results = await RecurringOrderCartsService.GetRecurringOrderCartListViewModelAsync(new GetRecurringOrderCartsViewModelParam
            {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpGet]
        [ActionName("upcoming-orders")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetPastOrders()
        {
            var viewModel = await RecurringOrderCartsService.GetLightRecurringOrderCartListViewModelAsync(new GetLightRecurringOrderCartListViewModelParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(viewModel);
        }
    }
}

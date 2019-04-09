using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public class RecurringOrderTemplatesViewService : IRecurringOrderTemplatesViewService
    {
        protected IRecurringOrdersRepository RecurringOrderRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IOvertureClient OvertureClient { get; private set; }
        protected IRecurringOrderTemplateViewModelFactory RecurringOrderTemplateViewModelFactory { get; private set; }
        protected ILineItemService LineItemService { get; private set; }
        protected ILookupService LookupService { get; private set; }
        //protected IRecurringOrderProgramViewModelFactory _recurringOrderProgramViewModelFactory { get; private set; }
        //private readonly IExtendedCartRepository _cartRepository;
        //private readonly IRecurringOrderInactifProductViewModelFactory _recurringOrderInactifProductViewModelFactory;

        public RecurringOrderTemplatesViewService(IRecurringOrdersRepository recurringOrdersRepository, IViewModelMapper viewModelMapper,
            IOvertureClient overtureClient, ILineItemService lineItemService, ILookupService lookupService,
            IRecurringOrderTemplateViewModelFactory recurringOrderTemplateViewModelFactory)
        {
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (viewModelMapper == null) { throw new ArgumentNullException(nameof(viewModelMapper)); }
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (lineItemService == null) { throw new ArgumentNullException(nameof(lineItemService)); }
            if (lookupService == null) { throw new ArgumentNullException(nameof(lookupService)); }
            if (recurringOrderTemplateViewModelFactory == null) { throw new ArgumentNullException(nameof(recurringOrderTemplateViewModelFactory)); }

            RecurringOrderRepository = recurringOrdersRepository;
            ViewModelMapper = viewModelMapper;
            OvertureClient = overtureClient;
            LineItemService = lineItemService;
            LookupService = lookupService;
            RecurringOrderTemplateViewModelFactory = recurringOrderTemplateViewModelFactory;
        }
        public async Task<bool> GetIsPaymentMethodUsedInRecurringOrders(GetIsPaymentMethodUsedInRecurringOrdersRequest request)
        {
            if (RecurringOrderCartHelper.IsRecurringOrdersEnabled())
                return false;

            if (request.ScopeId == null) { throw new ArgumentNullException(nameof(request.ScopeId)); }
            if (request.CustomerId == null) { throw new ArgumentNullException(nameof(request.CustomerId)); }
            if (request.CultureInfo == null) { throw new ArgumentNullException(nameof(request.CultureInfo)); }
            if (request.PaymentMethodId == null) { throw new ArgumentNullException(nameof(request.PaymentMethodId)); }

            var listOfRecurringOrderLineItems = await RecurringOrderRepository.GetRecurringOrderTemplates(request.ScopeId, request.CustomerId).ConfigureAwait(false);

            if (listOfRecurringOrderLineItems != null)
            {
                foreach (var item in listOfRecurringOrderLineItems.RecurringOrderLineItems ?? Enumerable.Empty<RecurringOrderLineItem>())
                {
                    if (item.PaymentMethodId == RecurringOrderCartHelper.ConvertStringToGuid(request.PaymentMethodId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<RecurringOrderTemplatesViewModel> GetRecurringOrderTemplatesAsync(string scope, Guid customerId, CultureInfo culture, string baseUrl)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new RecurringOrderTemplatesViewModel();

            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            if (customerId == null) { throw new ArgumentNullException(nameof(customerId)); }
            if (culture == null) { throw new ArgumentNullException(nameof(culture)); }

            var listOfRecurringOrderLineItems = await RecurringOrderRepository.GetRecurringOrderTemplates(scope, customerId).ConfigureAwait(false);


            var vm = await CreateTemplatesViewModelAsync(new CreateRecurringOrderTemplatesViewModelParam
            {
                ListOfRecurringOrderLineItems = listOfRecurringOrderLineItems,
                CultureInfo = culture,
                BaseUrl = baseUrl,
                CustomerId = customerId,
                ScopeId = scope
            }).ConfigureAwaitWithCulture(false);


            return vm;
        }

        protected virtual async Task<RecurringOrderTemplatesViewModel> CreateTemplatesViewModelAsync(CreateRecurringOrderTemplatesViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.ListOfRecurringOrderLineItems == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.ListOfRecurringOrderLineItems)), nameof(param)); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await LineItemService.GetImageUrlsAsync(param.ListOfRecurringOrderLineItems).ConfigureAwait(false),
            };

            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).ConfigureAwait(false);

            param.PaymentMethodDisplayNames = methodDisplayNames;

            var vm = await RecurringOrderTemplateViewModelFactory.CreateRecurringOrderTemplatesViewModel(param).ConfigureAwaitWithCulture(false);

            return vm;
        }
        /*
        public async Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItemQuantity(UpdateRecurringOrderTemplateLineItemQuantityRequest request)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderTemplatesViewModel();

            if (request == null) throw new ArgumentNullException(nameof(request), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(request)));


            //Update the cart if quantity is the same from the original template value
            //TODO: see to optimize calls to Services
            var originalLineItems = await _recurringOrderRepository.GetRecurringOrderTemplates(request.ScopeId, request.CustomerId).ConfigureAwait(false);
            var originalLineitem = originalLineItems.RecurringOrderLineItems.ToList().SingleOrDefault(r => r.RecurringOrderLineItemId == RecurringOrderCartHelper.ConvertStringToGuid(request.RecurringLineItemId));
            if (originalLineitem != null)
            {

                var updateCartIfDifferentRequest = new UpdateRecurringOrderCartLineItemQuantityIfDifferentParam()
                {
                    BaseUrl = request.BaseUrl,
                    CustomerId = request.CustomerId,
                    CultureInfo = request.CultureInfo,
                    ScopeId = request.ScopeId,
                    TemplateNewQuantity = request.Quantity,
                    TemplateOriginalQuantity = originalLineitem.Quantity,
                    TemplateProductId = originalLineitem.ProductId,
                    TemplateVariantId = originalLineitem.VariantId

                };
                var result = await _cartRepository.UpdateRecurringOrderCartLineItemQuantityIfDifferent(updateCartIfDifferentRequest).ConfigureAwaitWithCulture(false);
            }

            var listOfRecurringOrderLineItems = await _recurringOrderRepository.UpdateRecurringOrderTemplateLineItemQuantity(request).ConfigureAwaitWithCulture(false);

            return await CreateTemplatesViewModelAsync(new CreateRecurringOrderTemplatesViewModelParam
            {
                ListOfRecurringOrderLineItems = listOfRecurringOrderLineItems,
                CultureInfo = request.CultureInfo,
                BaseUrl = request.BaseUrl,
                ScopeId = request.ScopeId,
                CustomerId = request.CustomerId,
            }).ConfigureAwaitWithCulture(false);
        }

        public async Task<RecurringOrderProgramViewModel> GetRecurringOrderProgramAsync(GetRecurringOrderFrequenciesRequest request)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderProgramViewModel();

            if (request == null) throw new ArgumentNullException(nameof(request), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(request)));

            var program = await _recurringOrderRepository.GetRecurringOrderProgram(request.RecurringOrderProgramName).ConfigureAwaitWithCulture(false);

            if (program != null)
            {

                return _recurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, request.CultureInfo);

            }
            return new RecurringOrderProgramViewModel();
        }

        public async Task<RecurringOrderProgramsViewModel> GetRecurringOrderProgramsByUserAsync(string scope, Guid customerId, CultureInfo culture)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderProgramsViewModel();

            var listOfRecurringOrderLineItems = await _recurringOrderRepository.GetRecurringOrderTemplates(scope, customerId).ConfigureAwait(false);

            var distinctProgramNames = listOfRecurringOrderLineItems.RecurringOrderLineItems.Select(p => p.RecurringOrderProgramName).Distinct(StringComparer.InvariantCultureIgnoreCase);

            var tasks = distinctProgramNames.Select(pName => _recurringOrderRepository.GetRecurringOrderProgram(pName));
            var programs = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new RecurringOrderProgramsViewModel
            {
                Programs = programs.Select(p => _recurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(p, culture)).ToList()
            };
        }

        public async Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItem(UpdateRecurringOrderTemplateLineItemRequest request)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderTemplatesViewModel();

            if (request == null) throw new ArgumentNullException(nameof(request), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(request)));

            var listOfRecurringOrderLineItems = await _recurringOrderRepository.UpdateRecurringOrderTemplateLineItem(request).ConfigureAwaitWithCulture(false);

            return await CreateTemplatesViewModelAsync(new CreateRecurringOrderTemplatesViewModelParam
            {
                ListOfRecurringOrderLineItems = listOfRecurringOrderLineItems,
                CultureInfo = request.CultureInfo,
                BaseUrl = request.BaseUrl,
                ScopeId = request.ScopeId,
                CustomerId = request.CustomerId,
            }).ConfigureAwaitWithCulture(false);
        }

        public async Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplateLineItem(RemoveRecurringOrderTemplateLineItemRequest request)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderTemplatesViewModel();

            if (request == null) throw new ArgumentNullException(nameof(request), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(request)));

            var response = await _recurringOrderRepository.RemoveRecurringOrderTemplateLineItem(request).ConfigureAwaitWithCulture(false);

            return await GetRecurringOrderTemplatesAsync(request.ScopeId, request.CustomerId, request.Culture, request.BaseUrl).ConfigureAwaitWithCulture(false);
        }

        public async Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplatesLineItems(RemoveRecurringOrderTemplateLineItemsRequest request)
        {
            if (!ExtendedComposerConfiguration.RecurringOrders.Enabled)
                return new RecurringOrderTemplatesViewModel();

            if (request == null) throw new ArgumentNullException(nameof(request), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(request)));

            var response = await _recurringOrderRepository.RemoveRecurringOrderTemplateLineItems(request).ConfigureAwaitWithCulture(false);

            return await GetRecurringOrderTemplatesAsync(request.ScopeId, request.CustomerId, request.Culture, request.BaseUrl).ConfigureAwaitWithCulture(false);
        }

       
        public async Task<RecurringOrderShippingMethodsViewModel> GetRecurringOrderShippingMethods(string scopeId, CultureInfo culture)
        {
            var fulfillmentMethods = await _recurringOrderRepository.GetFulfillmentMethods(scopeId).ConfigureAwaitWithCulture(false);

            if (fulfillmentMethods != null && fulfillmentMethods.FulfillmentMethods != null)
            {
                var shippingMethodViewModels = fulfillmentMethods.FulfillmentMethods
                   .Select(sm => _recurringOrderTemplateViewModelFactory.GetShippingMethodViewModel(sm, culture)).ToList();

                foreach (var sm in shippingMethodViewModels)
                {
                    sm.IsShipToStore = sm.ShippingProviderId == ExtendedComposerConfiguration.RecurringOrders.MondouFreeShipToStoreId.ToString();
                }

                return new RecurringOrderShippingMethodsViewModel
                {
                    ShippingMethods = shippingMethodViewModels
                };
            }

            return new RecurringOrderShippingMethodsViewModel();
        }

        public async Task<bool> ClearCustomerInactifItems(ClearCustomerInactifItemsRequest request)
        {
            await CustomerHelper.ClearAllInactifProduct(request.CustomerId, request.ScopeId, _overtureClient).ConfigureAwaitWithCulture(false);
            return await Task.FromResult(true).ConfigureAwaitWithCulture(false);
        }

        public async Task<RecurringOrderInactifProductsViewModel> GetInactifProducts(Guid customerId, string scope, CultureInfo cultureInfo)
        {
            var customerRequest = new GetCustomerRequest
            {
                CustomerId = customerId,
                ScopeId = scope
            };

            var customer = await _overtureClient.SendAsync(customerRequest).ConfigureAwaitWithCulture(false);

            var listInactif = CustomerHelper.GetCustomerInactifList(customer);

            if (listInactif.Count == 0)
                return new RecurringOrderInactifProductsViewModel();
            else
            {
                var vm = await _recurringOrderInactifProductViewModelFactory.CreateRecurringOrderInactifProductsViewModel(new CreateRecurringOrderInactifProductsViewModelParam
                {
                    ListInactifLineItem = listInactif,
                    CultureInfo = cultureInfo,
                    ScopeId = scope,
                    CustomerId = customerId,

                }).ConfigureAwaitWithCulture(false);

                return vm;
            }
        }
        */

    }
}

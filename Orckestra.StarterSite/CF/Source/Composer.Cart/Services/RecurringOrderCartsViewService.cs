using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Helper;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.RecurringOrders;

namespace Orckestra.Composer.Cart.Services
{
    public class RecurringOrderCartsViewService : IRecurringOrderCartsViewService
    {
        protected ICartRepository CartRepository { get; private set; }
        protected IOvertureClient OvertureClient { get; private set; }
        protected IRecurringOrderCartViewModelFactory RecurringOrderCartViewModelFactory { get; private set; }
        protected IImageService ImageService { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IRecurringOrdersRepository RecurringOrdersRepository { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }

        public RecurringOrderCartsViewService(
            ICartRepository cartRepository,
            IOvertureClient overtureClient,
            IRecurringOrderCartViewModelFactory recurringOrderCartViewModelFactory,
            IImageService imageService,
            ILookupService lookupService,
            IRecurringOrdersRepository recurringOrdersRepository,
            IComposerContext composerContext,
            IAddressRepository addressRepository)
        {
            if (cartRepository == null) { throw new ArgumentNullException(nameof(cartRepository)); }
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (recurringOrderCartViewModelFactory == null) { throw new ArgumentNullException(nameof(recurringOrderCartViewModelFactory)); }
            if (imageService == null) { throw new ArgumentNullException(nameof(imageService)); }
            if (lookupService == null) { throw new ArgumentNullException(nameof(lookupService)); }
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }
            if (addressRepository == null) { throw new ArgumentNullException(nameof(addressRepository)); }

            OvertureClient = overtureClient;
            CartRepository = cartRepository;
            RecurringOrderCartViewModelFactory = recurringOrderCartViewModelFactory;
            ImageService = imageService;
            LookupService = lookupService;
            RecurringOrdersRepository = recurringOrdersRepository;
            ComposerContext = composerContext;
            AddressRepository = addressRepository;
        }

        public async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelAsync(GetRecurringOrderCartsViewModelParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new RecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(param).ConfigureAwait(false);

            var tasks = carts.Select(pc => CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = pc,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }));
            var viewModels = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new RecurringOrderCartsViewModel
            {
                RecurringOrderCartViewModelList = viewModels.OrderBy(v => v.NextOccurence).ToList(),
            };            
        }

        public async Task<IRecurringOrderCartViewModel> CreateCartViewModelAsync(CreateRecurringOrderCartViewModelParam param)
        {
            var lineItems = param.Cart.GetLineItems();

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
            };

            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).ConfigureAwait(false);

            param.PaymentMethodDisplayNames = methodDisplayNames;

            var roProgramNames = lineItems.Select(x => x.RecurringOrderProgramName)
                                                        .Where(l => !string.IsNullOrEmpty(l))
                                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                                        .ToList();
            var programTasks = roProgramNames.Select(programName => RecurringOrdersRepository.GetRecurringOrderProgram(ComposerContext.Scope, programName));
            var programs = await Task.WhenAll(programTasks).ConfigureAwait(false);
            param.RecurringOrderPrograms = programs.ToList();

            var vm = RecurringOrderCartViewModelFactory.CreateRecurringOrderCartViewModel(param);

            return vm;
        }

        public async Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new LightRecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam {
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                CustomerId = param.CustomerId
            }).ConfigureAwait(false);

            var tasks = carts.Select(pc => CreateLightCartViewModelAsync(new CreateLightRecurringOrderCartViewModelParam
            {
                Cart = pc,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
            }));
            var viewModels = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new LightRecurringOrderCartsViewModel
            {
                RecurringOrderCarts = viewModels.OrderBy(v => v.NextOccurence).ToList(),
            };
        }

        private async Task<LightRecurringOrderCartViewModel> CreateLightCartViewModelAsync(CreateLightRecurringOrderCartViewModelParam param)
        {
            var lineItems = param.Cart.GetLineItems();

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
            };

            var vm = RecurringOrderCartViewModelFactory.CreateLightRecurringOrderCartViewModel(param);

            return vm;
        }
         
        public async Task<IRecurringOrderCartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param)
        {
            var emptyVm = GetEmptyRecurringOrderCartViewModel();

            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return emptyVm;

            if(string.Equals(param.CartName, CartConfiguration.ShoppingCartName, StringComparison.OrdinalIgnoreCase))
                return emptyVm;

            var cart = await CartRepository.GetCartAsync(new GetCartParam {
                CartName = param.CartName,
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                ExecuteWorkflow = true,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }).ConfigureAwaitWithCulture(false);

            return vm;
        }

        public async Task<IRecurringOrderCartViewModel> UpdateRecurringOrderCartShippingAddressAsync(UpdateRecurringOrderCartShippingAddressParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return GetEmptyRecurringOrderCartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));
            if (param.ShippingAddressId == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.ShippingAddressId)));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.ScopeId,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments.First();
            var newAddress = await AddressRepository.GetAddressByIdAsync(param.ShippingAddressId).ConfigureAwaitWithCulture(false);

            if(newAddress == null)
                throw new InvalidOperationException("Address not found");

            shipment.Address = newAddress;

            if (param.UseSameForShippingAndBilling)
            {
                var payment = cart.Payments.First();
                if (payment == null)
                    throw new InvalidOperationException("No payment");
                payment.BillingAddress = newAddress;
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart)).ConfigureAwaitWithCulture(false);

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }).ConfigureAwait(false);

            return vm;
        }
                 
        private IRecurringOrderCartViewModel GetEmptyRecurringOrderCartViewModel()
        {
            var emptyVm = new CartViewModel();
            return emptyVm.AsExtensionModel<IRecurringOrderCartViewModel>();
        }

        public async Task<IRecurringOrderCartViewModel> UpdateRecurringOrderCartBillingAddressAsync(UpdateRecurringOrderCartBillingAddressParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return GetEmptyRecurringOrderCartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));
            if (param.BillingAddressId == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.BillingAddressId)));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.ScopeId,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments.First();
            var newAddress = await AddressRepository.GetAddressByIdAsync(param.BillingAddressId).ConfigureAwaitWithCulture(false);
            var payment = cart.Payments.First();
            if (payment == null)
                throw new InvalidOperationException("No payment");

            if(newAddress != null)
                throw new InvalidOperationException("Address not found");

            payment.BillingAddress = newAddress;

            if (param.UseSameForShippingAndBilling)
            {
                shipment.Address = newAddress;
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart)).ConfigureAwaitWithCulture(false);

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }).ConfigureAwait(false);

            return vm;
        }

        public async Task<RecurringOrderCartsViewModel> UpdateRecurringOrderCartNextOccurenceAsync(UpdateRecurringOrderCartNextOccurenceParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new RecurringOrderCartsViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));

            //get customer cart 
            var cart = await CartRepository.GetCartAsync(new GetCartParam {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                ExecuteWorkflow = false
            }).ConfigureAwaitWithCulture(false);

            //get customer recurring lineitems
            var listOfRecurringLineItems = await RecurringOrdersRepository.GetRecurringOrderTemplates(param.Scope, param.CustomerId).ConfigureAwaitWithCulture(false);

            if (listOfRecurringLineItems == null)
                throw new InvalidOperationException($"Recurring lineItems for customer {param.CustomerId} not found");

            var recurringLineItem = new RecurringOrderLineItem();
            var continueShipment = true;

            //We need to conserve the same time
            foreach (var shipment in cart.Shipments)
            {
                foreach (var lineitem in shipment.LineItems)
                {
                    if (RecurringOrderCartHelper.IsRecurringOrderLineItemValid(lineitem))
                    {
                        var recurringOrderLineitem = listOfRecurringLineItems.RecurringOrderLineItems?.FirstOrDefault(l =>
                            RecurringOrderTemplateHelper.IsLineItemAndRecurringTemplateLineItemSameProduct(lineitem, l));

                        if (recurringOrderLineitem != null)
                        {
                            recurringLineItem = recurringOrderLineitem;
                            continueShipment = false;
                            break;
                        }
                    }
                }
                if (!continueShipment)
                    break;
            }

            var newDate = param.NextOccurence;
            if (Guid.Empty != recurringLineItem.RecurringOrderLineItemId)
            {
                var nextOccurenceWithTime = recurringLineItem.NextOccurence;

                newDate = new DateTime(param.NextOccurence.Year, param.NextOccurence.Month, param.NextOccurence.Day,
                                        nextOccurenceWithTime.Hour, nextOccurenceWithTime.Minute, nextOccurenceWithTime.Second, DateTimeKind.Utc);
            }                       

            var listOfRecurringOrderLineItemsUpdated = await CartRepository.RescheduleRecurringCartAsync(new RescheduleRecurringCartParam()
            {
                CustomerId = param.CustomerId,
                NextOccurence = newDate,
                Scope = param.Scope,
                CartName = param.CartName
            }).ConfigureAwait(false);
            
            return await GetRecurringOrderCartListViewModelAsync(new GetRecurringOrderCartsViewModelParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CustomerId = param.CustomerId,
                CultureInfo =param.CultureInfo
            }).ConfigureAwaitWithCulture(false);
        }
    }
}

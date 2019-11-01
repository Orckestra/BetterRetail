using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Helper;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

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
        protected IComposerRequestContext ComposerContext { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected ICouponViewService CouponViewService { get; private set; }
        protected IRecurringCartUrlProvider RecurringCartUrlProvider { get; private set; }
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public RecurringOrderCartsViewService(
            ICartRepository cartRepository,
            IOvertureClient overtureClient,
            IRecurringOrderCartViewModelFactory recurringOrderCartViewModelFactory,
            IImageService imageService,
            ILookupService lookupService,
            IRecurringOrdersRepository recurringOrdersRepository,
            IComposerRequestContext composerContext,
            IAddressRepository addressRepository,
            ICouponViewService couponViewService,
            IRecurringCartUrlProvider recurringCartUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            if (cartRepository == null) { throw new ArgumentNullException(nameof(cartRepository)); }
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (recurringOrderCartViewModelFactory == null) { throw new ArgumentNullException(nameof(recurringOrderCartViewModelFactory)); }
            if (imageService == null) { throw new ArgumentNullException(nameof(imageService)); }
            if (lookupService == null) { throw new ArgumentNullException(nameof(lookupService)); }
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }
            if (addressRepository == null) { throw new ArgumentNullException(nameof(addressRepository)); }
            if (couponViewService == null) { throw new ArgumentNullException(nameof(couponViewService)); }
            if (recurringCartUrlProvider == null) { throw new ArgumentNullException(nameof(recurringCartUrlProvider)); }
            if (recurringScheduleUrlProvider == null) { throw new ArgumentNullException(nameof(recurringScheduleUrlProvider)); }

            OvertureClient = overtureClient;
            CartRepository = cartRepository;
            RecurringOrderCartViewModelFactory = recurringOrderCartViewModelFactory;
            ImageService = imageService;
            LookupService = lookupService;
            RecurringOrdersRepository = recurringOrdersRepository;
            ComposerContext = composerContext;
            AddressRepository = addressRepository;
            CouponViewService = couponViewService;
            RecurringCartUrlProvider = recurringCartUrlProvider;
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider;
            RecurringOrdersSettings = recurringOrdersSettings;
        }

        public virtual async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelAsync(GetRecurringOrderCartsViewModelParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return new RecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(param).ConfigureAwait(false);

            return await GetRecurringOrderCartListViewModelFromCartsAsync(new GetRecurringOrderCartsViewModelFromCartsParam
            {
                Carts = carts,
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            }).ConfigureAwaitWithCulture(false);
        }

        public virtual async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelFromCartsAsync(GetRecurringOrderCartsViewModelFromCartsParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return new RecurringOrderCartsViewModel();

            var carts = param.Carts;

            var tasks = carts.Select(pc => CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = pc,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }));
            var viewModels = await Task.WhenAll(tasks).ConfigureAwait(false);

            var comparer = new RecurringOrderCartViewModelNextOcurrenceComparer();
            return new RecurringOrderCartsViewModel
            {
                RecurringOrderCartViewModelList = viewModels.OrderBy(v => v, comparer).ToList(),
            };
        }

        public virtual async Task<CartViewModel> CreateCartViewModelAsync(CreateRecurringOrderCartViewModelParam param)
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

            await ExtendLineItems(vm, ComposerContext.Scope, ComposerContext.CustomerId, ComposerContext.CultureInfo).ConfigureAwaitWithCulture(false);

            return vm;
        }

        protected virtual async Task<bool> ExtendLineItems(CartViewModel vm, string scope, Guid customerId, CultureInfo culture)
        {
            var recurringOrderLineItems = await RecurringOrdersRepository.GetRecurringOrderTemplates(scope, customerId).ConfigureAwaitWithCulture(false);

            foreach(var lineItem in vm.LineItemDetailViewModels)
            {
                var roLineItemVm = lineItem.AsExtensionModel<IRecurringOrderLineItemViewModel>();

                roLineItemVm.RecurringScheduleDetailUrl = FindUrl(recurringOrderLineItems, lineItem, culture);
            }

            return true;
        }

        protected virtual string FindUrl(ListOfRecurringOrderLineItems recurringOrderLineItems, LineItemDetailViewModel lineItem, CultureInfo culture)
        {
            //TODO: Change if a customer can have more than one template of the same product/variant
            var recurringLineItem = recurringOrderLineItems.RecurringOrderLineItems.SingleOrDefault(r => string.Compare(r.ProductId, lineItem.ProductId, StringComparison.OrdinalIgnoreCase) == 0
                && string.Compare(r.VariantId, lineItem.VariantId, StringComparison.OrdinalIgnoreCase) == 0);

            if (recurringLineItem == null)
                return string.Empty;

            var id = recurringLineItem.RecurringOrderLineItemId;

            var url = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam()
            {
                CultureInfo = culture,
                RecurringScheduleId = id.ToString()
            });

            return url;
        }

        public virtual async Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
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

        protected virtual async Task<LightRecurringOrderCartViewModel> CreateLightCartViewModelAsync(CreateLightRecurringOrderCartViewModelParam param)
        {
            var lineItems = param.Cart.GetLineItems();

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems, ImageConfiguration.RecurringCartSummaryThumbnailImageSize).ConfigureAwait(false),
            };

            var vm = RecurringOrderCartViewModelFactory.CreateLightRecurringOrderCartViewModel(param);

            return vm;
        }

        public virtual async Task<CartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param)
        {
            var emptyVm = GetEmptyRecurringOrderCartViewModel();

            if (!RecurringOrdersSettings.Enabled)
                return emptyVm;

            if (string.Equals(param.CartName, CartConfiguration.ShoppingCartName, StringComparison.OrdinalIgnoreCase))
                return emptyVm;

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
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

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartShippingAddressAsync(UpdateRecurringOrderCartShippingAddressParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
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
            if (newAddress == null)
                throw new InvalidOperationException("Address not found");
            shipment.Address = newAddress;

            var payment = cart.Payments.First();
            if (payment == null)
                throw new InvalidOperationException("No payment");

            if (param.UseSameForShippingAndBilling)
            {
                payment.BillingAddress = newAddress;
            }
            else if (param.BillingAddressId != Guid.Empty)
            {
                var newbillingAddress = await AddressRepository.GetAddressByIdAsync(param.BillingAddressId).ConfigureAwaitWithCulture(false);
                if (newbillingAddress == null)
                    throw new InvalidOperationException("Address not found");
                payment.BillingAddress = newbillingAddress;
                payment.BillingAddressId = newbillingAddress.Id; 
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

        protected virtual CartViewModel GetEmptyRecurringOrderCartViewModel()
        {
            return new CartViewModel();
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartBillingAddressAsync(UpdateRecurringOrderCartBillingAddressParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
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
            if (newAddress == null)
                throw new InvalidOperationException("Address not found");
            payment.BillingAddress = newAddress;
            payment.BillingAddressId = newAddress.Id;

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

        public virtual async Task<RecurringOrderCartsRescheduleResultViewModel> UpdateRecurringOrderCartNextOccurenceAsync(UpdateRecurringOrderCartNextOccurenceParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return new RecurringOrderCartsRescheduleResultViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));

            //get customer cart 
            var cart = await CartRepository.GetCartAsync(new GetCartParam {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                ExecuteWorkflow = false
            }).ConfigureAwaitWithCulture(false);

            var originalCartName = param.CartName;

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

            var vm = new RecurringOrderCartsRescheduleResultViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo
            }).ConfigureAwait(false);

            vm.RescheduledCartHasMerged = !carts.Any(rc => string.Compare(rc.Name, originalCartName, StringComparison.OrdinalIgnoreCase) == 0);

            var cartsVm = await GetRecurringOrderCartListViewModelFromCartsAsync(new GetRecurringOrderCartsViewModelFromCartsParam
            {
                Carts = carts,
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            });

            vm.RecurringOrderCartsViewModel = cartsVm;

            var url = RecurringCartUrlProvider.GetRecurringCartsUrl(new GetRecurringCartsUrlParam
            {
                CultureInfo = param.CultureInfo
            });

            vm.RecurringCartsUrl = url;

            return vm;
        }

        public virtual async Task<CartViewModel> RemoveLineItemAsync(RemoveRecurringCartLineItemParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return GetEmptyRecurringOrderCartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentNullException(nameof(param.Scope), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.Scope)));
            if (param.CultureInfo == null) throw new ArgumentNullException(nameof(param.CultureInfo), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CultureInfo)));
            if (param.LineItemId == Guid.Empty) throw new ArgumentNullException(nameof(param.LineItemId), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.LineItemId)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentNullException(nameof(param.CustomerId), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId)));

            await CartRepository.RemoveRecurringCartLineItemAsync(param).ConfigureAwait(false);

            return await GetRecurringOrderCartViewModelAsync(new GetRecurringOrderCartViewModelParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwaitWithCulture(false);
        }

        public virtual async Task<CartViewModel> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return GetEmptyRecurringOrderCartViewModel();

            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException("param.ScopeId is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException("param.LineItemId is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (param.Quantity < 1) { throw new ArgumentException("param.Quantity must be greater than 0", "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException("param.BaseUrl is required", "param"); }

            var cart = await CartRepository.UpdateLineItemAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(cart.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.ScopeId
            }).ConfigureAwait(false);

            var vmParam = new CreateRecurringOrderCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            return viewModel;
        }

        public virtual async Task<bool> UpdateRecurringOrderCartsAddressesAsync(UpdateRecurringOrderCartsAddressesParam param)
        {
            if (!RecurringOrdersSettings.Enabled)
                return false;

            var roCarts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.ScopeId
            }).ConfigureAwait(false);

            var tasks = new List<Task>();
            var addressId = param.AddressId;

            foreach (var roc in roCarts)
            {
                var updateShippingAddress = false;
                var updateBillingAddress = false;

                var shipment = roc.Shipments?.FirstOrDefault();
                if (shipment != null)
                {
                    if (shipment.Address.Id == addressId)
                    {
                        updateShippingAddress = true;
                    }
                    var payment = roc.Payments?.FirstOrDefault();
                    if (payment != null)
                    {
                        if (payment.BillingAddress.Id == addressId)
                        {
                            updateBillingAddress = true;
                        }
                    }
                }

                if (updateShippingAddress)
                {
                    tasks.Add(UpdateRecurringOrderCartShippingAddressAsync(new UpdateRecurringOrderCartShippingAddressParam()
                    {
                        BaseUrl = param.BaseUrl,
                        CartName = roc.Name,
                        CultureInfo = param.CultureInfo,
                        CustomerId = param.CustomerId,
                        ScopeId = param.ScopeId,
                        ShippingAddressId = addressId,
                        UseSameForShippingAndBilling = updateBillingAddress
                    }));
                }
                else if (updateBillingAddress)
                {
                    tasks.Add(UpdateRecurringOrderCartBillingAddressAsync(new UpdateRecurringOrderCartBillingAddressParam()
                    {
                        BaseUrl = param.BaseUrl,
                        CartName = roc.Name,
                        CultureInfo = param.CultureInfo,
                        CustomerId = param.CustomerId,
                        ScopeId = param.ScopeId,
                        BillingAddressId = addressId,
                        UseSameForShippingAndBilling = false
                    }));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwaitWithCulture(false);
            return true;
        }
    }
}
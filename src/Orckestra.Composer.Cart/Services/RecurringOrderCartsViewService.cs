using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

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
            IComposerContext composerContext,
            IAddressRepository addressRepository,
            ICouponViewService couponViewService,
            IRecurringCartUrlProvider recurringCartUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            RecurringOrderCartViewModelFactory = recurringOrderCartViewModelFactory ?? throw new ArgumentNullException(nameof(recurringOrderCartViewModelFactory));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            RecurringOrdersRepository = recurringOrdersRepository ?? throw new ArgumentNullException(nameof(recurringOrdersRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            AddressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            CouponViewService = couponViewService ?? throw new ArgumentNullException(nameof(couponViewService));
            RecurringCartUrlProvider = recurringCartUrlProvider ?? throw new ArgumentNullException(nameof(recurringCartUrlProvider));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        public virtual async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelAsync(GetRecurringOrderCartsViewModelParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new RecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(param).ConfigureAwait(false);

            return await GetRecurringOrderCartListViewModelFromCartsAsync(new GetRecurringOrderCartsViewModelFromCartsParam
            {
                Carts = carts,
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            });
        }

        public virtual async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelFromCartsAsync(GetRecurringOrderCartsViewModelFromCartsParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new RecurringOrderCartsViewModel();

            var tasks = param.Carts.Select(pc => CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
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
            });

            param.PaymentMethodDisplayNames = methodDisplayNames;

            var programTasks = new Dictionary<string, Task<Overture.ServiceModel.RecurringOrders.RecurringOrderProgram>>(StringComparer.OrdinalIgnoreCase);

            foreach(var el in lineItems)
            {
                var programName = el.RecurringOrderProgramName;
                if (string.IsNullOrEmpty(programName) || programTasks.ContainsKey(programName)) { continue; }

                programTasks.Add(programName, RecurringOrdersRepository.GetRecurringOrderProgram(ComposerContext.Scope, programName));
            }

            var programs = await Task.WhenAll(programTasks.Values);
            param.RecurringOrderPrograms = programs.ToList();

            var vm = RecurringOrderCartViewModelFactory.CreateRecurringOrderCartViewModel(param);

            await ExtendLineItems(vm, ComposerContext.Scope, ComposerContext.CustomerId, ComposerContext.CultureInfo);

            return vm;
        }

        protected virtual async Task<bool> ExtendLineItems(CartViewModel vm, string scope, Guid customerId, CultureInfo culture)
        {
            var recurringOrderLineItems = await RecurringOrdersRepository.GetRecurringOrderTemplates(scope, customerId).ConfigureAwait(false);

            foreach (var lineItem in vm.LineItemDetailViewModels)
            {
                var roLineItemVm = lineItem.AsExtensionModel<IRecurringOrderLineItemViewModel>();

                roLineItemVm.RecurringScheduleDetailUrl = FindUrl(recurringOrderLineItems, lineItem, culture);
            }

            return true;
        }

        protected virtual string FindUrl(ListOfRecurringOrderLineItems recurringOrderLineItems, LineItemDetailViewModel lineItem, CultureInfo culture)
        {
            //TODO: Change if a customer can have more than one template of the same product/variant
            var recurringLineItem = recurringOrderLineItems.RecurringOrderLineItems.SingleOrDefault(r => 
                string.Equals(r.ProductId, lineItem.ProductId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(r.VariantId, lineItem.VariantId, StringComparison.OrdinalIgnoreCase));

            if (recurringLineItem == null) return string.Empty;

            var url = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam()
            {
                CultureInfo = culture,
                RecurringScheduleId = recurringLineItem.RecurringOrderLineItemId.ToString()
            });

            return url;
        }

        public virtual async Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new LightRecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam
            {
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

            var viewModels = await Task.WhenAll(tasks);

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
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
            };

            var vm = RecurringOrderCartViewModelFactory.CreateLightRecurringOrderCartViewModel(param);

            return vm;
        }

        public virtual async Task<CartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param)
        {
            var emptyVm = new CartViewModel();

            if (!RecurringOrdersSettings.Enabled) return emptyVm;

            if (string.Equals(param.CartName, CartConfiguration.ShoppingCartName, StringComparison.OrdinalIgnoreCase)) return emptyVm;

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
            });

            return vm;
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartShippingAddressAsync(UpdateRecurringOrderCartShippingAddressParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new CartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.ShippingAddressId == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ShippingAddressId)), nameof(param));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.ScopeId,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments.First();
            var newAddress = await AddressRepository.GetAddressByIdAsync(param.ShippingAddressId) 
                ?? throw new InvalidOperationException("Address not found");
            
            shipment.Address = newAddress;

            var payment = cart.Payments.FirstOrDefault() ?? throw new InvalidOperationException("No payment");     

            if (param.UseSameForShippingAndBilling)
            {
                payment.BillingAddress = newAddress;
            }
            else if (param.BillingAddressId != Guid.Empty)
            {
                var newbillingAddress = await AddressRepository.GetAddressByIdAsync(param.BillingAddressId) 
                    ?? throw new InvalidOperationException("Address not found");

                payment.BillingAddress = newbillingAddress;
                payment.BillingAddressId = newbillingAddress.Id;
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart));

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl
            });

            return vm;
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartBillingAddressAsync(UpdateRecurringOrderCartBillingAddressParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new CartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.BillingAddressId == null) throw new ArgumentException(GetMessageOfNull(nameof(param.BillingAddressId)), nameof(param));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.ScopeId,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var newAddress = await AddressRepository.GetAddressByIdAsync(param.BillingAddressId) 
                ?? throw new InvalidOperationException("Address not found");

            var payment = cart.Payments.FirstOrDefault() 
                ?? throw new InvalidOperationException("No payment");

            payment.BillingAddress = newAddress;
            payment.BillingAddressId = newAddress.Id;

            if (param.UseSameForShippingAndBilling)
            {
                cart.Shipments.First().Address = newAddress;
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart));

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl
            });

            return vm;
        }

        public virtual async Task<RecurringOrderCartsRescheduleResultViewModel> UpdateRecurringOrderCartNextOccurenceAsync(UpdateRecurringOrderCartNextOccurenceParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new RecurringOrderCartsRescheduleResultViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param));

            //get customer cart 
            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                ExecuteWorkflow = false
            }).ConfigureAwait(false);

            //get customer recurring lineitems
            var listOfRecurringLineItems = await RecurringOrdersRepository.GetRecurringOrderTemplates(param.Scope, param.CustomerId)
                ?? throw new InvalidOperationException($"Recurring lineItems for customer {param.CustomerId} not found");

            var continueShipment = true;
            var newDate = param.NextOccurence;
            if (listOfRecurringLineItems.RecurringOrderLineItems != null && listOfRecurringLineItems.RecurringOrderLineItems.Any())
            {
                var lookup = listOfRecurringLineItems.RecurringOrderLineItems.ToLookup(el => el.ProductId, el => el, StringComparer.OrdinalIgnoreCase);

                //We need to conserve the same time
                foreach (var shipment in cart.Shipments)
                {
                    foreach (var lineitem in shipment.LineItems)
                    {
                        if (string.IsNullOrEmpty(lineitem.RecurringOrderFrequencyName) 
                            || string.IsNullOrEmpty(lineitem.RecurringOrderProgramName)) { continue; }

                        var recurringOrderLineitem = lookup[lineitem.ProductId]?.FirstOrDefault(x => x.VariantId == lineitem.VariantId);
                        if (recurringOrderLineitem == null) { continue; }

                        //V: Kept the same logic just for case, but think checking a guid was provided to figure out, have we found an object or not
                        if (recurringOrderLineitem.RecurringOrderLineItemId != Guid.Empty)
                        {
                            var nextOccurenceWithTime = recurringOrderLineitem.NextOccurence;

                            newDate = new DateTime(newDate.Year, newDate.Month, newDate.Day,
                                nextOccurenceWithTime.Hour, nextOccurenceWithTime.Minute, nextOccurenceWithTime.Second, DateTimeKind.Utc);
                        }
                        continueShipment = false;
                        break;
                    }
                    if (!continueShipment) break;
                }
            }

            var listOfRecurringOrderLineItemsUpdated = await CartRepository.RescheduleRecurringCartAsync(new RescheduleRecurringCartParam()
            {
                CustomerId = param.CustomerId,
                NextOccurence = newDate,
                Scope = param.Scope,
                CartName = param.CartName
            });

            var vm = new RecurringOrderCartsRescheduleResultViewModel();

            var carts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo
            });

            vm.RescheduledCartHasMerged = !carts.Any(rc => string.Equals(rc.Name, param.CartName, StringComparison.OrdinalIgnoreCase));

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
            if (!RecurringOrdersSettings.Enabled) return new CartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));
            if (param.LineItemId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param));

            await CartRepository.RemoveRecurringCartLineItemAsync(param).ConfigureAwait(false);

            return await GetRecurringOrderCartViewModelAsync(new GetRecurringOrderCartViewModelParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            });
        }

        public virtual async Task<CartViewModel> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new CartViewModel();

            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var cart = await CartRepository.UpdateLineItemAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(cart.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.ScopeId
            });

            var vmParam = new CreateRecurringOrderCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam);

            return viewModel;
        }

        public virtual async Task<bool> UpdateRecurringOrderCartsAddressesAsync(UpdateRecurringOrderCartsAddressesParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return false;

            var roCarts = await CartRepository.GetRecurringCartsAsync(new GetRecurringOrderCartsViewModelParam
            {
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
                    if (payment != null && payment.BillingAddress.Id == addressId)
                    {
                        updateBillingAddress = true;
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

            await Task.WhenAll(tasks);
            return true;
        }
    }
}
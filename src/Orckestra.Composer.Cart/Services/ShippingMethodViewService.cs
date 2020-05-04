using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Shipping Methods.
    /// </summary>
    public class ShippingMethodViewService : IShippingMethodViewService
    {
        protected IFulfillmentMethodRepository FulfillmentMethodRepository { get; set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ICartRepository CartRepository { get; private set; }
        protected ICartService CartService { get; private set; }
        protected IRecurringOrderTemplateViewModelFactory RecurringOrderTemplateViewModelFactory { get; private set; }
        protected IRecurringOrderCartsViewService RecurringOrderCartsViewService { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public ShippingMethodViewService(
            IFulfillmentMethodRepository fulfillmentMethodRepository,
            ICartViewModelFactory cartViewModelFactory,
            ICartRepository cartRepository,
            ICartService cartService,
            IRecurringOrderTemplateViewModelFactory recurringOrderTemplateViewModelFactory,
            IRecurringOrderCartsViewService recurringOrderCartsViewService,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            FulfillmentMethodRepository = fulfillmentMethodRepository ?? throw new ArgumentNullException(nameof(fulfillmentMethodRepository));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            RecurringOrderTemplateViewModelFactory = recurringOrderTemplateViewModelFactory ?? throw new ArgumentNullException(nameof(recurringOrderTemplateViewModelFactory));
            RecurringOrderCartsViewService = recurringOrderCartsViewService ?? throw new ArgumentNullException(nameof(recurringOrderCartsViewService));
            RecurringOrdersSettings = recurringOrdersSettings;
        }

        /// <summary>
        /// Get the Shipping methods available for a shipment.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        public async virtual Task<ShippingMethodsViewModel> GetShippingMethodsAsync(GetShippingMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var shippingMethods = await GetFulfillmentMethods(param).ConfigureAwait(false);

            if (shippingMethods == null) { return null; }

            var shippingMethodViewModels = shippingMethods
                .Select(sm => CartViewModelFactory.GetShippingMethodViewModel(sm, param.CultureInfo))
                .ToList();

            return new ShippingMethodsViewModel
            {
                ShippingMethods = shippingMethodViewModels
            };
        }

        public async virtual Task<ShippingMethodTypesViewModel> GetShippingMethodTypesAsync(GetShippingMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var shippingMethods = await GetFulfillmentMethods(param).ConfigureAwait(false);

            if (shippingMethods == null)
            {
                return null;
            }

            var shippingMethodTypeViewModels = shippingMethods
                .Select(sm => CartViewModelFactory.GetShippingMethodViewModel(sm, param.CultureInfo))
                .Where(FilterShippingMethodView)
                .GroupBy(sm => sm.FulfillmentMethodType)
                .Select(type => CartViewModelFactory.GetShippingMethodTypeViewModel(type.Key, type.ToList(), param.CultureInfo))
                .OrderBy(OrderShippingMethodTypeView)
                .ToList();

            return new ShippingMethodTypesViewModel
            {
                ShippingMethodTypes = shippingMethodTypeViewModels
            };
        }

        public virtual bool FilterShippingMethodView(ShippingMethodViewModel sippingMethod)
        {
            return sippingMethod.FulfillmentMethodType == FulfillmentMethodType.Shipping || sippingMethod.FulfillmentMethodType == FulfillmentMethodType.PickUp;
        }

        public virtual int OrderShippingMethodTypeView(ShippingMethodTypeViewModel sippingMethodType)
        {
            switch(sippingMethodType.FulfillmentMethodType)
            {
                case FulfillmentMethodType.Shipping: return 0;
                case FulfillmentMethodType.PickUp: return 1;
                default: return 2;
            }
        }

        /// <summary>
        /// Get the Shipping methods available for a shipment. Calls the GetCart to get the shipment Id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        public async virtual Task<ShippingMethodsViewModel> GetRecurringCartShippingMethodsAsync(GetShippingMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            //misleading method name, creates a cart if it does not exist, not just a "get" call
            await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = string.Empty,
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            return await GetShippingMethodsAsync(param);
        }

        protected virtual Task<List<FulfillmentMethod>> GetFulfillmentMethods(GetShippingMethodsParam param)
        {
            return FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(param);
        }

        public async Task<CartViewModel> SetCheapestShippingMethodAsync(SetCheapestShippingMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope

            }).ConfigureAwait(false);

            await EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                ForceUpdate = true

            });

            return await CartService.UpdateCartAsync(new UpdateCartViewModelParam
            {
                BaseUrl = param.BaseUrl,
                BillingCurrency = cart.BillingCurrency,
                CartName = cart.Name,
                CartType = cart.CartType,
                Coupons = cart.Coupons,
                CultureInfo = param.CultureInfo,
                Customer = cart.Customer,
                CustomerId = cart.CustomerId,
                OrderLocation = cart.OrderLocation,
                Payments = cart.Payments,
                PropertyBag = cart.PropertyBag,
                Scope = cart.ScopeId,
                Shipments = cart.Shipments,
                Status = cart.Status
            });
        }

        /// <summary>
        /// Estimates the shipping method. Does not save the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<ShippingMethodViewModel> EstimateShippingAsync(EstimateShippingParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.Cart == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Cart)), nameof(param)); }

            var shippingMethods = await GetShippingMethodsForShippingEstimationAsync(param);
            var selectedMethod = GetCheapestShippingMethodViewModel(shippingMethods);
            var firstShipment = GetShipment(param.Cart);

            if (param.ForceUpdate || firstShipment.FulfillmentMethod == null)
            {
                firstShipment.FulfillmentMethod = selectedMethod;
            }

            var vm = CartViewModelFactory.GetShippingMethodViewModel(firstShipment.FulfillmentMethod, param.CultureInfo);
            return vm;
        }

        protected virtual async Task<List<FulfillmentMethod>> GetShippingMethodsForShippingEstimationAsync(EstimateShippingParam param)
        {
            var shippingMethods = await GetFulfillmentMethods(new GetShippingMethodsParam
            {
                Scope = param.Cart.ScopeId,
                CultureInfo = param.CultureInfo,
                CustomerId = param.Cart.CustomerId,
                CartName = param.Cart.Name
            }).ConfigureAwait(false);

            if (shippingMethods == null || !shippingMethods.Any()) { throw new InvalidDataException("No shipping method was defined."); }

            return shippingMethods;
        }

        protected virtual FulfillmentMethod GetCheapestShippingMethodViewModel(IEnumerable<FulfillmentMethod> shippingMethods)
        {
            var cheapestShippingMethod = shippingMethods.Aggregate((curMin, x) => x.Cost < curMin.Cost ? x : curMin);
            return cheapestShippingMethod;
        }

        protected virtual Shipment GetShipment(Overture.ServiceModel.Orders.Cart cart)
        {
            var shipment = cart.Shipments.FirstOrDefault() ?? throw new InvalidDataException("No shipment was found in Cart.");
            return shipment;
        }

        /// <summary>
        /// Get the Shipping methods available in the scope.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        public async virtual Task<RecurringOrdersTemplatesShippingMethodsViewModel> GetShippingMethodsScopeAsync(GetShippingMethodsScopeParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var fulfillmentMethods = await FulfillmentMethodRepository.GetFulfillmentMethods(param.Scope).ConfigureAwait(false);

            if (fulfillmentMethods == null) { return null; }

            var shippingMethodViewModels = fulfillmentMethods.FulfillmentMethods
                .Select(sm => RecurringOrderTemplateViewModelFactory.GetShippingMethodViewModel(sm, param.CultureInfo)).ToList();

            return new RecurringOrdersTemplatesShippingMethodsViewModel
            {
                ShippingMethods = shippingMethodViewModels
            };
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartShippingMethodAsync(UpdateRecurringOrderCartShippingMethodParam param)
        {
            if (!RecurringOrdersSettings.Enabled) return new CartViewModel();

            if (param == null) throw new ArgumentNullException(nameof(param));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments?.FirstOrDefault() ?? throw new InvalidOperationException("No shipment was found in the cart.");

            var fulfillmentMethods = await FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(new GetShippingMethodsParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            });

            if (fulfillmentMethods == null)
                throw new InvalidOperationException($"No fulfillmentMethods was found for the cart name ({param.CartName}).");

            var fulfillmentMethod = fulfillmentMethods.SingleOrDefault(f => f.ShippingProviderId == param.ShippingProviderId.ToGuid() &&
                    string.Equals(f.Name, param.ShippingMethodName, StringComparison.InvariantCultureIgnoreCase));

            shipment.FulfillmentMethod = fulfillmentMethod 
                ?? throw new InvalidOperationException($"The fulfillmentMethod ({param.ShippingProviderId}) was not found.");

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart));

            var vm = await RecurringOrderCartsViewService.CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = new CultureInfo(updatedCart.CultureName),
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            });

            return vm;
        }
    }
}
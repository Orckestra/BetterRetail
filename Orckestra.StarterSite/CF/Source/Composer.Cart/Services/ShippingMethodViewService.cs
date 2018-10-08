using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Orders;

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

        public ShippingMethodViewService(
            IFulfillmentMethodRepository fulfillmentMethodRepository, 
            ICartViewModelFactory cartViewModelFactory,
            ICartRepository cartRepository, 
            ICartService cartService)
        {
            if (fulfillmentMethodRepository == null) { throw new ArgumentNullException("fulfillmentMethodRepository"); }
            if (cartViewModelFactory == null) { throw new ArgumentNullException("cartViewModelFactory"); }
            if (cartRepository == null) { throw new ArgumentNullException("cartRepository"); }
            if (cartService == null) { throw new ArgumentNullException("cartService"); }

            FulfillmentMethodRepository = fulfillmentMethodRepository;
            CartViewModelFactory = cartViewModelFactory;
            CartRepository = cartRepository;
            CartService = cartService;
        }

        /// <summary>
        /// Get the Shipping methods available for a shipment.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        public async virtual Task<ShippingMethodsViewModel> GetShippingMethodsAsync(GetShippingMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }

            var shippingMethods = await GetFulfillmentMethods(param).ConfigureAwait(false);

            if (shippingMethods == null)
            {
                return null;
            }

            var shippingMethodViewModels = shippingMethods
                .Select(sm => CartViewModelFactory.GetShippingMethodViewModel(sm, param.CultureInfo)).ToList();

            return new ShippingMethodsViewModel
            {
                ShippingMethods = shippingMethodViewModels
            };
        }

        protected virtual Task<List<FulfillmentMethod>> GetFulfillmentMethods(GetShippingMethodsParam param)
        {
            return FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(param);
        }

        public async Task<CartViewModel> SetCheapestShippingMethodAsync(SetCheapestShippingMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

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

            }).ConfigureAwait(false);

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

            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Estimates the shipping method. Does not save the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<ShippingMethodViewModel> EstimateShippingAsync(EstimateShippingParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }
            if (param.Cart == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Cart"), "param"); }

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

        protected async Task<List<FulfillmentMethod>> GetShippingMethodsForShippingEstimationAsync(EstimateShippingParam param)
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
            var cheapestShippingMethod =
               shippingMethods.OrderBy(sm => sm.Cost).First();

            return cheapestShippingMethod;
        }

        protected virtual Shipment GetShipment(Overture.ServiceModel.Orders.Cart cart)
        {
            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null)
            {
                throw new InvalidDataException("No shipment was found in Cart.");
            }

            return shipment;
        }
    }
}

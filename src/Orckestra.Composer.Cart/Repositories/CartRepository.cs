using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Coupons;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.LineItems;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Shipments;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using ServiceStack;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories
{
    public class CartRepository : ICartRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }
        protected IScopeProvider ScopeProvider { get; private set; }

        public CartRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeProvider scopeProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        /// <summary>
        /// Retrieves the list of carts belonging to a customer
        ///
        /// param.IncludeChildScopes is optional
        /// A value indicating whether to include carts found in child scopes.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>List of Cart Summaries</returns>
        public virtual Task<List<CartSummary>> GetCartsByCustomerIdAsync(GetCartsByCustomerIdParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var request = new GetCartsByCustomerIdRequest
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                IncludeChildScopes = param.IncludeChildScopes,
                CartType = param.CartType
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Retrieve a cart.
        /// The cart is created if it does not exist.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Processed Cart requested or an empty one if it doesn't exist</returns>
        public virtual Task<ProcessedCart> GetCartAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);

            var request = new GetCartRequest
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                CartName = param.CartName,
                CartType = param.CartType,
                //Reexecute price engine and promotion engine is automatically done at each request
                ExecuteWorkflow = param.ExecuteWorkflow,
                WorkflowToExecute = param.WorkflowToExecute
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Delete a cart
        /// </summary>
        /// <param name="param">Parameters to be used for deleting</param>
        /// <returns>Http web response of deleting operation</returns>
        public virtual async Task<HttpWebResponse> DeleteCartAsync(DeleteCartParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param));

            DeleteCartRequest request = new DeleteCartRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope
            };
            CacheKey cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);
            await CacheProvider.RemoveAsync(cacheKey);

            return await OvertureClient.SendAsync(request);
        }

        public virtual Task<PaymentMethod> SetDefaultCustomerPaymentMethod(SetDefaultCustomerPaymentMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentMethodId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }

            var request = new SetDefaultCustomerPaymentMethodRequest
            {
                CustomerId = param.CustomerId,
                Default = true,
                PaymentMethodId = param.PaymentMethodId,
                PaymentProviderName = param.PaymentProviderName,
                ScopeId = param.ScopeId
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Add line item to the cart.
        ///
        /// CartName will be created if needed
        /// CustomerId (guest customers) will be created if needed
        /// If the (product,variant) is already in the cart, the quantity will be increased;
        /// otherwise a new line will be added
        ///
        /// param.VariantId is optional
        ///
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details</returns>
        public virtual Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }

            var request = BuildAddLineItemRequestFromParam(param);
            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        protected virtual AddLineItemRequest BuildAddLineItemRequestFromParam(AddLineItemParam param)
        {
            return new AddLineItemRequest
            {
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                CartName = param.CartName,
                CartType = param.CartType,
                CustomerId = param.CustomerId,
                ProductId = param.ProductId,
                Quantity = param.Quantity,
                VariantId = param.VariantId,
                RecurringOrderFrequencyName = param.RecurringOrderFrequencyName,
                RecurringOrderProgramName = param.RecurringOrderProgramName,
                PropertyBag = param.PropertyBag
            };
        }

        /// <summary>
        /// Add one or more line items to a cart.
        /// In case cart does not exist - it will be created.
        /// If a product or a  variant already exists in a cart, it will be merged with adding ones.
        /// </summary>
        /// <param name="param">Parameters to be used for adding</param>
        /// <returns>Processed cart</returns>
        public virtual Task<ProcessedCart> AddLineItemsAsync(AddLineItemsParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param));
            if (param.LineItems == null || param.LineItems.Count == 0) throw new ArgumentException(GetMessageOfNullEmpty(nameof(param.LineItems)), nameof(param));

            AddOrUpdateLineItemsRequest request = new AddOrUpdateLineItemsRequest
            {
                CartName = param.CartName,
                CartType = param.CartType,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                LineItems = param.LineItems,
                ScopeId = param.Scope
            };

            CacheKey cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Adds a payment on the first shipment of the specified cart.
        /// </summary>
        /// <param name="param">Parameters used to add a payment to the cart.</param>
        /// <returns></returns>
        public virtual async Task<ProcessedCart> AddPaymentAsync(AddPaymentParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var request = BuildAddPaymentRequest(param);

            // NOTE (SIMON.BERUBE) : 
            // Not caching the result because of the MyWallet workflow when updating a payment...
            // When updating a payment we are doing the following OCC requests :
            //  1. RemovePaymentRequest (in PaymentRepository)
            //  2. AddPaymentRequest (in CartRepository)
            //  3. UpdatePaymentMethodRequest (in PaymentRepository)
            //  4. GetCartRequest (in CartRepository)
            //
            // Because PaymentRepository and CartRepository doesn't share the same cache 
            // (CacheProvider is registred as Transient in ComposerHost) if we cache the result of 
            // AddPaymentRequest after, when doing the GetCartRequest, we are getting a cached cart with
            // no payment method.
            //
            // The fix is to register the CacheProvider to be Singleton and let the cache client handle the 
            // lifetime of the cache OR to move the methods that use the BuildCartCacheKey method 
            // from PaymentRepository to CartRepository.
            return await OvertureClient.SendAsync(request).ConfigureAwait(false);
        }

        protected IReturn<ProcessedCart> BuildAddPaymentRequest(AddPaymentParam param)
        {
            return new AddPaymentRequest
            {
                Amount = param.Amount,
                BillingAddress = param.BillingAddress,
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope
            };
        }

        /// <summary>
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details</returns>
        public virtual Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var request = new RemoveLineItemRequest
            {
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                CartName = param.CartName,
                CartType = param.CartType,
                Id = param.LineItemId,
                CustomerId = param.CustomerId
            };

            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);
            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Removes many line items from the cart at once.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<ProcessedCart> RemoveLineItemsAsync(RemoveLineItemsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.LineItems == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.LineItems)), nameof(param)); }

            var array = new Guid[param.LineItems.Count];
            for (int i = 0; i < param.LineItems.Count; i++)
            {
                var currentLine = param.LineItems[i];
                if (currentLine.Id == Guid.Empty)
                {
                    throw new InvalidOperationException($"Line item with index {i} has empty id");
                }
                else if (string.IsNullOrWhiteSpace(currentLine.ProductId))
                {
                    throw new InvalidOperationException($"Line item with index {i} has null or white space product id");
                }
                array[i] = currentLine.Id;
            }
            var request = new RemoveLineItemsRequest
            {
                CartName = param.CartName,
                CartType = param.CartType,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                LineItemIds = array
            };

            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);
            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Update a lineItem in the cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details</returns>
        public virtual Task<ProcessedCart> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }

            var request = new UpdateLineItemRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CartType = param.CartType,
                CustomerId = param.CustomerId,
                GiftMessage = param.GiftMessage,
                GiftWrap = param.GiftWrap,
                Id = param.LineItemId,
                PropertyBag = param.PropertyBag,
                Quantity = param.Quantity,
                ScopeId = param.ScopeId,
                RecurringOrderFrequencyName = param.RecurringOrderFrequencyName,
                RecurringOrderProgramName = param.RecurringOrderProgramName
            };

            var cacheKey = BuildCartCacheKey(param.ScopeId, param.CustomerId, param.CartName);
            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Get line items of a cart of a customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Line items of a cart</returns>
        public virtual Task<List<LineItem>> GetLineItemsAsync(GetLineItemsParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param));

            GetLineItemsInCartRequest request = new GetLineItemsInCartRequest
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                CartName = param.CartName,
                CartType = param.CartType
            };

            //Avoid caching because of returning with type of line items
            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Adds a coupon to the Cart, then returns an instance of the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details.</returns>
        public virtual Task<ProcessedCart> AddCouponAsync(CouponParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CouponCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CouponCode)), nameof(param)); }

            var request = new AddCouponRequest
            {
                CartName = param.CartName,
                CouponCode = param.CouponCode,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope
            };

            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);
            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Removes the specified coupons for a specific cart instance.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task RemoveCouponsAsync(RemoveCouponsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.CouponCodes == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CouponCodes)), nameof(param)); }

            foreach (var couponCode in param.CouponCodes)
            {
                var request = new RemoveCouponRequest
                {
                    CouponCode = couponCode,
                    CartName = param.CartName,
                    CartType = param.CartType,
                    CustomerId = param.CustomerId,
                    ScopeId = param.Scope
                };

                // Do not remove coupons in parallel, because otherwise it could corrupt your cart.
                await OvertureClient.SendAsync(request).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Update the Cart with new information
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<ProcessedCart> UpdateCartAsync(UpdateCartParam param)
        {
            var request = new UpdateCartRequest
            {
                CultureName = param.CultureInfo?.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                CartName = param.CartName,
                BillingCurrency = param.BillingCurrency,
                CartType = param.CartType,
                Coupons = param.Coupons,
                Customer = param.Customer,
                OrderLocation = param.OrderLocation,
                PropertyBag = param.PropertyBag,
                Shipments = param.Shipments,
                Status = param.Status,
                Payments = param.Payments
            };

            var cacheKey = BuildCartCacheKey(param.Scope, param.CustomerId, param.CartName);
            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Builds the cache key for a Cart.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="customerId"></param>
        /// <param name="cartName"></param>
        /// <returns></returns>
        protected virtual CacheKey BuildCartCacheKey(string scope, Guid customerId, string cartName)
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.Cart)
            {
                Scope = scope
            };

            key.AppendKeyParts(customerId, cartName);
            return key;
        }

        /// <summary>
        /// Update a shipment of a cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<ProcessedCart> UpdateShipmentAsync(UpdateShipmentParam param)
        {
            var request = new UpdateShipmentRequest
            {
                CartName = param.CartName,
                ScopeId = param.ScopeId,
                CustomerId = param.CustomerId,
                Id = param.Id,
                CultureName = param.CultureInfo.Name,
                PickUpLocationId = param.PickUpLocationId,
                FulfillmentLocationId = param.FulfillmentLocationId,
                FulfillmentMethodName = param.FulfillmentMethodName,
                FulfillmentScheduleMode = param.FulfillmentScheduleMode,
                FulfillmentScheduledTimeBeginDate = param.FulfillmentScheduledTimeBeginDate,
                FulfillmentScheduledTimeEndDate = param.FulfillmentScheduledTimeEndDate,
                PropertyBag = param.PropertyBag,
                ShippingAddress = param.ShippingAddress,
                ShippingProviderId = param.ShippingProviderId
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<Overture.ServiceModel.Orders.Order> CompleteCheckoutAsync(CompleteCheckoutParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var request = new CompleteCheckoutRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope
            };

            var cacheKey = OrderRepository.CustomerOrderedProductsCacheKey(ScopeProvider.DefaultScope, param.CustomerId);
            CacheProvider.Remove(cacheKey);
            return OvertureClient.SendAsync(request);
        }

        public virtual async Task<List<ProcessedCart>> GetRecurringCartsAsync(GetRecurringOrderCartsViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var request = new GetCartsByCustomerIdRequest()
            {
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                CartType = CartConfiguration.RecurringOrderCartType
            };

            var cartSummaries = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            var resultTasks = cartSummaries.Select(cart =>
            {
                var getCartParam = new GetCartParam
                {
                    Scope = param.Scope,
                    CultureInfo = param.CultureInfo,
                    CustomerId = param.CustomerId,
                    CartName = cart.Name,
                    BaseUrl = param.BaseUrl,
                    ExecuteWorkflow = true
                };
                return GetCartAsync(getCartParam);
            });

            var carts = await Task.WhenAll(resultTasks);

            return carts.Where(i => i != null).ToList();
        }

        public virtual async Task<ListOfRecurringOrderLineItems> RescheduleRecurringCartAsync(RescheduleRecurringCartParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var request = new RescheduleRecurringCartRequest()
            {
                CustomerId = param.CustomerId,
                NextOccurence = param.NextOccurence,
                ScopeId = param.Scope,
                CartName = param.CartName
            };

            return await OvertureClient.SendAsync(request).ConfigureAwait(false);
        }
        public virtual async Task<HttpWebResponse> RemoveRecurringCartLineItemAsync(RemoveRecurringCartLineItemParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var request = new DeleteRecurringCartLineItemsRequest()
            {
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                LineItemIds = new List<Guid>() { param.LineItemId }
            };

            return await OvertureClient.SendAsync(request).ConfigureAwait(false);
        }
    }
}
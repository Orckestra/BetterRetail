using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Constants.General;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        protected virtual IOvertureClient OvertureClient { get; private set; }
        protected virtual IFindOrdersRequestFactory FindOrdersRequestFactory { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }
        protected virtual IComposerContext ComposerContext { get; private set; }

        public OrderRepository(
            IOvertureClient overtureClient, 
            IFindOrdersRequestFactory findOrdersRequestFactory, 
            ICacheProvider cacheProvider,
            IComposerContext composerContext)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            FindOrdersRequestFactory = findOrdersRequestFactory ?? throw new ArgumentNullException(nameof(findOrdersRequestFactory));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(findOrdersRequestFactory));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Gets the customer orders asynchronous.
        /// </summary>
        /// <param name="param">The get customer orders parameter.</param>
        /// <returns></returns>
        public virtual async Task<OrderQueryResult> GetCustomerOrdersAsync(GetCustomerOrdersParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var request = FindOrdersRequestFactory.Create(param);
            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            return result?.TotalCount > 0 ? result : null;
        }

        /// <summary>
        /// Gets an Order by number.
        /// </summary>
        /// <param name="param">The get order parameter.</param>
        /// <returns></returns>
        public virtual Task<Overture.ServiceModel.Orders.Order> GetOrderAsync(GetOrderParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber)), nameof(param)); }

            var request = new GetOrderByNumberRequest
            {
                ScopeId = param.Scope,
                OrderNumber = param.OrderNumber,
                IncludeShipment = true,
                IncludeLineItems = true,
                IncludePayment = true
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Get history items related to specified order
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<List<Overture.ServiceModel.Orders.OrderHistoryItem>> GetOrderChangesAsync(GetOrderChangesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber)), nameof(param)); }

            var request = new GetOrderHistoryRequest()
            {
                ScopeId = param.Scope,
                OrderNumber = param.OrderNumber
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Returns the notes of a specified shipment.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<List<Note>> GetShipmentNotesAsync(GetShipmentNotesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.ShipmentId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.ShipmentId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var request = new GetShipmentNotesRequest
            {
                ScopeId = param.Scope,
                ShipmentId = param.ShipmentId
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Update order with current id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<Overture.ServiceModel.Orders.Order> UpdateOrderAsync(UpdateOrderParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.OrderId))); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope))); }
            if (param.Order == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Order))); }

            var request = new SaveOrderRequest
            {
                ScopeId = param.Scope,
                OrderId = param.OrderId,
                Order = param.Order,
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual async Task<OrderSettings> GetOrderSettings(string scope)
        {
            var cacheKey = BuildOrderSettingsCacheKey(scope);
            return await CacheProvider.GetOrAddAsync(cacheKey, async () => await OvertureClient.SendAsync(new GetOrderSettingsRequest())).ConfigureAwait(false);
        }

        protected virtual CacheKey BuildOrderSettingsCacheKey(string scope)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.OrderSettings)
            {
                Scope = scope
            };

            return cacheKey;
        }

        /// <summary>
        /// Create a draft of a cart by order id
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        /// <returns>Cart draft</returns>
        public async Task<Overture.ServiceModel.Orders.Cart> CreateEditOrder(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(orderId))); }

            var order = await OvertureClient.SendAsync(new GetOrderByIdRequest
            {
                ScopeId = GlobalScopeName,
                OrderId = orderId.ToGuid(),
                IncludeShipment = true,
            }).ConfigureAwait(false);

            if (order.Cart.Shipments == null || order.Cart.Shipments.Count == 0)
                throw new InvalidOperationException("Cannot edit this order");

            var createCartDraftRequest = new CreateCartOrderDraftRequest()
            {
                CultureName = ComposerContext.CultureInfo?.Name,
                CustomerId = Guid.Parse(order.CustomerId),
                OrderId = Guid.Parse(orderId),
                ScopeId = order.ScopeId
            };

            var cart = await OvertureClient.SendAsync(createCartDraftRequest);
            return cart;
        }
    }
}

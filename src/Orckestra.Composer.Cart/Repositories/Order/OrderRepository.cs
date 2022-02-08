using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Fulfillment;
using Orckestra.Overture.ServiceModel.Validation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        protected virtual IOvertureClient OvertureClient { get; private set; }
        protected virtual IFindOrdersRequestFactory FindOrdersRequestFactory { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public OrderRepository(
            IOvertureClient overtureClient,
            IFindOrdersRequestFactory findOrdersRequestFactory,
            ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            FindOrdersRequestFactory = findOrdersRequestFactory ?? throw new ArgumentNullException(nameof(findOrdersRequestFactory));
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
        public virtual Task<List<OrderHistoryItem>> GetOrderChangesAsync(GetOrderChangesParam param)
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
            return await CacheProvider
                .GetOrAddAsync(cacheKey, async () => await OvertureClient.SendAsync(new GetOrderSettingsRequest()))
                .ConfigureAwait(false);
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
        /// Create a cart draft of some order
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        /// <returns>Cart draft</returns>
        public Task<ProcessedCart> CreateCartOrderDraft(CreateCartOrderDraftParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.OrderId == default) throw new ArgumentException(GetMessageOfEmpty(nameof(param.OrderId)));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
            if (param.CustomerId == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)));

            var request = new CreateCartOrderDraftRequest
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                OrderId = param.OrderId,
                ScopeId = param.Scope
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Change ownership of an order draft to the requested user and revert pending changes to the original order cart when required.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<ProcessedCart> ChangeOwnership(ChangeOrderDraftOwnershipParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.OrderId))); }
            if (param.CustomerId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId))); }
            if (param.Scope == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.Scope))); }

            var request = new ChangeOwnershipCartOrderDraftRequest()
            {
                ScopeId = param.Scope,
                CustomerId = param.CustomerId,
                OrderId = param.OrderId,
                RevertPendingChanges = param.RevertPendingChanges,
                CultureName = param.CultureName
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<HttpWebResponse> DeleteCartOrderDraft(DeleteCartOrderDraftParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.OrderId))); }
            if (param.CustomerId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId))); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope))); }

            var request = new DeleteCartOrderDraftRequest()
            {
                ScopeId = param.Scope,
                CustomerId = param.CustomerId,
                OrderId = param.OrderId
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Get history items related to specified order
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<OrderFulfillmentState> GetOrderFulfillmentStateAsync(GetOrderFulfillmentStateParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderId)), nameof(param)); }

            var request = new GetOrderFulfillmentStateRequest()
            {
                ScopeId = param.ScopeId,
                OrderId = Guid.Parse(param.OrderId)
            };

            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Process and convert the order draft cart into an actual order.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<Overture.ServiceModel.Orders.Order> SubmitCartOrderDraftAsync(SubmitCartOrderDraftParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.OrderId))); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope))); }

            var request = new SubmitCartOrderDraftRequest
            {
                ScopeId = param.Scope,
                OrderId = param.OrderId,
                CustomerId = param.CustomerId
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<OrderFulfillmentState> ChangeShipmentStatusAsync(ChangeShipmentStatusParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderId)), nameof(param)); }
            if (param.RequestedStatus == default) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.RequestedStatus)), nameof(param)); }

            var request = new ChangeShipmentStatusRequest()
            {
                ScopeId = param.ScopeId,
                OrderId = param.OrderId,
                ShipmentId = param.ShipmentId,
                Reason = param.Reason,
                RequestedStatus = param.RequestedStatus
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual async Task<OrderFulfillmentState> AddShipmentFulfillmentMessagesAsync(AddShipmentFulfillmentMessagesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (param.OrderId == default) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderId)), nameof(param)); }

            var request = new AddShipmentFulfillmentMessagesRequest()
            {
                ScopeId = param.ScopeId,
                OrderId = param.OrderId,
                ShipmentId = param.ShipmentId,
                ExecutionMessages = param.ExecutionMessages ?? new List<ExecutionMessage>(),
                ValidationResults = param.ValidationResults ?? new List<ValidationResult>()
            };

            return await OvertureClient.SendAsync(request);
        }
    }
}
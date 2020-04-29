using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        protected virtual IOvertureClient OvertureClient { get; private set; }
        protected virtual IFindOrdersRequestFactory FindOrdersRequestFactory { get; private set; }

        public OrderRepository(IOvertureClient overtureClient, IFindOrdersRequestFactory findOrdersRequestFactory)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            FindOrdersRequestFactory = findOrdersRequestFactory ?? throw new ArgumentNullException(nameof(findOrdersRequestFactory));
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
    }
}

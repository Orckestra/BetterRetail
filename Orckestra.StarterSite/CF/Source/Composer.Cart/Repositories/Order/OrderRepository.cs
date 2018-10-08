using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        protected virtual IOvertureClient OvertureClient { get; private set; }
        protected virtual IFindOrdersRequestFactory FindOrdersRequestFactory { get; private set; }

        public OrderRepository(IOvertureClient overtureClient, IFindOrdersRequestFactory findOrdersRequestFactory)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (findOrdersRequestFactory == null) { throw new ArgumentNullException("findOrdersRequestFactory"); }

            OvertureClient = overtureClient;
            FindOrdersRequestFactory = findOrdersRequestFactory;
        }

        /// <summary>
        /// Gets the customer orders asynchronous.
        /// </summary>
        /// <param name="param">The get customer orders parameter.</param>
        /// <returns></returns>
        public virtual async Task<OrderQueryResult> GetCustomerOrdersAsync(GetCustomerOrdersParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo"); }
            if (param.CustomerId == null) { throw new ArgumentException("param.CustomerId"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope"); }

            var request = FindOrdersRequestFactory.Create(param);
            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            return result != null && result.TotalCount > 0 ? result : null;
        }

        /// <summary>
        /// Gets an Order by number.
        /// </summary>
        /// <param name="param">The get order parameter.</param>
        /// <returns></returns>
        public virtual Task<Overture.ServiceModel.Orders.Order> GetOrderAsync(GetOrderParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope"); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException("param.OrderNumber"); }

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
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope"); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException("param.OrderNumber"); }

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
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.ShipmentId == Guid.Empty) { throw new ArgumentException("param.ShipmentId"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope"); }

            var request = new GetShipmentNotesRequest
            {
                ScopeId = param.Scope,
                ShipmentId = param.ShipmentId
            };

            return OvertureClient.SendAsync(request);
        }
    }
}

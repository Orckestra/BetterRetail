using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<OrderQueryResult> GetCustomerOrdersAsync(GetCustomerOrdersParam param);

        /// <summary>
        /// Gets an Order by number.
        /// </summary>
        /// <param name="param">The get order parameter.</param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Orders.Order> GetOrderAsync(GetOrderParam param);

        /// <summary>
        /// Get history items related to specified order
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<Overture.ServiceModel.Orders.OrderHistoryItem>> GetOrderChangesAsync(GetOrderChangesParam param);

        /// <summary>
        /// Returns the notes of a specified shipment.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<Note>> GetShipmentNotesAsync(GetShipmentNotesParam param);

        /// <summary>
        /// Update order with current id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Orders.Order> UpdateOrderAsync(UpdateOrderParam param);


        /// <summary>
        /// Get Order Settings.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<OrderSettings> GetOrderSettings(string scope);

        Task<Overture.ServiceModel.Orders.Cart> CreateEditOrder(string orderId);
        Task SaveEditedOrder(string scopeId, string orderId);
        Task CancelEditOrder(string scopeId, string orderId);
    }
}

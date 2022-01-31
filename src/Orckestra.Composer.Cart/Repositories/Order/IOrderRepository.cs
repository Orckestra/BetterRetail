using System;
using System.Collections.Generic;
using System.Net;
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
        Task<List<OrderHistoryItem>> GetOrderChangesAsync(GetOrderChangesParam param);

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

        /// <summary>
        /// Create a cart draft to edit an order
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Cart draft</returns>
        Task<ProcessedCart> CreateCartOrderDraft(CreateCartOrderDraftParam param);

        /// <summary>
        /// Change Order Draft ownership
        /// </summary>
        /// <returns></returns>
        Task<ProcessedCart> ChangeOwnership(ChangeOrderDraftOwnershipParam param);

        /// <summary>
        /// Delete a cart of order draft type.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<HttpWebResponse> DeleteCartOrderDraft(DeleteCartOrderDraftParam param);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Extensions
{
    /// <summary>
    /// Extends the <see cref="Cart" /> with methods to calculate totals. (Based on Orckestra.Overture.Extensibility.Extensions)
    /// </summary>
    public static class CartExtensions
    {
        /// <summary>
        /// Gets the active shipments.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>All shipment that are not Canceled.</returns>
        public static IEnumerable<Shipment> GetActiveShipments(this Overture.ServiceModel.Orders.Cart cart)
        {
            if (cart == null || cart.Shipments == null)
                return Enumerable.Empty<Shipment>();

            return cart.Shipments.Where(s => s != null && s.IsActive());
        }

        /// <summary>
        /// Gets the active line items.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>All line items belonging to shipments that are not Canceled.</returns>
        public static IEnumerable<LineItem> GetActiveLineItems(this Overture.ServiceModel.Orders.Cart cart)
        {
            return cart.GetActiveShipments().SelectMany(s => s.LineItems);
        }

        public static List<string> GetAllShipmentStatuses(this Overture.ServiceModel.Orders.Cart cart)
        {
            if (cart == null || cart.Shipments == null)
            {
                return new List<string>();
            }
            return cart.Shipments.Select(item => item.Status).ToList();
        }
    }
}

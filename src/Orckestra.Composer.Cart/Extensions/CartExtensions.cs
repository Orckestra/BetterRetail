using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Orckestra.Composer.Utils;
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

        public static bool IsCurrentApplicationOwner(this CartSummary cartSummary)
        {
            //For now this is an only way to identify if website user owns draft or not
            //We use possible owner names we investigated, which can be used in AuthToken, but it can be configured in app settings
            if (cartSummary == null) return false;
            if (!cartSummary.PropertyBag.TryGetValue(Constants.OrderDraft.OwnershipPropertyBagKey, out object orderDraftOwnershipUserName)) return false;
            var applicationNames = ConfigurationManager.AppSettings["ApplicationNames"] ?? string.Empty;
            return applicationNames.ToLower().Split(',').Contains(orderDraftOwnershipUserName?.ToString().ToLower());
        }

        /// <summary>
        /// Convert order id to the format, which used for the name of editing cart
        /// </summary>
        /// <param name="orderId">Contains guid</param>
        /// <returns>Expected draft cart name</returns>
        public static string GetDraftCartName(this string orderId)
        {
            return Guid.TryParse(orderId, out Guid guid) ? guid.ToString("N") : null;
        }

        /// <summary>
        /// Get cart line items
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>Expected draft cart name</returns>
        /// <returns>All line items.</returns>
        public static List<LineItem> GetLineItems(this Overture.ServiceModel.Orders.Cart cart)
        {
            return cart.Shipments?.SelectMany(s => s.LineItems).ToList();
        }
    }
}

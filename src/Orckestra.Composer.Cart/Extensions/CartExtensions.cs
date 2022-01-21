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

        public static DateTime GetOrderEditableUntilDate(this Overture.ServiceModel.Orders.Cart cart)
        {
            DateTime convertedDate = cart.GetLocalFulfillmentDate();
            return convertedDate.Subtract(new TimeSpan(24, 0, 0));
        }

        public static DateTime GetLocalFulfillmentDate(this Overture.ServiceModel.Orders.Cart cart)
        {
            var adjustedFulfillmentDate = GetLocalFulfillmentDateTime(cart);
            var convertedDate = new DateTime(adjustedFulfillmentDate.Year, adjustedFulfillmentDate.Month, adjustedFulfillmentDate.Day, 0, 0, 0);

            return convertedDate;
        }

        public static DateTime GetLocalFulfillmentDateTime(this Overture.ServiceModel.Orders.Cart cart)
        {
            DateTime? fulfillmentDate = cart.Shipments.FirstOrDefault()?.FulfillmentScheduledTimeBeginDate;
            if (fulfillmentDate == null)
                return DateTime.MinValue;

            var timeZoneId = cart.PropertyBag[CartConfiguration.TimeZoneId].ToString();
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var adjustedFulfillmentDate = TimeZoneInfo.ConvertTimeFromUtc(fulfillmentDate.Value, timeZoneInfo);

            return adjustedFulfillmentDate;
        }

        public static DateTime GetOrderReleaseDate(this Overture.ServiceModel.Orders.Cart cart)
        {
            var fulfillmentDate = cart.Shipments.FirstOrDefault()?.FulfillmentScheduledTimeBeginDate;
            if (fulfillmentDate == null)
                throw new InvalidOperationException("Fulfillment date must be specified.");

            var timeZoneId = cart.PropertyBag[CartConfiguration.TimeZoneId].ToString();
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var adjustedFulfillmentDate = TimeZoneInfo.ConvertTimeFromUtc(fulfillmentDate.Value, timeZoneInfo);

            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(adjustedFulfillmentDate.Year, adjustedFulfillmentDate.Month, adjustedFulfillmentDate.Day, 0, 0, 0), timeZoneInfo);
        }
    }
}

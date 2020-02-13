using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Extensions
{
    /// <summary>
    /// Extends the <see cref="Shipment" /> with methods to calculate totals. (Based on Orckestra.Overture.Extensibility.Extensions)
    /// </summary>
    public static class ShipmentExtensions
    {
        private const string ShipmentCanceledStatus = "Canceled";

        /// <summary>
        /// Determines whether this shipment is active.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns><c>true</c> if the shipment is not Canceled; otherwise <c>false</c>.</returns>
        public static bool IsActive(this Shipment shipment)
        {
            return shipment.Status != ShipmentCanceledStatus;
        }

        public static bool IsShippingTaxable(this Shipment shipment)
        {
            if (shipment == null)
            {
                return false;
            }
            return shipment.Taxes.Any(x => x.TaxForShipmentId.Equals(shipment.FulfillmentMethod?.ShipmentId) && x.TaxTotal.HasValue && x.TaxTotal.Value > 0);
        }
    }
}

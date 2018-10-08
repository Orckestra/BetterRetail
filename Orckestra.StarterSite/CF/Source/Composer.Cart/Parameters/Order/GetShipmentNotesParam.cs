using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetShipmentNotesParam
    {
        /// <summary>
        /// The scope of the shipment.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The overture identifier of the shipment.
        /// </summary>
        public Guid ShipmentId { get; set; }
    }
}

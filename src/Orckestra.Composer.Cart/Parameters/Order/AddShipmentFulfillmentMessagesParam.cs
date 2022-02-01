using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class AddShipmentFulfillmentMessagesParam
    {
        public string ScopeId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShipmentId { get; set; }
    }
}

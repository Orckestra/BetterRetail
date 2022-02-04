using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class ChangeShipmentStatusParam
    {
        public string ScopeId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShipmentId { get; set; }
        public string Reason { get; set; }
        public string RequestedStatus { get; set; }
    }
}

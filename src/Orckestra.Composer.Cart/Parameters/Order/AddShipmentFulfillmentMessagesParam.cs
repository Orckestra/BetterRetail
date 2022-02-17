using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Validation;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class AddShipmentFulfillmentMessagesParam
    {
        public string ScopeId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShipmentId { get; set; }
        public List<ExecutionMessage> ExecutionMessages { get; set; }
        public List<ValidationResult> ValidationResults { get; set; }
    }
}

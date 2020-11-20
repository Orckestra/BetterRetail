using System;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class FixCartParam
    {
        /// <summary>
        /// The cart to fix
        /// </summary>
        public ProcessedCart Cart { get; set; }

        public string ScopeId { get; set; }
    }
}

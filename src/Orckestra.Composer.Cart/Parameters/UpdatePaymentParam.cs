using Orckestra.Overture.ServiceModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdatePaymentParam
    {
        /// <summary>
        /// Name of the cart to update.
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// ID of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Id of the payment that will be updated.
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Scope in which the cart is.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Culture of the request.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

       public PropertyBag PropertyBag { get; set; }
    }
}

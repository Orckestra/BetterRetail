using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class InitializePaymentParam
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
        /// Id of the payment that will be initialized.
        /// </summary>
        public Guid PaymentId { get; set; }
        /// <summary>
        /// Type of the payment that will be initialized.
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Scope in which the cart is.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Culture of the request.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Additional data that may be used by the Overture Payment Provider to initialize the payment.
        /// This is optional.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Options that may be used to override default behaviors of the Overture Payment Provider.
        /// This is optional.
        /// </summary>
        public Dictionary<string, object> Options { get; set; }
    }
}

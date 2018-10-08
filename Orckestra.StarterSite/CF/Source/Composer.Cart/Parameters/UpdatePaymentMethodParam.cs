using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdatePaymentMethodParam
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

        /// <summary>
        /// ID of the selected Payment Method.
        /// </summary>
        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Name of the Overture Payment Provider supplying the PaymentMethod.
        /// </summary>
        public string PaymentProviderName { get; set; }
        /// <summary>
        /// Type of the Overture Payment supplying the PaymentMethod.
        /// </summary>
        public string PaymentType { get; set; }

        public bool IsAuthenticated { get; set; }

        public List<string> ProviderNames { get; set; }

        public UpdatePaymentMethodParam()
        {
            ProviderNames = new List<string>();
        }
    }
}

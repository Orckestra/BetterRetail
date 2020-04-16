using System;
using System.Globalization;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Parameters
{
    public class AddPaymentParam
    {
        /// <summary>
        /// Name of the Cart.
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// ID of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Scope of the cart.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Culture of the returning cart.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Amount to authorize in this payment. This parameter is optional.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Address to use as a billing address. This parameter is optional.
        /// </summary>
        public Address BillingAddress { get; set; }
    }
}

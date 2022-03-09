using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class TokenizePaymentParam
    {
        public Guid PaymentId { get; set; }
        public string Token { get; set; }

        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public string Scope { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetPaymentParam
    {
        /// <summary>
        /// Name of the cart.
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Id of the customer to which belongs the cart.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Id of the payment to void.
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Scope in which resides the cart.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Culture in which to make the query.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}

using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CouponParam
    {
        /// <summary>
        /// Scope in which the cart to add coupon to is.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Id of the customer to whom belongs the cart.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Name of the cart.
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Culture info in which the cart is to be returned.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Code of the coupon to add.
        /// </summary>
        public string CouponCode { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveCouponsParam : BaseCartParam
    {
        /// <summary>
        /// Scope in which the cart resides.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Id of the customer to whom belongs the cart.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Name of the cart.
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Coupon codes to clean.
        /// </summary>
        public List<string> CouponCodes { get; set; }

        public RemoveCouponsParam()
        {
            CouponCodes = new List<string>();
        }
    }
}

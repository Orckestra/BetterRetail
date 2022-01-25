using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveCouponsParam : BaseCartParam
    { 
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

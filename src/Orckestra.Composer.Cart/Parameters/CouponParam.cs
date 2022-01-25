namespace Orckestra.Composer.Cart.Parameters
{
    public class CouponParam : BaseCartParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Code of the coupon to add.
        /// </summary>
        public string CouponCode { get; set; }
    }
}

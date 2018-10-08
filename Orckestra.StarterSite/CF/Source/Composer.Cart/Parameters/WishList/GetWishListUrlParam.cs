using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.WishList
{
    public class GetWishListUrlParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// ReturnUrl to preserve
        /// Optional
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}

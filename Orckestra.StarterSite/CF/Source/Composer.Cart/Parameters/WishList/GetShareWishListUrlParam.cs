using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.WishList
{
    public class GetShareWishListUrlParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        public string Scope { get; set; }

        public Guid CustomerId { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
        public Guid WebsiteId { get; set; }


    }
}

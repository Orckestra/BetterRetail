using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateWishListViewModelParam
    {
        public Overture.ServiceModel.Orders.Cart WishList { get; set; }

        /// <summary>
        /// Culture Info for the ViewModel.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public Guid WebsiteId { get; set; }
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }
    }
}

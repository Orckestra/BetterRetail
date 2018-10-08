using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class WishListViewModel: BaseViewModel
    {
        /// <summary>
        /// List of line items (different products)
        /// </summary>
        public List<LineItemDetailViewModel> Items { get; set; }

        /// <summary>
        /// Total Quantity of items in Wish List
        /// </summary>
        public int? TotalQuantity { get; set; } = null;

        public string ShareUrl { get; set; }

        public string SignInUrl { get; set; }

        public string Url { get; set; }
    }
}

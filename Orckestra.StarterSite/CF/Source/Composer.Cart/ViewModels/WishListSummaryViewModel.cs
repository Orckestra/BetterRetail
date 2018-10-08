using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class WishListSummaryViewModel: BaseViewModel
    {

        /// <summary>
        /// List of line items ids (different products)
        /// </summary>
        public List<LineItemIdsViewModel> Items { get; set; }

        /// <summary>
        /// Total Quantity of items in Wish List
        /// </summary>
        public int TotalQuantity { get; set; } = 0;

        public string SignInUrl { get; set; }

        public string Url { get; set; }
    }
}

using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with WishList.
    /// </summary>
    public interface IWishListViewService
    {
        /// <summary>
        /// Add line item to wishlist
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<WishListSummaryViewModel> AddLineItemAsync(AddLineItemParam param);

        /// <summary>
        /// Remove line item from the wishlist.
        /// </summary>
        Task<WishListSummaryViewModel> RemoveLineItemAsync(RemoveLineItemParam param);

        /// <summary>
        /// Retrieve a wishlist
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The WishListViewModel</returns>
        Task<WishListViewModel> GetWishListViewModelAsync(GetCartParam param);

        /// <summary>
        /// Retrieve a lightweight wishlist
        /// </summary>
        /// <returns>The lightweight WishListViewModel</returns>
        Task<WishListSummaryViewModel> GetWishListSummaryViewModelAsync(GetCartParam param);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Repositories
{
    /// <summary>
    /// Abstraction for the repository that will be in charge of interacting with wishlist.
    /// </summary>
    public interface IWishListRepository
    {
        /// <summary>
        /// Add line item to wishlist
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param);

        /// <summary>
        /// Remove line item from the wishlist.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated wishlist details</returns>
        Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param);

        /// <summary>
        /// Retrieve a wishlist
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Processed Cart</returns>
        Task<ProcessedCart> GetWishListAsync(GetCartParam param);
    }
}
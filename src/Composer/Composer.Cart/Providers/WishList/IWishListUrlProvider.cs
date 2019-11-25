using Orckestra.Composer.Cart.Parameters.WishList;

namespace Orckestra.Composer.Cart.Providers.WishList
{
    /// <summary>
    /// Provider for WishList Url
    /// </summary>
    public interface IWishListUrlProvider
    {
        /// <summary>
        /// Get the Url of the My Wish List page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The My WishList Url</returns>
        string GetWishListUrl(GetWishListUrlParam parameters);

        /// <summary>
        /// Get the Url of the AddToWishList SignIn page.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The Sign In Urll</returns>
        string GetSignInUrl(GetWishListUrlParam parameters);

        /// <summary>
        /// Generate the public Url, which can be used to share the Wish List
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetShareUrl(GetShareWishListUrlParam parameters);
    }
}

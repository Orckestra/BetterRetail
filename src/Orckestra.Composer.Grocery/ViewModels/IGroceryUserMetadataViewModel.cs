using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryUserMetadataViewModel : IExtensionOf<UserMetadataViewModel>
    {
        /// <summary>
        ///  Url for Usuals page
        /// </summary>
        string MyUsualsUrl { get; set; }

        /// <summary>
        ///  Url for Recipe Favorites page
        /// </summary>
        string RecipeFavoritesUrl { get; set; }
    }
}

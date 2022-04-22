using Orckestra.Composer.Recipes.Parameters;
using Orckestra.Composer.Recipes.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Recipes.Services
{
    public interface IRecipesViewService
    {
        List<IngredientsListViewModel> GetIngedientsListsViewModel(Guid recipeId);
        Task<FavoritesViewModel> GetCustomerRecipeFavorites(GetCustomerRecipeFavoritesParam param);
        Task AddFavorite(UpdateFavoriteParam param);
        Task RemoveFavorite(UpdateFavoriteParam param);
        string GetSignInUrl(CultureInfo cultureInfo);
    }
}

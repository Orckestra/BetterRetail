using Orckestra.Composer.Recipes.ViewModels;
using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Recipes.Services
{
    public interface IRecipesViewService
    {
        List<IngredientsListViewModel> GetIngedientsListsViewModel(Guid recipeId);
    }
}

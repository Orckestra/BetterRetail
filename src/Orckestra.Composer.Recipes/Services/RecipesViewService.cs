using Composite.Data;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Recipes.Services
{
    public class RecipesViewService : IRecipesViewService
    {
        public RecipesViewService()
        {
        }

        public virtual List<IngredientsListViewModel> GetIngedientsListsViewModel(Guid recipeId)
        {
            var ingredientsList = DataFacade.GetData<IIngredientsList>().Where(l => l.Recipe == recipeId).OrderBy(l => l.Order).ToList();
            var listWithIngredients = new List<IngredientsListViewModel>();
            foreach (var iList in ingredientsList)
            {
                var ingredients = DataFacade.GetData<IIngredient>().Where(i => i.IngredientsList == iList.Id)
                    .OrderBy(i => i.Order)
                    .ToList();

                var isIgredientsPresent = ingredients.Count() > 0;
                if (!isIgredientsPresent) { continue; }

                var listNew = new IngredientsListViewModel()
                {
                    Id = iList.Id,
                    Title = iList.Title,
                    HideTitle = iList.HideTitle,
                    Ingredients = ingredients
                };

                listWithIngredients.Add(listNew);

            };
            return listWithIngredients;
        }
    }
}
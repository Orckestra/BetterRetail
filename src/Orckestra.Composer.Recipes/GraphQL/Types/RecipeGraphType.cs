using Composite.Data;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class RecipeGraphType : DataObjectGraphType<IRecipe>
    {
        public RecipeGraphType()
        {
            FieldList<IIngredientsList, IngredientsListGraphType>((ingredientsList, recipe) =>
                ingredientsList.Recipe == recipe.Id);
        }
    }
}

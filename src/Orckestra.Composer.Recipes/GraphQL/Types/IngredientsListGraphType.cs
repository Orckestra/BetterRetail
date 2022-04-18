using Composite.Data;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class IngredientsListGraphType : DataObjectGraphType<IIngredientsList>
    {
        public IngredientsListGraphType() : base()
        {
            FieldList<IIngredient, IngredientGraphType>((ingredient, ingredientsList) => ingredient.IngredientsList == ingredientsList.Id);
        }
    }
}

using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class IngredientsInputGraphType: DataInputObjectGraphType<IIngredientsList>
    {
        public IngredientsInputGraphType() : base ()
        {
            Field<ListGraphType<DataInputObjectGraphType<IIngredient>>>("ingredients");
        }
    }
}

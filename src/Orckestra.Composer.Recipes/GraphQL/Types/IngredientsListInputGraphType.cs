using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.GraphQL.Extensions;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class IngredientsListInputGraphType: DataInputObjectGraphType<IIngredientsList>
    {
        public IngredientsListInputGraphType() : base (list => list.Recipe)
        {
            Field<ListGraphType<IngredientInputGraphType>>(typeof(IIngredient).DataQLNameList());
        }
    }
}

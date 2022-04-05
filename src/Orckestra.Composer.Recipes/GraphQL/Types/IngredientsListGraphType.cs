using Composite.Data;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class IngredientsListGraphType : AutoRegisteringObjectGraphType<IIngredientsList>
    {
        public IngredientsListGraphType() : base()
        {
            Field<ListGraphType<IngredientGraphType>>("ingredients", resolve: context =>
            {
                using (var connection = new DataConnection(PublicationScope.Unpublished))
                {
                    return connection.Get<IIngredient>()
                        .Where(i => i.IngredientsList == context.Source.Id);
                }
            });
        }
    }
}

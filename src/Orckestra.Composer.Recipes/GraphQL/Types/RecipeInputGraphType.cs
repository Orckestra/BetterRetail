using Composite.Data;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class RecipeInputGraphType : DataInputObjectGraphType<IRecipe>
    {
        public RecipeInputGraphType()
        {
            
        }
    }
}

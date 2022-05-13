using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Recipes.DataTypes;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class IngredientInputGraphType : DataInputObjectGraphType<IIngredient>
    {
        public IngredientInputGraphType() : base(ingredient => ingredient.IngredientsList)
        {
        }
    }
}

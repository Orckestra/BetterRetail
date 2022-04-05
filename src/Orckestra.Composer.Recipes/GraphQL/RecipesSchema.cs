using GraphQL.Types;
using Orckestra.Composer.Recipes.GraphQL.Interfaces;
using System;

namespace Orckestra.Composer.Recipes.GraphQL
{
    public class RecipesSchema : Schema, IRecipeSchema
    {
        public RecipesSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = (RecipesQuery)provider.GetService(typeof(RecipesQuery));
            Mutation = (RecipesMutation)provider.GetService(typeof(RecipesMutation));
        }

    }
}

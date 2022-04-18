using Composite.Data;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.GraphQL.Extensions;
using Orckestra.Composer.Recipes.GraphQL.Types;
using System;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL
{
    internal class RecipesQuery : ObjectGraphType<object>
    {
        public RecipesQuery()
        {
            Name = "Query";

            this.Field<IRecipeMealType>(d => d.Id);
            this.Field<IDishType>(d => d.Id);
            this.Field<IRecipe, RecipeGraphType>(d => d.Id);
            this.FieldList<IRecipe, RecipeGraphType>();


            Field<IngredientsListGraphType>("IngredientsList",
                arguments: new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id", Description = "id of the ingredients List" }),
                resolve: (context) =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var id = context.GetArgument<Guid>("Id");
                        return connection.Get<IIngredientsList>()
                            .FirstOrDefault(il => il.Id == id);
                    }
                }
            );

            Field<ListGraphType<IngredientGraphType>>("ingredients",
                arguments: new QueryArguments(new QueryArgument<GuidGraphType>
                { Name = "IngredientsList", Description = "id of the ingredients List" }),
                resolve: (context) =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var id = context.GetArgument<Guid>("IngredientsList");
                        return connection.Get<IIngredient>()
                            .Where(i => i.IngredientsList == id);
                    }
                });

        }
    }
}
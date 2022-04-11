using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Linq;
using Composite.Data;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.GraphQL.Types;

namespace Orckestra.Composer.Recipes.GraphQL
{
    public class RecipesMutation : ObjectGraphType
    {
        public RecipesMutation()
        {
            Name = "Mutation";

            Field<StringGraphType>(
                "updateIngredients",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType>() { Name = "ingredientsList" },
                    new QueryArgument<ListGraphType<DataInputObjectGraphType<IIngredient>>> { Name = "ingredients" }
                ),
                resolve: context =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var ingredientsListId = context.GetArgument<Guid>("ingredientsList");
                        var newIngredients = context.GetArgument<List<IIngredient>>("ingredients");
                        var oldIngredients = connection.Get<IIngredient>()
                            .Where(d => d.IngredientsList == ingredientsListId).ToList();
                        var newIngredientsIds = new HashSet<Guid>(newIngredients.Select(d => d.Id));
                        var oldIngredientsIds = new HashSet<Guid>(oldIngredients.Select(d => d.Id));

                        var ingredientsToUpdate = oldIngredients.Where(d => newIngredientsIds.Contains(d.Id)).ToList();
                        var ingredientsToRemove = oldIngredients.Where(d => !newIngredientsIds.Contains(d.Id)).ToList();
                        var ingredientsToAdd = newIngredients.Where(d => !oldIngredientsIds.Contains(d.Id));

                        //TODO: add transaction
                        foreach (var ingredient in ingredientsToUpdate)
                        {
                            var newIngredient = newIngredients.First(d => d.Id == ingredient.Id);
                            //TODO: make generic method to map properties
                            ingredient.Keyword = newIngredient.Keyword;
                            ingredient.Order = newIngredient.Order;
                            ingredient.SKU = newIngredient.SKU;
                            ingredient.Title = newIngredient.Title;
                        }

                        connection.Update(ingredientsToUpdate);
                        connection.Add(ingredientsToAdd);
                        connection.Delete(ingredientsToRemove);

                        var ingredientsList =
                            connection.Get<IIngredientsList>().FirstOrDefault(d => d.Id == ingredientsListId);
                        var dataEntityToken = ingredientsList.GetDataEntityToken();

                        var relationshipGraph = new RelationshipGraph(dataEntityToken, RelationshipGraphSearchOption.Both);

                        var levels = relationshipGraph.Levels.Evaluate();
                        var relationshipGraphLevel = levels.ElementAt(1);

                        foreach (var entityToken in relationshipGraphLevel.AllEntities)
                        {
                            ConsoleMessageQueueFacade.Enqueue(new RefreshTreeMessageQueueItem() { EntityToken = entityToken }, string.Empty);
                        }
                    }

                    return "ok";
                });
        }

    }
}
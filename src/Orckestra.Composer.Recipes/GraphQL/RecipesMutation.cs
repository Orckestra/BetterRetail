using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Linq;
using Composite.Data;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.GraphQL.Extensions;
using Orckestra.Composer.Recipes.GraphQL.Types;

namespace Orckestra.Composer.Recipes.GraphQL
{
    public class RecipesMutation : ObjectGraphType
    {
        private class Constants
        {
            public string IngredientsLists { get; set; }
            public Guid RecipeId { get; set; }
        }

        public RecipesMutation()
        {
            Name = "Mutation";

            Field<StringGraphType>(
                "updateIngredients",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType>() { Name = typeof(IIngredientsList).DataQLName() },
                    new QueryArgument<ListGraphType<IngredientInputGraphType>> { Name = typeof(IIngredient).DataQLNameList() }
                ),
                resolve: context =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var ingredientsListId = context.GetArgument<Guid>(typeof(IIngredientsList).DataQLName());
                        var newIngredients = context.GetArgument<List<DataInput<IIngredient>>>(typeof(IIngredient).DataQLNameList());

                        UpdateItems<IIngredient>(connection, 
                            d => d.IngredientsList == ingredientsListId, 
                            d => d.IngredientsList = ingredientsListId,
                            d => d.Id, newIngredients);

                        var ingredientsList =
                            connection.Get<IIngredientsList>().FirstOrDefault(d => d.Id == ingredientsListId);
                        var dataEntityToken = ingredientsList.GetDataEntityToken();

                        var relationshipGraph =
                            new RelationshipGraph(dataEntityToken, RelationshipGraphSearchOption.Both);

                        var levels = relationshipGraph.Levels.Evaluate();
                        var relationshipGraphLevel = levels.ElementAt(1);

                        foreach (var entityToken in relationshipGraphLevel.AllEntities)
                        {
                            ConsoleMessageQueueFacade.Enqueue(
                                new RefreshTreeMessageQueueItem() { EntityToken = entityToken }, string.Empty);
                        }
                    }

                    return "ok";
                });


            Field<StringGraphType>(
                "updateRecipeIngredients",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = typeof(IRecipe).DataQLId() },
                    new QueryArgument<ListGraphType<IngredientsListInputGraphType>> { Name = typeof(IIngredientsList).DataQLNameList() }
                ),
                resolve: context =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var recipeId = context.GetArgument<Guid>(typeof(IRecipe).DataQLId());
                        var newLists = context.GetArgument<List<DataInput<IIngredientsList>>>(typeof(IIngredientsList).DataQLNameList());

                        UpdateItems<IIngredientsList>(connection, 
                            d => d.Recipe == recipeId,
                            d => { d.Recipe = recipeId; }, 
                            d => d.Id,
                            newLists, (input, data) =>
                            {
                                var ingredientListId = data.Id;

                                var ingredients = ((input.PropertyBag[typeof(IIngredient).DataQLNameList()] as IEnumerable) ?? Array.Empty<object>())
                                    .OfType<DataInput<IIngredient>>().ToList();

                                UpdateItems<IIngredient>(connection, 
                                    d => d.IngredientsList == ingredientListId, 
                                    d => d.IngredientsList = ingredientListId,
                                    d => d.Id,
                                    ingredients);

                            });


                        return "Ok";
                    }
                });
        }


        private void UpdateItems<TSourceType>(DataConnection connection,
            Expression<Func<TSourceType, bool>> sourceSelector, Action<TSourceType> targetUpdater,
            Func<TSourceType, Guid> getId,
            List<DataInput<TSourceType>> newItems,
            Action<DataInput<TSourceType>, TSourceType> afterAddOrUpdate = null) where TSourceType : class, IData
        {
            var oldItems = connection.Get<TSourceType>()
                .Where(sourceSelector).ToList();
            var newIds = new HashSet<Guid>(newItems.Select(d => d.Data).Select(getId));
            var oldIds = new HashSet<Guid>(oldItems.Select(getId));

            var itemsToUpdate = oldItems.Where(d => newIds.Contains(getId(d))).ToList();
            var itemsToRemove = oldItems.Where(d => !newIds.Contains(getId(d))).ToList();
            var itemsToAdd = newItems.Where(d => !oldIds.Contains(getId(d.Data))).ToList();

            foreach (var item in itemsToUpdate)
            {
                var inputItem = newItems.First(d => getId(d.Data) == getId(item));
                inputItem.Data.ProjectedCopyTo(item);
                targetUpdater(item);

                connection.Update(item);
                if (afterAddOrUpdate != null)
                {
                    afterAddOrUpdate(inputItem, item);
                }
            }

            foreach (var inputItem in itemsToAdd)
            {
                targetUpdater(inputItem.Data);
                var item = connection.Add(inputItem.Data);
                if (afterAddOrUpdate != null)
                {
                    afterAddOrUpdate(inputItem, item);
                }
            }

            connection.Delete(itemsToRemove);
        }

    }
}
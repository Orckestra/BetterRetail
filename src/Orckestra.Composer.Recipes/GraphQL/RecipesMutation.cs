using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.Core.Linq;
using Composite.Core.Localization;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.GraphQL.Extensions;
using Orckestra.Composer.Recipes.GraphQL.Types;

namespace Orckestra.Composer.Recipes.GraphQL
{
    public class RecipesMutation : ObjectGraphType
    {
        public RecipesMutation()
        {
            Name = "Mutation";

            GetOrAddAddRecipeReference<IDishType>(d => d.Id, d => d.Title, recipe => recipe.DishType);
            GetOrAddAddRecipeReference<ICuisineType>(d => d.Id, d => d.Title, recipe => recipe.CuisineType);
            GetOrAddAddRecipeReference<IDifficulty>(d => d.Id, d => d.Title, recipe => recipe.Difficulty);
            GetOrAddAddRecipeReference<IRecipeMealType>(d => d.Id, d => d.Title, recipe => recipe.MealType);
            GetOrAddAddRecipeReference<IDietType>(d => d.Id, d => d.Title, recipe => recipe.DietType);

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

                        UpdateItems(connection,
                            d => d.IngredientsList == ingredientsListId,
                            d => d.IngredientsList = ingredientsListId,
                            (a, b) => a.Id == b.Id, newIngredients);

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

                        UpdateItems(connection,
                            d => d.Recipe == recipeId,
                            d => { d.Recipe = recipeId; },
                            (a, b) => a.Id == b.Id,
                            newLists, (input, data) =>
                            {
                                var ingredientListId = data.Id;

                                var ingredients = ((input.PropertyBag[typeof(IIngredient).DataQLNameList()] as IEnumerable) ?? Array.Empty<object>())
                                    .OfType<DataInput<IIngredient>>().ToList();

                                UpdateItems(connection,
                                    d => d.IngredientsList == ingredientListId,
                                    d => d.IngredientsList = ingredientListId,
                                    (a, b) => a.Id == b.Id,
                                    ingredients);
                            });

                        return "Ok";
                    }
                });

            Field<StringGraphType>(
               "insertRecipe",
               arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = typeof(IPage).DataQLId() },
                    new QueryArgument<RecipeInputGraphType> { Name = typeof(IRecipe).DataQLName() },
                    new QueryArgument<ListGraphType<IngredientsListInputGraphType>> { Name = typeof(IIngredientsList).DataQLNameList() },
                    new QueryArgument<BooleanGraphType> { Name = "publish" }
               ),
               resolve: context =>
               {
                   var pageId = context.GetArgument<Guid>(typeof(IPage).DataQLId());
                   var recipe = context.GetArgument<DataInput<IRecipe>>(typeof(IRecipe).DataQLName());
                   var newLists = context.GetArgument<List<DataInput<IIngredientsList>>>(typeof(IIngredientsList).DataQLNameList());
                   var publish = context.GetArgument<Boolean>("publish");

                   var recipeData = recipe.Data;
                   recipeData.PageId = pageId;
                   recipeData.PublicationStatus = publish ? GenericPublishProcessController.Published : GenericPublishProcessController.Draft;

                   using (var connection = new DataConnection())
                   {
                       UpdateItem(connection, recipeData, d => i => d.Id == i.Id);

                       UpdateIds(newLists, d => d.Recipe == recipeData.Id, (a, b) => a.Order == b.Order, (a, b) => a.Id = b.Id);

                       UpdateItems(connection,
                           d => d.Recipe == recipeData.Id,
                           d => { d.Recipe = recipeData.Id; },
                           (a, b) => a.Id == b.Id,
                           newLists, (input, data) =>
                           {
                               var ingredientListId = data.Id;

                               var ingredients = ((input.PropertyBag[typeof(IIngredient).DataQLNameList()] as IEnumerable) ?? Array.Empty<object>())
                                   .OfType<DataInput<IIngredient>>().ToList();

                               UpdateIds(ingredients, d => d.IngredientsList == ingredientListId, (a, b) => a.Order == b.Order, (a, b) => a.Id = b.Id);

                               UpdateItems(connection,
                                   d => d.IngredientsList == ingredientListId,
                                   d => d.IngredientsList = ingredientListId,
                                   (a, b) => a.Id == b.Id,
                                   ingredients);
                           });
                   }

                   if (publish)
                   {
                       using (var connection = new DataConnection(PublicationScope.Published))
                       {
                           UpdateItem(connection, recipeData, d => i => d.Id == i.Id);
                       }
                   }

                   return "Ok";
               });
        }

        private void GetOrAddAddRecipeReference<TSourceType>(
            Expression<Func<TSourceType, Guid>> idExpression,
            Expression<Func<TSourceType, string>> titleExpression,
            Expression<Func<IRecipe, string>> stringExpression
        ) where TSourceType : class, IData
        {
            var stringGuid = stringExpression.Compile();

            GetOrAddAddRecipeReference(idExpression, titleExpression,
                recipe => stringGuid(recipe).Split(',').Select(d => new Guid(d)).ToList());
        }

        private void GetOrAddAddRecipeReference<TSourceType>(
            Expression<Func<TSourceType, Guid>> idExpression,
            Expression<Func<TSourceType, string>> titleExpression,
            Expression<Func<IRecipe, Guid?>> guidExpression
        ) where TSourceType : class, IData
        {
            var getGuid = guidExpression.Compile();


            GetOrAddAddRecipeReference(idExpression, titleExpression,
                recipe => new[] { getGuid(recipe) }.Where(d => d.HasValue).Select(d => d.Value).ToList());
        }

        private void GetOrAddAddRecipeReference<TSourceType>(
            Expression<Func<TSourceType, Guid>> idExpression,
            Expression<Func<TSourceType, string>> titleExpression,
            Expression<Func<IRecipe, List<Guid>>> referencesExpression
        )
            where TSourceType : class, IData
        {
            var getTitle = titleExpression.Compile();
            var getId = idExpression.Compile();
            var getExistingIds = referencesExpression.Compile();

            Field<ListGraphType<GuidGraphType>>(
                $"GetOrAddRecipe{typeof(TSourceType).Name}",
              arguments: new QueryArguments(
                  new QueryArgument<GuidGraphType> { Name = typeof(IRecipe).DataQLId() },
                  new QueryArgument<ListGraphType<StringGraphType>> { Name = "titles" }
                  ),
              resolve: (context) =>
              {
                  if (context.Errors.Count > 0) return null;

                  var locale = UserSettings.ActiveLocaleCultureInfo;
                  var defaultLocale = DataLocalizationFacade.DefaultLocalizationCulture;
                  var titles = context.GetArgument<List<string>>("titles");
                  var recipeId = context.GetArgument<Guid>(typeof(IRecipe).DataQLId());

                  using (var connection = new DataConnection(PublicationScope.Unpublished, locale))
                  {
                      if (locale.Equals(defaultLocale))
                      {
                          var existingItems = connection.Get<TSourceType>().Where(d => titles.Contains(getTitle(d))).ToList();
                          var existingTitles = existingItems.Select(d => getTitle(d));
                          var missingTitles = titles.Except(existingTitles).ToList();

                          foreach (var title in missingTitles)
                          {
                              var data = connection.CreateNew<TSourceType>();
                              data.SetPropertyValue(titleExpression, title);
                              data = connection.Add(data);
                              existingItems.Add(data);
                          }

                          return titles.Select(d => getId(existingItems.FirstOrDefault(ei => getTitle(ei) == d)));
                      }
                      else
                      {
                          IRecipe recipe;
                          using (var defaultLocaleConnection = new DataConnection(PublicationScope.Unpublished, defaultLocale))
                          {
                              recipe = defaultLocaleConnection.Get<IRecipe>().FirstOrDefault(d => d.Id == recipeId);
                          }
                          if (recipe == null)
                          {
                              context.Errors.Add(new ExecutionError("Recipe not found in the source culture")
                              {
                                  Code = "RecipeNotFound",
                              });
                              return null;
                          }

                          var existingIdsInDefaultLocale = getExistingIds(recipe);

                          if (existingIdsInDefaultLocale.Count != titles.Count)
                          {
                              context.Errors.Add(new ExecutionError($"Recipe '{typeof(TSourceType).DataQLName()}' reference not found  in the source culture")
                              {
                                  Code = "RecipeReferenceNotFound",
                              });
                              return null;
                          }

                          var existingIds = connection.Get<TSourceType>().Where(d => existingIdsInDefaultLocale.Contains(getId(d)))
                              .Select(d => getId(d)).ToList();

                          var missingIds = existingIdsInDefaultLocale.Except(existingIds);

                          foreach (var missingId in missingIds)
                          {
                              var index = existingIdsInDefaultLocale.IndexOf(missingId);
                              var data = connection.CreateNew<TSourceType>();
                              data.SetPropertyValue(idExpression, missingId);
                              data.SetPropertyValue(titleExpression, titles[index]);
                              connection.Add(data);
                          }

                          return existingIdsInDefaultLocale;
                      }
                  }
              });
        }


        private void UpdateItem<TSourceType>(DataConnection connection, TSourceType item, Func<TSourceType, Expression<Func<TSourceType, bool>>> findOne) where TSourceType : class, IData
        {
            var oldRecipe = connection.Get<TSourceType>().FirstOrDefault(findOne(item));
            if (oldRecipe != null)
            {
                item.ProjectedCopyTo(oldRecipe);
                connection.Update(oldRecipe);
            }
            else
            {
                connection.Add(item);
            }
        }

        private void UpdateItems<TSourceType>(DataConnection connection,
            Expression<Func<TSourceType, bool>> sourceSelector,
            Action<TSourceType> targetUpdater,
            Func<TSourceType, TSourceType, bool> equals,
            List<DataInput<TSourceType>> newItems,
            Action<DataInput<TSourceType>, TSourceType> afterAddOrUpdate = null) where TSourceType : class, IData
        {
            var oldItems = connection.Get<TSourceType>()
                .Where(sourceSelector).ToList();

            var itemsToUpdate = oldItems.Where(d => newItems.Any(x => equals(x.Data, d))).ToList();
            var itemsToRemove = oldItems.Where(d => !newItems.Any(x => equals(x.Data, d))).ToList();
            var itemsToAdd = newItems.Where(d => !oldItems.Any(x => equals(x, d.Data))).ToList();

            foreach (var item in itemsToUpdate)
            {
                var inputItem = newItems.First(d => equals(item, d.Data));
                inputItem.Data.ProjectedCopyTo(item);
                targetUpdater(item);

                connection.Update(item);
                afterAddOrUpdate?.Invoke(inputItem, item);
            }

            foreach (var inputItem in itemsToAdd)
            {
                targetUpdater(inputItem.Data);
                var item = connection.Add(inputItem.Data);
                afterAddOrUpdate?.Invoke(inputItem, item);
            }

            connection.Delete(itemsToRemove);
        }

        private void UpdateIds<TSourceType>(
            List<DataInput<TSourceType>> items,
            Func<TSourceType, bool> sourceSelector,
            Func<TSourceType, TSourceType, bool> equals,
            Action<TSourceType, TSourceType> update) where TSourceType : class, IData
        {
            using (var conn = new DataConnection(DataLocalizationFacade.DefaultLocalizationCulture))
            {
                var enItems = conn.Get<TSourceType>().Where(sourceSelector).ToList();

                items.ForEach(item =>
                {
                    var enItem = enItems.FirstOrDefault(x => equals(x, item.Data));
                    if (enItem != null)
                    {
                        update(item.Data, enItem);
                    }
                });
            }
        }

    }
}
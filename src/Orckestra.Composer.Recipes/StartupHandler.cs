using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Core.Routing.Foundation.PluginFacades;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.Search;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Composite.Core;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using GraphQL;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using Orckestra.Composer.Recipes.GraphQL;
using Orckestra.Composer.Recipes.GraphQL.Interfaces;

namespace Orckestra.Composer.Recipes
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IDataFieldProcessorProvider>(new RecipeDataFieldProcessorProvider());


            //GraphQL, NOTE: may be move to IRecipeDocumentExecuter and IRecipeDocumentWriter
            collection.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            collection.AddSingleton<IDocumentWriter>(new DocumentWriter(true, new ErrorInfoProvider(options => options.ExposeExceptionStackTrace = true)));
            collection.AddSingleton<IRecipeSchema>(provider => new RecipesSchema(new FuncServiceProvider(type => Get(provider, type))));
            collection.AddTransient<AutoRegisteringObjectGraphType<IIngredient>>();
        }

        public static object Get(IServiceProvider provider, Type serviceType)
        {
            var type = provider.GetService(serviceType);
            if (type != null)
            {
                return type;
            }

            if (!serviceType.GetTypeInfo().IsAbstract)
            {
                return CreateInstance(provider, serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        private static object CreateInstance(IServiceProvider provider, Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            var parameterTypes = ctor?.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes?.Select(d => Get(provider, d)).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }




        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {
            EnsureCreateStore();

            if (!HostingEnvironment.IsHosted) return;

            DataEventSystemFacade.SubscribeToDataBeforeUpdate<IRecipe>(SetTitleUrl, true);
            DataEventSystemFacade.SubscribeToDataBeforeAdd<IRecipe>(SetTitleUrl, true);

            DataEventSystemFacade.SubscribeToDataAfterAdd<IRecipe>(OnRecipeTranslated, true);
        }


        private static void SetTitleUrl(object sender, DataEventArgs dataEventArgs)
        {
            var entry = (IRecipe)dataEventArgs.Data;
            entry.UrlTitle = GetUrlFromTitle(entry.Title);
        }

        public static string GetUrlFromTitle(string title)
        {
            const string autoRemoveChars = @",./\?#!""@+'`´*():;$%&=¦§";
            var generated = new StringBuilder();

            foreach (char c in title.Where(c => autoRemoveChars.IndexOf(c) == -1))
            {
                generated.Append(c);
            }

            string url = generated.ToString().Trim().Replace(' ', '-');

            return UrlFormattersPluginFacade.FormatUrl(url, false);
        }

        private static void EnsureCreateStore()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dataInterface = typeof(IData);
            var types = assembly.GetExportedTypes();

            foreach (var type in types)
            {
                if (!dataInterface.IsAssignableFrom(type)) continue;
                DynamicTypeManager.EnsureCreateStore(type);
            }
        }


        public static readonly string TranslateProcessing = $"Orckestra.Composer.Recipes.TranslateProcessing";
        private static void OnRecipeTranslated(object sender, DataEventArgs dataEventArgs)
        {
            var entityToken = HttpContext.Current.Items["EntityToken"] as DataEntityToken;
            var sourceRecipe = entityToken?.Data as IRecipe;
            var translatedRecipe = dataEventArgs.Data as IRecipe;

            if (!HttpContext.Current.Items.Contains(TranslateProcessing) && sourceRecipe != null && translatedRecipe != null &&
                sourceRecipe.SourceCultureName != translatedRecipe.SourceCultureName && translatedRecipe.PublicationStatus == GenericPublishProcessController.Draft)
            {

                HttpContext.Current.Items[TranslateProcessing] = true;

                Log.LogInformation(typeof(StartupHandler).Namespace, $"Translating ingredients for recipe '{translatedRecipe.Title}'");

                var lists = TranslateChildren<IIngredientsList>(
                    sourceRecipe.SourceCultureName,
                    translatedRecipe.SourceCultureName,
                    list => list.Recipe == translatedRecipe.Id
                );

                foreach (var ingredientsList in lists)
                {
                    TranslateChildren<IIngredient>(
                        sourceRecipe.SourceCultureName,
                        translatedRecipe.SourceCultureName,
                        ingredient => ingredient.IngredientsList == ingredientsList.Id
                    );
                }

                HttpContext.Current.Items.Remove(TranslateProcessing);
            }
        }


        private static ICollection<TChild> TranslateChildren<TChild>(string sourceCulture, string targetCulture, Expression<Func<TChild, bool>> predicate) where TChild: class, IData, ILocalizedControlled
        {
            var targetChildren = new List<TChild>();
            List<TChild> sourceChildren;
            using (var connection = new DataConnection(PublicationScope.Unpublished, new CultureInfo(sourceCulture)))
            {
                sourceChildren = connection.Get<TChild>().Where(predicate).ToList();
            }

            using (var connection = new DataConnection(PublicationScope.Unpublished, new CultureInfo(targetCulture)))
            {
                foreach (var child in sourceChildren)
                {
                    var newData = DataFacade.BuildNew<TChild>();
                    child.ProjectedCopyTo(newData);
                    child.SourceCultureName = targetCulture;

                    newData = DataFacade.AddNew(newData, false, false, true);
                    targetChildren.Add(newData);
                }
            }

            return targetChildren;
        }
    }
}
using Composite.Core.Application;
using Composite.Core.Routing.Foundation.PluginFacades;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.Search;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Hosting;

namespace Orckestra.Composer.Recipes
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IDataFieldProcessorProvider>(new RecipeDataFieldProcessorProvider());
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
    }
}
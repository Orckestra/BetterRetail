using System.Web.Hosting;
using Composite.Core.Application;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.ContentSearch.DataTypes;
using Orckestra.Composer.ContentSearch.Search;

namespace Orckestra.Composer.ContentSearch
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;

            // Registered directly in C1 rather than in the Composer container: C1 search document sources
            // resolve their extensions before ComposerHost.Init(), when the Composer bridge still returns null.
            collection.AddSingleton<ISearchDocumentBuilderExtension, MediaSearchDocumentBuilderExtension>();
        }

        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {
            DynamicTypeManager.EnsureCreateStore(typeof(IContentTab));
            DynamicTypeManager.EnsureCreateStore(typeof(ISortOption));
        }
    }
}
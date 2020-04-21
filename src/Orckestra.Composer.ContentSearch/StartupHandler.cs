using System.Web.Hosting;
using Composite.Core.Application;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.ContentSearch.DataTypes;
using Orckestra.Composer.ContentSearch.Search;
using Orckestra.Composer.ContentSearch.Services;

namespace Orckestra.Composer.ContentSearch
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;

            collection.AddSingleton<ISearchDocumentBuilderExtension>(new MediaSearchDocumentBuilderExtension());
            collection.AddTransient<IContentSearchViewService, ContentSearchViewService>();
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
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
            collection.AddSingleton<ISearchDocumentBuilderExtension>(new MediaSearchDocumentBuilderExtension());
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
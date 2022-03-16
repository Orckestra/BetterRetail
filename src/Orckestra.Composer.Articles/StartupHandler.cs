using Composite.Core.Application;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.Articles.DataTypes;
using Orckestra.Composer.Articles.Search;

namespace Orckestra.Composer.Articles
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            //TODO: roback this comment when C1 CMS 6.12 is used , for now there is a bug in C1 and need to remove line below, so Recipes works. Without fix both DataFieldProcessorProvider for Articles and Recipes do not work together 
            //collection.AddSingleton<IDataFieldProcessorProvider>(new ArticlesDataFieldProcessorProvider());
        }

        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {

            DynamicTypeManager.EnsureCreateStore(typeof(IArticle));
            DynamicTypeManager.EnsureCreateStore(typeof(IArticleCategory));
        }
        

    }
}
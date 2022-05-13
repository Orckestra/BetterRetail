using System.Web.Hosting;
using Composite.Core.Application;
using Composite.Data.DynamicTypes;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.ContentSearch.DataTypes;

namespace Orckestra.Composer.ContentSearch
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;
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
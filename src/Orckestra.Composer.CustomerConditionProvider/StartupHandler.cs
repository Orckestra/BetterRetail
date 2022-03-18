using System.Web.Hosting;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.CustomerConditionProvider.Repositories;
using Orckestra.Tools.ConditionalContent.Providers;

namespace Orckestra.Composer.ConditionalContentProvider
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {
            if (!HostingEnvironment.IsHosted) return;

        }

        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;

            collection.AddTransient<IConditionProvider, CustomerConditionProvider.Providers.CustomerConditionProvider>();
            collection.AddTransient<ICustomerDefinitionsRepository, CustomerDefinitionsRepository>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.SearchQuery.Repositories;

namespace Orckestra.Composer.Sitemap
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {

        }

        public static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IMultiSitemapGenerator, MultiSitemapGenerator>();
        }
    }
}

using System;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Sitemap.EventHandlers;
using Orckestra.Composer.Sitemap.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Web.Hosting;
using Composite.Core;
using Orckestra.Composer.Sitemap.Config;

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
            if (!HostingEnvironment.IsHosted) return;

            SitemapEventRegistrator.Initialize();
        }

        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;

            var sitemapConfiguration = new C1SitemapConfiguration();
            collection.AddSingleton<IC1SitemapConfiguration>(container => sitemapConfiguration);

            var sitemapGeneratorConfig = new SitemapGeneratorConfig(
              HostingEnvironment.MapPath(sitemapConfiguration.SitemapDirectory),
              HostingEnvironment.MapPath(sitemapConfiguration.WorkingDirectory)
            );

            collection.AddSingleton<ISitemapGeneratorConfig>(container => sitemapGeneratorConfig);
            collection.TryAddSingleton<IMultiSitemapGenerator, MultiSitemapGenerator>();
            collection.AddSingleton<ISitemapGeneratorScheduler, SitemapGeneratorScheduler>();
        }
    }
}

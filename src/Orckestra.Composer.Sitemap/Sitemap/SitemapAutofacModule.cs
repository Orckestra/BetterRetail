using Autofac;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Sitemap;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Sitemap.Product;
using Composite.Core;
using Orckestra.Composer.Sitemap.Factory;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class SitemapAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var sitemapConfiguration = ServiceLocator.GetService<IC1SitemapConfiguration>();
            if (sitemapConfiguration.IsScheduleDefined)
            {
                RegisterSitemapGenerationDependencies(builder);
            }
        }

        private static void RegisterSitemapGenerationDependencies(ContainerBuilder builder)
        {
            // Register overture client + product url provider + scope provider + culture service
            builder.Register(container => ComposerOvertureClient.CreateFromConfig()).As<IComposerOvertureClient>().SingleInstance();

            builder.RegisterType<WebsiteProductUrlProvider>().As<IProductUrlProvider>();
            builder.RegisterType<ProductUrlParamFactory>().As<IProductUrlParamFactory>();
            builder.RegisterType<SiteConfiguration>().As<ISiteConfiguration>();
            builder.RegisterType<PageService>().As<IPageService>();

            builder.Register(container => ServiceLocator.GetService<ICookieAccesserSettings>()).SingleInstance();
            builder.Register(container => ServiceLocator.GetService<ICdnDamProviderSettings>()).SingleInstance();
            builder.Register(container => ServiceLocator.GetService<IC1SitemapConfiguration>()).SingleInstance();

            builder.RegisterType<ScopeProvider>().As<IScopeProvider>().SingleInstance();
            builder.RegisterType<CultureService>().As<ICultureService>().SingleInstance();

            // C1 content sitemap provider
            builder.RegisterType<C1ContentSitemapEntryProvider>().AsSelf().SingleInstance();
            builder.RegisterType<C1ContentSitemapProviderConfig>().AsSelf().SingleInstance();
            builder.RegisterType<C1ContentSitemapProvider>().As<ISitemapProvider>().SingleInstance();
            builder.RegisterType<C1ContentSitemapPageExcludeProvider>().As<IC1ContentSitemapPageExcludeProvider>().SingleInstance();
            builder.RegisterType<C1ContentSitemapDataTypesIncluder>().As<IC1ContentSitemapDataTypesIncluder>().SingleInstance();

            // Product sitemap provider             
            builder.RegisterType<ProductSitemapEntryProvider>().AsSelf().SingleInstance();
            builder.RegisterType<ProductSitemapProviderConfig>().AsSelf().SingleInstance();
            builder.RegisterType<ProductSitemapProvider>().As<ISitemapProvider>().SingleInstance();

            // Sitemap index generator
            builder.RegisterType<SitemapIndexGenerator>().As<ISitemapIndexGenerator>().SingleInstance();

            // Sitemap Generator   
            builder.Register(container => ServiceLocator.GetService<ISitemapGeneratorConfig>()).SingleInstance();
            builder.RegisterType<SitemapGenerator>().AsSelf().As<ISitemapGenerator>().SingleInstance();
        }
    }
}

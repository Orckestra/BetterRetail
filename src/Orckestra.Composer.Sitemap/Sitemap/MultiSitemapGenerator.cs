using Autofac;
using Orckestra.Composer.Services;
using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Core;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Sitemap
{

    internal class WebsiteContextWrapper : IWebsiteContext
    {
        public WebsiteContextWrapper(Guid websiteId)
        {
            WebsiteId = websiteId;
        }
        public Guid WebsiteId
        {
            get; private set;
        }
    }

    public class MultiSitemapGenerator : IMultiSitemapGenerator
    {
        public IContainer Container { get; protected set; }

        public MultiSitemapGenerator(ISiteConfiguration siteConfiguration, IC1SitemapConfiguration c1SitemapConfiguration)
        {
            SiteConfiguration = siteConfiguration;
            C1SitemapConfiguration = c1SitemapConfiguration;

            var builder = new ContainerBuilder();
            RegisterAdditionalTypes(builder);
            builder.RegisterModule(new SitemapAutofacModule());

            Container = builder.Build();
        }

        public virtual void RegisterAdditionalTypes(ContainerBuilder builder) { }

        public ISiteConfiguration SiteConfiguration { get; }

        public IC1SitemapConfiguration C1SitemapConfiguration { get; }

        public SitemapResponse GenerateSitemaps()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var sitemapGenerator = scope.Resolve<ISitemapGenerator>();

                var websites = new Dictionary<Guid, List<CultureInfo>>();
                var websitesBaseUrl = new Dictionary<Guid, string>();
                foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures.ToList())
                {
                    using (var conn = new DataConnection(PublicationScope.Published, culture))
                    {
                        var websiteIds = conn.Get<ISiteConfigurationMeta>().Select(d => d.PageId).Distinct().ToList();
                        foreach (var websiteId in websiteIds)
                        {
                            websites.AddToList(websiteId, culture);
                        }
                    }
                }

                using (var conn = new DataConnection())
                {
                    foreach (var hostname in conn.Get<IHostnameBinding>())
                    {
                        websitesBaseUrl[hostname.HomePageId] =
                            (hostname.EnforceHttps ? "https" : "http") + $"://{hostname.Hostname}";
                    }
                }

                var sitemapResponse = new SitemapResponse { SitemapList = new List<string>(), ErrorList = new List<string>() };
                foreach (var website in websites)
                {
                    if (websitesBaseUrl.TryGetValue(website.Key, out string websiteBaseUrl))
                    {
                        var sitemapUrl = $"{websiteBaseUrl}{VirtualPathUtility.ToAbsolute(C1SitemapConfiguration.SitemapDirectory, "/")}/{website.Key}";
                        sitemapGenerator.GenerateSitemaps(website.Key, websiteBaseUrl, sitemapUrl, website.Value.ToArray());
                        sitemapResponse.SitemapList.Add(websiteBaseUrl);
                    }
                    else
                    {
                        var websitePage = PageManager.GetPageById(website.Key);
                        var errorMessage = $"The {websitePage?.Title} website has no hostname configuration.";
                        Log.LogError(nameof(MultiSitemapGenerator), errorMessage);
                        sitemapResponse.ErrorList.Add(errorMessage);
                    }
                }
                return sitemapResponse;
            }
        }
    }
}


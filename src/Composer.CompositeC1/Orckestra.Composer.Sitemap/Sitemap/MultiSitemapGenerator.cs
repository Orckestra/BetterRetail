using Autofac;
using Orckestra.Composer.Providers;
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

        public MultiSitemapGenerator(ISiteConfiguration siteConfiguration)
        {
            SiteConfiguration = siteConfiguration;

            var builder = new ContainerBuilder();
            builder.RegisterModule(new SitemapAutofacModule());

            Container = builder.Build();
        }

        public ISiteConfiguration SiteConfiguration { get; }

        public void GenerateSitemaps()
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

                foreach (var website in websites)
                {
                    string websiteBaseUrl;
                    if (websitesBaseUrl.TryGetValue(website.Key, out websiteBaseUrl))
                    {
                        sitemapGenerator.GenerateSitemaps(new SitemapParams
                        {
                            Website = website.Key,
                            BaseUrl = websiteBaseUrl,
                            Scope = SiteConfiguration.GetScopeIdByPageId(website.Key)
                        }, $"{websiteBaseUrl}/{website.Key}", website.Value.ToArray());
                    }
                    else
                    {
                        Log.LogWarning(nameof(MultiSitemapGenerator),
                            $"Website {website.Key} does not contain hostname");
                    }
                }
            }
        }
    }
}


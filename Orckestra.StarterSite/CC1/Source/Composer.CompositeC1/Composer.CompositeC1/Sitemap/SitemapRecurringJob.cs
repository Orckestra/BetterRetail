using Autofac;
using Hangfire;
using Orckestra.Composer.CompositeC1.Hangfire;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public static class SitemapRecurringJob
    {
        public static void RegisterRecurringJobIfScheduleIsDefined(this HangfireHost hangfireHost)
        {
            // Only start Hangfire if a sitemap generation schedule is defined
            if (C1SitemapConfiguration.IsScheduleDefined)
            {                
                var hangfireContainer = hangfireHost.Container;
                var scopeProvider = hangfireContainer.Resolve<IScopeProvider>();
                var cultureService = hangfireContainer.Resolve<ICultureService>();
                var sitemapGenerator = hangfireContainer.Resolve<ISitemapGenerator>();
                
                var baseUrl = new Uri(C1SitemapConfiguration.BaseUrl);
                var baseSitemapUrl = new Uri(baseUrl, VirtualPathUtility.ToAbsolute(C1SitemapConfiguration.SitemapDirectory));
                var scope = scopeProvider.DefaultScope;
                var cultures = cultureService.GetAllSupportedCultures();

                // Daily
                RecurringJob.AddOrUpdate(
                    "sitemap-generation",
                    () => sitemapGenerator.GenerateSitemaps(baseUrl.ToString(), baseSitemapUrl.ToString(), scope, cultures),
                    C1SitemapConfiguration.JobCronSchedule
                );
            }
        }
    }
}

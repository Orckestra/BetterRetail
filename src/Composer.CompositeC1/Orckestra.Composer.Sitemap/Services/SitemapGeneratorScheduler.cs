using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Sitemap;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Services
{
    public class SitemapGeneratorScheduler : Scheduler, ISitemapGeneratorScheduler
    {
        private IMultiSitemapGenerator MultiSitemapGenerator { get; set; }
        private IC1SitemapConfiguration C1SitemapConfiguration { get; set; }
        public SitemapGeneratorScheduler(IMultiSitemapGenerator multiSitemapGenerator, IC1SitemapConfiguration c1SitemapConfiguration)
        {
            MultiSitemapGenerator = multiSitemapGenerator ?? throw new ArgumentException(nameof(multiSitemapGenerator));
            C1SitemapConfiguration = c1SitemapConfiguration ?? throw new ArgumentException(nameof(c1SitemapConfiguration));
        }

        public void RegenerateSitemapJob()
        {
            const string jobName = "SiteMapGeneration";
            Task.Run(() => ScheduleTask(() => { MultiSitemapGenerator.GenerateSitemaps(); }, jobName, C1SitemapConfiguration.ScheduleDelayInSeconds));
        }
    }
}

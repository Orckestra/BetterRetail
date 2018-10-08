using Orckestra.Composer.CompositeC1.Hangfire;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Orckestra.Composer.CompositeC1.Mvc
{
    public class HangfireHostApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            // Hangfire host and sitemap recurring job
            LogProvider.SetCurrentLogProvider(C1LogProvider.Instance);
            HangfireHost.Current.Init(new SitemapAutofacModule());
            HangfireHost.Current.RegisterRecurringJobIfScheduleIsDefined();
        }
    }
}

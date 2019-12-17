using Composite.Core;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Sitemap.Services;
using Orckestra.ExperienceManagement.Configuration.ServiceBus;
using Orckestra.ExperienceManagement.Configuration.Settings;
using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Sitemap.EventHandlers
{
    public class SitemapEventRegistrator
    {
        private static readonly ISitemapGeneratorScheduler SitemapGeneratorScheduler;
        private static readonly ServiceBusListener ServiceBusListener;
        private static IEnumerable<string> _dataTypesToIncludeFromConfig = C1ContentSitemapProviderConfig.DataTypesToInclude;

        static SitemapEventRegistrator()
        {
            SitemapGeneratorScheduler = ServiceLocator.GetService<ISitemapGeneratorScheduler>();

            var settings = ServiceLocator.GetService<IServiceBusSettings>();
            ServiceBusListener = new ServiceBusListener(settings, "Events");
        }

        public static void Initialize()
        {
            ServiceBusListener.Register(new CommonEventMessageProcessor()
            {
                EventName = "ProductUpdatedEvent",
                Action = SitemapGeneratorScheduler.RegenerateSitemapJob
            });
            ServiceBusListener.Start();

            DataEvents<IPage>.OnAfterAdd += new DataEventHandler(SitemapUpdateAfterPageChanged);
            DataEvents<IPage>.OnDeleted += new DataEventHandler(SitemapUpdateAfterPageChanged);

            foreach (var typeFullName in _dataTypesToIncludeFromConfig)
            {
                var type = Type.GetType(typeFullName);
                if (type == null) continue;

                DataEventSystemFacade.SubscribeToDataAfterAdd(type, SitemapUpdateAfterPageChanged, true);
                DataEventSystemFacade.SubscribeToDataDeleted(type, SitemapUpdateAfterPageChanged, true);
            }
        }

        private static void SitemapUpdateAfterPageChanged(object sender, DataEventArgs dataEventArgs)
        {
            if (dataEventArgs.Data.DataSourceId.PublicationScope != PublicationScope.Published) return;

            SitemapGeneratorScheduler.RegenerateSitemapJob();
        }
    }
}

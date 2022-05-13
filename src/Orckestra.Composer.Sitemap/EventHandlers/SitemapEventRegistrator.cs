using Composite.C1Console.Events;
using Composite.Core;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Sitemap.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration.ServiceBus;
using Orckestra.ExperienceManagement.Configuration.Settings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.EventHandlers
{
    public class SitemapEventRegistrator
    {
        private static readonly ISitemapGeneratorScheduler SitemapGeneratorScheduler;
        private static readonly ServiceBusListener ServiceBusListener;
        private static IEnumerable<string> _dataTypesToIncludeFromConfig = C1ContentSitemapProviderConfig.DataTypesToInclude;

        private const int WaitingForCancellationTimeInMs = 5000;
        private const int WaitingTimeForCancellationToFinishInMs = 5000;

        static SitemapEventRegistrator()
        {
            SitemapGeneratorScheduler = ServiceLocator.GetService<ISitemapGeneratorScheduler>();

            var settings = ServiceLocator.GetService<IServiceBusSettings>();
            ServiceBusListener = new ServiceBusListener(settings, "Events");
        }

        public static void Initialize()
        {
            SubscribeToDataEvents();

            var cts = new CancellationTokenSource();
            var task = Task.Run(() => WaitForWriteLockAndStartServiceBusListener(cts.Token));
            GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(args =>
            {
                try
                {
                    cts.Cancel();
                    task.Wait(WaitingTimeForCancellationToFinishInMs);
                }
                catch (Exception ex)
                {
                    Log.LogError(nameof(SitemapEventRegistrator), ex);
                }
            });
        }

        /// <summary>
        /// Repeatedly tries to acquire a write lock, when successful - starts a ServiceBus listener
        /// </summary>
        /// <returns></returns>
        private static async Task WaitForWriteLockAndStartServiceBusListener(CancellationToken cancellationToken)
        {
            IDisposable writeLock = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                writeLock = FileWriteLock.TryAcquire(nameof(SitemapEventRegistrator));

                if (writeLock != null) break;

                try
                {
                    await Task.Delay(WaitingForCancellationTimeInMs, cancellationToken);
                }
                catch (TaskCanceledException) {}
            }

            if (cancellationToken.IsCancellationRequested)
            {
                writeLock?.Dispose();
                return;
            }

            StartServiceBusListener();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(WaitingForCancellationTimeInMs, cancellationToken);
                }
                catch (TaskCanceledException) { }
            }

            writeLock?.Dispose();
        }

        /// <summary>
        /// Starts a ServiceBus listener, that triggers (with debounce) a sitemap regeneration on ProductUpdateEvent.
        /// </summary>
        private static void StartServiceBusListener()
        {
            ServiceBusListener.Register(new CommonEventMessageProcessor
            {
                EventName = "ProductUpdatedEvent",
                Action = SitemapGeneratorScheduler.RegenerateSitemapJob
            });
            ServiceBusListener.Start();
        }

        private static void SubscribeToDataEvents()
        {

            DataEvents<IPage>.OnAfterAdd += SitemapUpdateAfterPageChanged;
            DataEvents<IPage>.OnDeleted += SitemapUpdateAfterPageChanged;

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

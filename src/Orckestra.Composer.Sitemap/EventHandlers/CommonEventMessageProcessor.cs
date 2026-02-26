using Azure.Messaging.ServiceBus;
using Orckestra.ExperienceManagement.Configuration.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.EventHandlers
{
    public class CommonEventMessageProcessor : IMessageProcessor
    {
        public string EventName { set; get; }

        public Action Action { set; get; }

        public Task ProcessMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            if (EventName == null) throw new ArgumentException(nameof(EventName));
            if (Action == null) throw new ArgumentException(nameof(Action));

            if (message.ContentType.Contains(EventName))
            {
                Action();
            }

            return Task.CompletedTask;
        }
    }
}

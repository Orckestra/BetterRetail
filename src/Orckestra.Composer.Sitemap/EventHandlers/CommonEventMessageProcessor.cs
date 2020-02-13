using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Orckestra.ExperienceManagement.Configuration.ServiceBus;

namespace Orckestra.Composer.Sitemap.EventHandlers
{
    public class CommonEventMessageProcessor : IMessageProcessor
    {
        public string EventName { set; get; }

        public Action Action { set; get; }

        public async Task ProcessMessageAsync(BrokeredMessage message, CancellationToken cancellationToken)
        {
            if (EventName == null) throw new ArgumentException(nameof(EventName));
            if (Action == null) throw new ArgumentException(nameof(Action));

            if (message.ContentType.Contains(EventName))
            {
                Action();
            }
        }
    }
}

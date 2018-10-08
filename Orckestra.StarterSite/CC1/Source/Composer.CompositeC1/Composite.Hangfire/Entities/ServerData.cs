using System;

namespace Hangfire.CompositeC1.Entities
{
    public class ServerData
    {
        public int WorkerCount { get; set; }
        public string[] Queues { get; set; }
        public DateTime? StartedAt { get; set; }
    }
}

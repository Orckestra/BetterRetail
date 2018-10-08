using System;

namespace Hangfire.CompositeC1
{
    public class CompositeC1StorageOptions
    {
        public TimeSpan JobExpirationCheckInterval { get; set; }
        public TimeSpan CountersAggregateInterval { get; set; }
        public int? DashboardJobListLimit { get; set; }

        public CompositeC1StorageOptions()
        {
            JobExpirationCheckInterval = TimeSpan.FromHours(1);
            CountersAggregateInterval = TimeSpan.FromMinutes(5);
        }
    }
}

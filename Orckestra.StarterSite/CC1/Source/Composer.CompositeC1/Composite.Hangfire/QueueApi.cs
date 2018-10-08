using System;
using System.Collections.Generic;
using System.Linq;

using Composite;

using Hangfire.CompositeC1.Entities;
using Hangfire.CompositeC1.Types;

namespace Hangfire.CompositeC1
{
    public class QueueApi
    {
        private readonly CompositeC1Storage _storage;

        public QueueApi(CompositeC1Storage storage)
        {
            Verify.ArgumentNotNull(storage, "storage");

            _storage = storage;
        }

        public IEnumerable<string> GetQueues()
        {
            using (var data = (CompositeC1Connection) _storage.GetConnection())
            {
                return data.Get<IJobQueue>().Select(j => j.Queue).Distinct();
            }
        }

        public IEnumerable<Guid> GetEnqueuedJobIds(string queue, int from, int perPage)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var queues = data.Get<IJobQueue>();

                var ids = (from q in queues
                           where q.Queue == queue
                           select q.Id).Skip(from).Take(perPage).ToList();

                return ids;
            }
        }

        public IEnumerable<Guid> GetFetchedJobIds(string queue, int from, int perPage)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var jobs = data.Get<IJob>();
                var queues = data.Get<IJobQueue>();

                var ids = (from q in queues
                           join j in jobs on q.JobId equals j.Id
                           where q.Queue == queue && q.FetchedAt.HasValue
                           select j.Id).Skip(from).Take(perPage).ToList();

                return ids;
            }
        }

        public EnqueuedAndFetchedCountDto GetEnqueuedAndFetchedCount(string queue)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var fetchedCount = data.Get<IJobQueue>().Count(q => q.Queue == queue && q.FetchedAt.HasValue);
                var enqueuedCount = data.Get<IJobQueue>().Count(q => q.Queue == queue && !q.FetchedAt.HasValue);

                return new EnqueuedAndFetchedCountDto
                {
                    EnqueuedCount = enqueuedCount,
                    FetchedCount = fetchedCount
                };
            }
        }
    }
}

using System;
using System.Linq;

using Composite;

using Hangfire.CompositeC1.Types;
using Hangfire.Storage;

namespace Hangfire.CompositeC1
{
    public class CompositeC1FetchedJob : IFetchedJob
    {
        private readonly CompositeC1Connection _connection;

        private bool _disposed;
        private bool _removedFromQueue;
        private bool _requeued;

        public Guid Id { get; private  set; }
        public string JobId { get; private set; }

        public CompositeC1FetchedJob(CompositeC1Connection connection, IJobQueue queue)
        {
            Verify.ArgumentNotNull(connection, "connection");
            Verify.ArgumentNotNull(queue, "queue");

            _connection = connection;
            Id = queue.Id;
            JobId = queue.JobId.ToString();
        }

        public void RemoveFromQueue()
        {
            var queue = _connection.Get<IJobQueue>().SingleOrDefault(q => q.Id == Id);
            if (queue != null)
            {
                _connection.Delete(queue);
            }

            _removedFromQueue = true;
        }

        public void Requeue()
        {
            var queue = _connection.Get<IJobQueue>().SingleOrDefault(q => q.Id == Guid.Parse(JobId));
            if (queue != null)
            {
                queue.FetchedAt = null;

                _connection.Update(queue);
            }

            _requeued = true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (!_removedFromQueue && !_requeued)
            {
                Requeue();
            }

            _disposed = true;
        }
    }
}

using System;
using System.Linq;
using System.Threading;

using Composite;
using Composite.Core.Threading;
using Composite.Data;

using Hangfire.CompositeC1.Types;
using Hangfire.Logging;
using Hangfire.Server;

namespace Hangfire.CompositeC1
{
    public class ExpirationManager : IServerComponent
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private static readonly TimeSpan DefaultLockTimeout = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan DelayBetweenPasses = TimeSpan.FromSeconds(1);
        private const int NumberOfRecordsInSinglePass = 1000;

        private static readonly Type[] Types =
        {
            typeof(IAggregatedCounter),
            typeof(IJob),
            typeof(IList),
            typeof(ISet),
            typeof(IHash)
        };

        private readonly CompositeC1Storage _storage;
        private readonly TimeSpan _checkInterval;

        public ExpirationManager(CompositeC1Storage storage) : this(storage, TimeSpan.FromHours(1)) { }

        public ExpirationManager(CompositeC1Storage storage, TimeSpan checkInterval)
        {
            Verify.ArgumentNotNull(storage, "storage");

            _storage = storage;
            _checkInterval = checkInterval;
        }

        public void Execute(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            using (ThreadDataManager.EnsureInitialize())
            {
                var connection = (CompositeC1Connection)_storage.GetConnection();

                foreach (var t in Types)
                {
                    Logger.DebugFormat("Removing outdated records from type '{0}'", t.Name);

                    int removedCount;

                    do
                    {
                        using (connection.AcquireDistributedLock("locks:expirationmanager", DefaultLockTimeout))
                        {
                            var table = connection.Get<IExpirable>(t);
                            var data = (from d in table
                                        where d.ExpireAt < now
                                        orderby d.ExpireAt
                                        select d).Take(NumberOfRecordsInSinglePass).ToList();

                            removedCount = data.Count;

                            if (removedCount <= 0)
                            {
                                continue;
                            }

                            Logger.TraceFormat("Removed '{0}' outdated records from type '{1}'", removedCount, t.Name);

                            connection.Delete<IData>(data);

                            cancellationToken.WaitHandle.WaitOne(DelayBetweenPasses);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    } while (removedCount != 0);
                }
            }

            cancellationToken.WaitHandle.WaitOne(_checkInterval);
        }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }
}

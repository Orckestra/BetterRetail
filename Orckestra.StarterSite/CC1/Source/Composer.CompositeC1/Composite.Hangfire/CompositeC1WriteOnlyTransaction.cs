using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Composite;

using Hangfire.Common;
using Hangfire.CompositeC1.Types;
using Hangfire.Storage;

using IHangfireState = Hangfire.States.IState;

namespace Hangfire.CompositeC1
{
    public class CompositeC1WriteOnlyTransaction : IWriteOnlyTransaction
    {
        private readonly IList<Action<CompositeC1Connection>> _queue = new List<Action<CompositeC1Connection>>();

        private readonly CompositeC1Connection _connection;

        public CompositeC1WriteOnlyTransaction(CompositeC1Connection connection)
        {
            _connection = connection;
        }

        public void AddJobState(string jobId, IHangfireState state)
        {
            QueueCommand(data =>
            {
                var stateData = data.CreateNew<IState>();

                stateData.Id = Guid.NewGuid();
                stateData.JobId = Guid.Parse(jobId);
                stateData.Name = state.Name;
                stateData.Reason = state.Reason;
                stateData.CreatedAt = DateTime.UtcNow;
                stateData.Data = JobHelper.ToJson(state.SerializeData());

                data.Add(stateData);
            });
        }

        public void AddToQueue(string queue, string jobId)
        {
            QueueCommand(data =>
            {
                var queueData = data.CreateNew<IJobQueue>();

                queueData.Id = Guid.NewGuid();
                queueData.Queue = queue;
                queueData.AddedAt = DateTime.UtcNow;
                queueData.JobId = Guid.Parse(jobId);

                data.Add(queueData);
            });
        }

        public void AddToSet(string key, string value)
        {
            AddToSet(key, value, 0.0);
        }

        public void AddToSet(string key, string value, double score)
        {
            QueueCommand(data =>
            {
                var add = false;

                var set = data.Get<ISet>().SingleOrDefault(s => s.Key == key && s.Value == value);
                if (set == null)
                {
                    add = true;

                    set = data.CreateNew<ISet>();

                    set.Id = Guid.NewGuid();
                    set.Key = key;
                    set.Value = value;
                }

                set.Score = (long)score;

                data.AddOrUpdate(add, set);
            });
        }

        public void ExpireJob(string jobId, TimeSpan expireIn)
        {
            QueueCommand(data =>
            {
                var job = data.Get<IJob>().SingleOrDefault(j => j.Id == Guid.Parse(jobId));
                if (job != null)
                {
                    job.ExpireAt = DateTime.UtcNow.Add(expireIn);

                    data.Update(job);
                }
            });
        }

        public void IncrementCounter(string key)
        {
            QueueCommand(data =>
            {
                var add = false;

                var counter = data.Get<ICounter>().Where(c => c.Key == key).OrderByDescending(c => c.Value).FirstOrDefault();
                if (counter != null)
                {
                    counter.Value = counter.Value + 1;
                }
                else
                {
                    counter = data.CreateNew<ICounter>();

                    counter.Id = Guid.NewGuid();
                    counter.Key = key;
                    counter.Value = 1;

                    add = true;
                }

                data.AddOrUpdate(add, counter);
            });
        }

        public void IncrementCounter(string key, TimeSpan expireIn)
        {
            QueueCommand(data =>
            {
                var add = false;

                var counter = data.Get<ICounter>().Where(c => c.Key == key).OrderByDescending(c => c.Value).FirstOrDefault();
                if (counter != null)
                {
                    counter.Value = counter.Value + 1;
                }
                else
                {
                    counter = data.CreateNew<ICounter>();

                    counter.Id = Guid.NewGuid();
                    counter.Key = key;
                    counter.Value = 1;

                    add = true;
                }

                counter.ExpireAt = DateTime.UtcNow.Add(expireIn);

                data.AddOrUpdate(add, counter);
            });
        }

        public void DecrementCounter(string key)
        {
            QueueCommand(data =>
            {
                var add = false;

                var counter = data.Get<ICounter>().Where(c => c.Key == key).OrderByDescending(c => c.Value).FirstOrDefault();
                if (counter != null)
                {
                    counter.Value = counter.Value - 1;
                }
                else
                {
                    counter = data.CreateNew<ICounter>();

                    counter.Id = Guid.NewGuid();
                    counter.Key = key;
                    counter.Value = 0;

                    add = true;
                }

                data.AddOrUpdate(add, counter);
            });
        }

        public void DecrementCounter(string key, TimeSpan expireIn)
        {
            QueueCommand(data =>
            {
                var add = false;

                var counter = data.Get<ICounter>().Where(c => c.Key == key).OrderByDescending(c => c.Value).FirstOrDefault();
                if (counter != null)
                {
                    counter.Value = counter.Value - 1;
                }
                else
                {
                    counter = data.CreateNew<ICounter>();

                    counter.Id = Guid.NewGuid();
                    counter.Key = key;
                    counter.Value = 0;

                    add = true;
                }

                counter.ExpireAt = DateTime.UtcNow.Add(expireIn);

                data.AddOrUpdate(add, counter);
            });
        }

        public void InsertToList(string key, string value)
        {
            QueueCommand(data =>
            {
                var list = data.CreateNew<IList>();

                list.Id = Guid.NewGuid();
                list.Key = key;
                list.Value = value;

                data.Add(list);
            });
        }

        public void PersistJob(string jobId)
        {
            QueueCommand(data =>
            {
                var job = data.Get<IJob>().SingleOrDefault(j => j.Id == Guid.Parse(jobId));
                if (job != null)
                {
                    job.ExpireAt = null;

                    data.Update(job);
                }
            });
        }

        public void RemoveFromList(string key, string value)
        {
            QueueCommand(data =>
            {
                var list = data.Get<IList>().SingleOrDefault(j => j.Key == key && j.Value == value);
                if (list != null)
                {
                    data.Delete(list);
                }
            });
        }

        public void RemoveFromSet(string key, string value)
        {
            QueueCommand(data =>
            {
                var set = data.Get<ISet>().SingleOrDefault(j => j.Key == key && j.Value == value);
                if (set != null)
                {
                    data.Delete(set);
                }
            });
        }

        public void RemoveHash(string key)
        {
            Verify.ArgumentNotNull(key, "key");

            QueueCommand(data =>
            {
                var hash = data.Get<IHash>().Where(j => j.Key == key);
                if (hash.Any())
                {
                    data.Delete<IHash>(hash);
                }
            });
        }

        public void SetJobState(string jobId, IHangfireState state)
        {
            QueueCommand(data =>
            {
                var stateData = data.CreateNew<IState>();

                stateData.Id = Guid.NewGuid();
                stateData.JobId = Guid.Parse(jobId);
                stateData.Name = state.Name;
                stateData.Reason = state.Reason;
                stateData.CreatedAt = DateTime.UtcNow;
                stateData.Data = JobHelper.ToJson(state.SerializeData());

                data.Add(stateData);

                var job = data.Get<IJob>().SingleOrDefault(j => j.Id == Guid.Parse(jobId));
                if (job != null)
                {
                    job.StateId = stateData.Id;
                    job.StateName = state.Name;

                    data.Update(job);
                }
            });
        }

        public void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            Verify.ArgumentNotNull(key, "key");
            Verify.ArgumentNotNull(keyValuePairs, "keyValuePairs");

            foreach (var kvp in keyValuePairs)
            {
                var local = kvp;
                QueueCommand(data =>
                {
                    var add = false;

                    var hash = data.Get<IHash>().SingleOrDefault(h => h.Key == key && h.Field == local.Key);
                    if (hash == null)
                    {
                        add = true;

                        hash = data.CreateNew<IHash>();

                        hash.Id = Guid.NewGuid();
                        hash.Key = key;
                        hash.Field = local.Key;
                    }

                    hash.Value = local.Value;

                    data.AddOrUpdate(add, hash);
                });
            }
        }

        public void TrimList(string key, int keepStartingFrom, int keepEndingAt)
        {

        }

        public void Commit()
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var cmd in _queue)
                {
                    cmd(_connection);
                }

                _queue.Clear();
                transaction.Complete();
            }
        }

        private void QueueCommand(Action<CompositeC1Connection> command)
        {
            _queue.Add(command);
        }

        public void Dispose() { }
    }
}

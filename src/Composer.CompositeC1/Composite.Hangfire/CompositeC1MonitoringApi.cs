using System;
using System.Collections.Generic;
using System.Linq;

using Composite;

using Hangfire.Common;
using Hangfire.CompositeC1.Entities;
using Hangfire.CompositeC1.Types;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;

using IState = Hangfire.CompositeC1.Types.IState;

namespace Hangfire.CompositeC1
{
    public class CompositeC1MonitoringApi : IMonitoringApi
    {
        private readonly CompositeC1Storage _storage;
        private readonly QueueApi _queueApi;
        private readonly int? _jobListLimit;

        public CompositeC1MonitoringApi(CompositeC1Storage storage, int? jobListLimit)
        {
            _storage = storage;
            _queueApi = new QueueApi(_storage);
            _jobListLimit = jobListLimit;
        }

        public JobList<DeletedJobDto> DeletedJobs(int from, int count)
        {
            return GetJobs(
                from,
                count,
                DeletedState.StateName,
                (jsonJob, job, stateData) => new DeletedJobDto
                {
                    Job = job,
                    DeletedAt = JobHelper.DeserializeNullableDateTime(stateData["DeletedAt"])
                });
        }

        public long DeletedListCount()
        {
            return GetNumberOfJobsByStateName(DeletedState.StateName);
        }

        public long EnqueuedCount(string queue)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                return data.Get<IJobQueue>().Count(q => q.Queue == queue && !q.FetchedAt.HasValue);
            }
        }

        public JobList<EnqueuedJobDto> EnqueuedJobs(string queue, int from, int perPage)
        {
            var enqueuedJobIds = _queueApi.GetEnqueuedJobIds(queue, from, perPage);

            return EnqueuedJobs(enqueuedJobIds);
        }

        public IDictionary<DateTime, long> FailedByDatesCount()
        {
            return GetTimelineStats("failed");
        }

        public long FailedCount()
        {
            return GetNumberOfJobsByStateName(FailedState.StateName);
        }

        public JobList<FailedJobDto> FailedJobs(int from, int count)
        {
            return GetJobs(from, count,
                FailedState.StateName,
                (jsonJob, job, stateData) => new FailedJobDto
                {
                    Job = job,
                    Reason = jsonJob.StateReason,
                    ExceptionDetails = stateData["ExceptionDetails"],
                    ExceptionMessage = stateData["ExceptionMessage"],
                    ExceptionType = stateData["ExceptionType"],
                    FailedAt = JobHelper.DeserializeNullableDateTime(stateData["FailedAt"])
                });
        }

        public long FetchedCount(string queue)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                return data.Get<IJobQueue>().Count(q => q.Queue == queue && q.FetchedAt.HasValue);
            }
        }

        public JobList<FetchedJobDto> FetchedJobs(string queue, int from, int perPage)
        {
            var fetchedJobIds = _queueApi.GetFetchedJobIds(queue, from, perPage);

            return FetchedJobs(fetchedJobIds);
        }

        public StatisticsDto GetStatistics()
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var states =
                    data.Get<IJob>()
                        .Where(j => j.StateName != null)
                        .GroupBy(j => j.StateName)
                        .ToDictionary(j => j.Key, j => j.Count());

                Func<string, int> getCountIfExists = name => states.ContainsKey(name) ? states[name] : 0;

                var succeded = data.GetCombinedCounter("stats:succeeded");
                var deleted = data.GetCombinedCounter("stats:deleted");

                var recurringJobs = data.Get<ISet>().Count(c => c.Key == "recurring-jobs");
                var servers = data.Get<IServer>().Count();

                var stats = new StatisticsDto
                {
                    Enqueued = getCountIfExists(EnqueuedState.StateName),
                    Failed = getCountIfExists(FailedState.StateName),
                    Processing = getCountIfExists(ProcessingState.StateName),
                    Scheduled = getCountIfExists(ScheduledState.StateName),

                    Servers = servers,

                    Succeeded = succeded ?? 0,
                    Deleted = deleted ?? 0,
                    Recurring = recurringJobs,

                    Queues = _queueApi.GetQueues().Count()
                };

                return stats;
            }
        }

        public IDictionary<DateTime, long> HourlyFailedJobs()
        {
            return GetHourlyTimelineStats("failed");
        }

        public IDictionary<DateTime, long> HourlySucceededJobs()
        {
            return GetHourlyTimelineStats("succeeded");
        }

        public JobDetailsDto JobDetails(string jobId)
        {
            Verify.ArgumentNotNull(jobId, "jobId");

            Guid id;
            if (!Guid.TryParse(jobId, out id))
            {
                return null;
            }

            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var job = data.Get<IJob>().SingleOrDefault(j => j.Id == id);
                if (job == null)
                {
                    return null;
                }

                var jobParameters = data.Get<IJobParameter>().Where(p => p.Id == id).ToDictionary(p => p.Name, p => p.Value);

                var state = data.Get<IState>().Where(s => s.Id == id).ToList().Select(x => new StateHistoryDto
                {
                    StateName = x.Name,
                    CreatedAt = x.CreatedAt,
                    Reason = x.Reason,
                    Data = new Dictionary<string, string>(
                        JobHelper.FromJson<Dictionary<string, string>>(x.Data),
                        StringComparer.OrdinalIgnoreCase)
                }).ToList();

                return new JobDetailsDto
                {
                    CreatedAt = job.CreatedAt,
                    ExpireAt = job.ExpireAt,
                    Job = DeserializeJob(job.InvocationData, job.Arguments),
                    History = state,
                    Properties = jobParameters
                };
            }
        }

        public long ProcessingCount()
        {
            return GetNumberOfJobsByStateName(ProcessingState.StateName);
        }

        public JobList<ProcessingJobDto> ProcessingJobs(int from, int count)
        {
            return GetJobs(from, count,
                ProcessingState.StateName,
                (jsonJob, job, stateData) => new ProcessingJobDto
                {
                    Job = job,
                    ServerId = stateData.ContainsKey("ServerId") ? stateData["ServerId"] : stateData["ServerName"],
                    StartedAt = JobHelper.DeserializeDateTime(stateData["StartedAt"]),
                });
        }

        public IList<QueueWithTopEnqueuedJobsDto> Queues()
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var queuesData = data.Get<IJobQueue>().ToList();
                var queues = queuesData.GroupBy(q => q.Queue).ToDictionary(q => q.Key, q => q.Count());

                var query =
                    from kvp in queues
                    let enqueuedJobIds = _queueApi.GetEnqueuedJobIds(kvp.Key, 0, 5)
                    let counters = _queueApi.GetEnqueuedAndFetchedCount(kvp.Key)
                    select new QueueWithTopEnqueuedJobsDto
                    {
                        Name = kvp.Key,
                        Length = counters.EnqueuedCount,
                        Fetched = counters.FetchedCount,
                        FirstJobs = EnqueuedJobs(enqueuedJobIds)
                    };

                return query.ToList();
            }
        }

        public long ScheduledCount()
        {
            return GetNumberOfJobsByStateName(ScheduledState.StateName);
        }

        public JobList<ScheduledJobDto> ScheduledJobs(int from, int count)
        {
            return GetJobs(from, count,
                ScheduledState.StateName,
                (jsonJob, job, stateData) => new ScheduledJobDto
                {
                    Job = job,
                    EnqueueAt = JobHelper.DeserializeDateTime(stateData["EnqueueAt"]),
                    ScheduledAt = JobHelper.DeserializeDateTime(stateData["ScheduledAt"])
                });
        }

        public IList<ServerDto> Servers()
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var servers = data.Get<IServer>();

                var query =
                    from server in servers
                    let serverData = JobHelper.FromJson<ServerData>(server.Data)
                    select new ServerDto
                    {
                        Name = server.Id,
                        Heartbeat = server.LastHeartbeat,
                        Queues = serverData.Queues,
                        StartedAt = serverData.StartedAt.HasValue ? serverData.StartedAt.Value : DateTime.MinValue,
                        WorkersCount = serverData.WorkerCount
                    };

                return query.ToList();
            }
        }

        public IDictionary<DateTime, long> SucceededByDatesCount()
        {
            return GetTimelineStats("succeeded");
        }

        public JobList<SucceededJobDto> SucceededJobs(int from, int count)
        {
            return GetJobs(from, count,
                SucceededState.StateName,
                (jsonJob, job, stateData) => new SucceededJobDto
                {
                    Job = job,
                    Result = stateData.ContainsKey("Result") ? stateData["Result"] : null,
                    TotalDuration = stateData.ContainsKey("PerformanceDuration") && stateData.ContainsKey("Latency")
                        ? (long?)long.Parse(stateData["PerformanceDuration"]) + (long?)long.Parse(stateData["Latency"])
                        : null,
                    SucceededAt = JobHelper.DeserializeNullableDateTime(stateData["SucceededAt"])
                });
        }

        public long SucceededListCount()
        {
            return GetNumberOfJobsByStateName(SucceededState.StateName);
        }

        private Dictionary<DateTime, long> GetHourlyTimelineStats(string type)
        {
            var endDate = DateTime.UtcNow;
            var dates = new List<DateTime>();

            for (var i = 0; i < 24; i++)
            {
                dates.Add(endDate);

                endDate = endDate.AddHours(-1);
            }

            var keyMap = dates.ToDictionary(x => String.Format("stats:{0}:{1}", type, x.ToString("yyyy-MM-dd-HH")), x => x);

            return GetTimelineStats(keyMap);
        }

        public Dictionary<DateTime, long> GetTimelineStats(string type)
        {
            var endDate = DateTime.UtcNow.Date;
            var dates = new List<DateTime>();

            for (var i = 0; i < 7; i++)
            {
                dates.Add(endDate);

                endDate = endDate.AddDays(-1);
            }

            var keyMap = dates.ToDictionary(x => String.Format("stats:{0}:{1}", type, x.ToString("yyyy-MM-dd")), x => x);

            return GetTimelineStats(keyMap);
        }

        private Dictionary<DateTime, long> GetTimelineStats(IDictionary<string, DateTime> keyMap)
        {
            IDictionary<string, long> valuesMap;

            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var counters = data.Get<IAggregatedCounter>();

                valuesMap = (from c in counters
                             where keyMap.Keys.Contains(c.Key)
                             select c).ToDictionary(o => o.Key, o => o.Value);
            }

            foreach (var key in keyMap.Keys.Where(key => !valuesMap.ContainsKey(key)))
            {
                valuesMap.Add(key, 0);
            }

            return keyMap.ToDictionary(k => k.Value, k => valuesMap[k.Key]);
        }

        private JobList<FetchedJobDto> FetchedJobs(IEnumerable<Guid> jobIds)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var jobs = data.Get<IJob>();
                var states = data.Get<IState>();

                var query = (from j in jobs
                             join s in states on j.StateId equals s.Id
                             where jobIds.Contains(j.Id)
                             select new JsonJob
                             {
                                 Arguments = j.Arguments,
                                 CreatedAt = j.CreatedAt,
                                 ExpireAt = j.ExpireAt,
                                 Id = j.Id.ToString(),
                                 InvocationData = j.InvocationData,
                                 StateReason = s.Reason,
                                 StateData = s.Data,
                                 StateName = j.StateName
                             })
                    .AsEnumerable()
                    .Select(job => new KeyValuePair<string, FetchedJobDto>(job.Id, new FetchedJobDto
                    {
                        Job = DeserializeJob(job.InvocationData, job.Arguments),
                        State = job.StateName
                    }));

                return new JobList<FetchedJobDto>(query);
            }
        }

        private JobList<EnqueuedJobDto> EnqueuedJobs(IEnumerable<Guid> jobIds)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var jobs = data.Get<IJob>();
                var states = data.Get<IState>();
                var queues = data.Get<IJobQueue>();

                var query = (from j in jobs
                             join s in states on j.StateId equals s.Id
                             join q in queues on j.Id equals q.JobId
                             where jobIds.Contains(j.Id) && !q.FetchedAt.HasValue
                             select new JsonJob
                             {
                                 Arguments = j.Arguments,
                                 CreatedAt = j.CreatedAt,
                                 ExpireAt = j.ExpireAt,
                                 Id = j.Id.ToString(),
                                 InvocationData = j.InvocationData,
                                 StateReason = s.Reason,
                                 StateData = s.Data,
                                 StateName = j.StateName
                             }).ToList();

                return DeserializeJobs(query, (jsonJob, job, stateData) => new EnqueuedJobDto
                {
                    Job = job,
                    State = jsonJob.StateName,
                    EnqueuedAt = jsonJob.StateName == EnqueuedState.StateName
                        ? JobHelper.DeserializeNullableDateTime(stateData["EnqueuedAt"])
                        : null
                });
            }
        }

        private JobList<TDto> GetJobs<TDto>(
            int from,
            int count,
            string stateName,
            Func<JsonJob, Job, Dictionary<string, string>, TDto> selector)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var jobs = data.Get<IJob>().Where(j => j.StateName == stateName).OrderByDescending(j => j.CreatedAt);
                var states = data.Get<IState>();

                var query = (from job in jobs
                             join state in states on job.StateId equals state.Id
                             select new JsonJob
                             {
                                 Id = job.Id.ToString(),
                                 InvocationData = job.InvocationData,
                                 Arguments = job.Arguments,
                                 CreatedAt = job.CreatedAt,
                                 ExpireAt = job.ExpireAt,
                                 StateReason = state.Reason,
                                 StateData = state.Data
                             }
                    ).Skip(from).Take(count).ToList();

                return DeserializeJobs(query, selector);
            }
        }

        private static Job DeserializeJob(string invocationData, string arguments)
        {
            var data = JobHelper.FromJson<InvocationData>(invocationData);
            data.Arguments = arguments;

            try
            {
                return data.Deserialize();
            }
            catch (JobLoadException)
            {
                return null;
            }
        }

        private static JobList<TDto> DeserializeJobs<TDto>(
            IEnumerable<JsonJob> jobs,
            Func<JsonJob, Job, Dictionary<string, string>, TDto> selector)
        {
            var result = from job in jobs
                         let deserializedData = JobHelper.FromJson<Dictionary<string, string>>(job.StateData)
                         let stateData = deserializedData != null ?
                             new Dictionary<string, string>(deserializedData, StringComparer.OrdinalIgnoreCase)
                             : null
                         let dto = selector(job, DeserializeJob(job.InvocationData, job.Arguments), stateData)
                         select new KeyValuePair<string, TDto>(job.Id, dto);

            return new JobList<TDto>(result);
        }

        private long GetNumberOfJobsByStateName(string stateName)
        {
            using (var data = (CompositeC1Connection)_storage.GetConnection())
            {
                var query = data.Get<IJob>().Where(j => j.StateName == stateName);

                if (_jobListLimit.HasValue)
                {
                    query = query.Take(_jobListLimit.Value);
                }

                return query.Count();
            }
        }
    }
}

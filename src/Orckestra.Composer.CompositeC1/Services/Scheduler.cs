using Composite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class Scheduler : IScheduler, IDisposable
    {
        private const int HeartbeatInMs = 100;

        private struct ScheduledTask
        {
            public DateTime StartTime { get; set; }
            public Action Action { get; set; }
        }

        readonly Dictionary<string, ScheduledTask> tasks = new Dictionary<string, ScheduledTask>();
        readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private Task loopTask;

        public Scheduler()
        { 
            loopTask = new Task(() => Start(cancelTokenSource.Token), TaskCreationOptions.LongRunning);
            loopTask.Start();
        }

        public void ScheduleTask(Action action, string name, int delayInSeconds)
        {
            lock (tasks)
            {
                tasks[name] = new ScheduledTask
                {
                    Action = action,
                    StartTime = DateTime.UtcNow.AddSeconds(delayInSeconds),
                };
            }
        }

        public async void Start(CancellationToken cToken)
        {
            List<Action> actions = new List<Action>();

            while (!cToken.IsCancellationRequested)
            {
                await Task.Delay(HeartbeatInMs, cToken);

                lock (tasks)
                {
                    var now = DateTime.UtcNow;
                    foreach (var taskName in tasks.Keys.ToList())
                    {
                        var task = tasks[taskName];
                        if (now > task.StartTime)
                        {
                            actions.Add(task.Action);
                            tasks.Remove(taskName);
                        }
                    }
                }

                if (actions.Any())
                {
                    foreach (var action in actions)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            Log.LogError(nameof(Scheduler), ex);
                        }
                    }

                    actions.Clear();
                }
            }
        }

        public void Dispose()
        {
            cancelTokenSource.Cancel();
        }
    }
}
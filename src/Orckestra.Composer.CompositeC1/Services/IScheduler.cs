using System;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IScheduler
    {
        /// <summary>
        /// Schedules a task to be executed once with a delay. If a new task with the same name is scheduled, the original task will be cancelled.
        /// </summary>
        void ScheduleTask(Action action, string name, int delayInSeconds);
    }
}

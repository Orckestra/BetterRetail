using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class DailyScheduleExceptionViewModel : BaseViewModel
    {
        public string Name { get; set; }
        [Formatting("Store", "ShortDateFormat")]
        public string StartDate { get; set; }
        [Formatting("Store", "ShortDateFormat")]
        public string EndDate { get; set; }
        public ScheduleIntervalViewModel OpeningTime { get; set; }
        public bool IsClosed { get; set; }

    }
}

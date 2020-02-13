using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class DailyScheduleViewModel: BaseViewModel
    {
        public bool IsDayToday { get; set; }
        public string LocalizedDay { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOpenedAllDay { get; set; }
        public List<ScheduleIntervalViewModel> OpeningTimes { get; set; }

    }
}

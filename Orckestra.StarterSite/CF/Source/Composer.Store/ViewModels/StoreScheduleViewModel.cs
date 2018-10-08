using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreScheduleViewModel : BaseViewModel
    {
        public List<DailyScheduleViewModel> OpeningHours { get; set; }

        public List<DailyScheduleExceptionViewModel> OpeningHourExceptions { get; set; }

        public List<ScheduleIntervalViewModel> TodayOpeningTimes { get; set; }

        public bool IsOpenNow { get; set; }
    }
}

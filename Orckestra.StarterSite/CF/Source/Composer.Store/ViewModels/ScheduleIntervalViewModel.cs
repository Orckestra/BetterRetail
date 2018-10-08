using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class ScheduleIntervalViewModel : BaseViewModel
    {
        [Formatting("Store", "OpeningHourFormat")]
        public string BeginTime { get; set; }

        [Formatting("Store", "OpeningHourFormat")]
        public string EndTime { get; set; }
    }
}
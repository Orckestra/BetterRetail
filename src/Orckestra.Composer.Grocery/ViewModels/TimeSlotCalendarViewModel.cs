using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public sealed class TimeSlotCalendarViewModel : BaseViewModel
    {
        public int StartHour { get; set; }

        public int EndHour { get; set; }

        public List<string> RowLabelList { get; set; }

        public List<DayAvailabilityViewModel> DayAvailabilityList { get; set; }

    }
}
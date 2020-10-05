using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public sealed class DayAvailabilityViewModel : BaseViewModel
    {
        public DateTime Date { get; set; }
        public List<TimeSlotViewModel> SlotList { get; set; }

        public string DayOfWeekString => Date.ToString("ddd");
        public string DisplayMonth => Date.ToString("MMMM yyyy");
        public string Day => Date.Day.ToString();

        public DayAvailabilityViewModel()
        {
            SlotList = new List<TimeSlotViewModel>();
        }
    }
}
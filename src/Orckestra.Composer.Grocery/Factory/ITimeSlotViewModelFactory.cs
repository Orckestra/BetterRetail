using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Factory
{
    public interface ITimeSlotViewModelFactory
    {
        TimeSlotCalendarViewModel CreateTimeSlotCalendarViewModel(List<DayAvailability> dayAvailabilityList, CultureInfo cultureInfo);
        DayAvailabilityViewModel CreateDayAvailabilityViewModel(DayAvailability dayAvailability, CultureInfo cultureInfo, TimeSpan startTime, TimeSpan endTime);
        TimeSlotViewModel CreateTimeSlotViewModel(SlotInstance slotInstance, CultureInfo cultureInfo);
        TimeSlotReservationViewModel CreateTimeSlotReservationViewModel(TimeSlotReservation timeSlotReservation, CultureInfo cultureInfo);
        TimeSlotReservationViewModel GetTimeSlotReservationViewModel(Shipment shipment);
    }
}
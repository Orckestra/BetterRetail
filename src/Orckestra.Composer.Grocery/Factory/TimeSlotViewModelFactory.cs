using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Factory
{
    public class TimeSlotViewModelFactory : ITimeSlotViewModelFactory
    {
        public TimeSlotViewModelFactory(ILocalizationProvider localizationProvider, IViewModelMapper viewModelMapper)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
        }

        public ILocalizationProvider LocalizationProvider { get; }
        public IViewModelMapper ViewModelMapper { get; }

        public virtual TimeSlotCalendarViewModel CreateTimeSlotCalendarViewModel(List<DayAvailability> dayAvailabilityList, CultureInfo cultureInfo)
        {
            int StartTime = -1;
            int EndTime = -1;

            dayAvailabilityList.ForEach(dayAvailability =>
            {
                dayAvailability.SlotInstances.ForEach(slotInstance =>
                {
                    var startHours = slotInstance.Slot.SlotBeginTime.Hours;
                    var endHours = slotInstance.Slot.SlotEndTime.Hours;
                    if (slotInstance.Slot.SlotEndTime.Minutes > 0) endHours++;

                    if (StartTime < 0 && EndTime < 0)
                    {
                        StartTime = startHours;
                        EndTime = endHours;
                    }

                    if (startHours < StartTime) StartTime = startHours;
                    if (endHours > EndTime) EndTime = endHours;
                });
            });

            List<string> rowLabelList = new List<string>();
            for (var i = StartTime; i < EndTime; i++)
            {
                rowLabelList.Add(GetFormattedRowLabel(i, cultureInfo));
            }

            var dayAvailabilityViewModelList = dayAvailabilityList
                .Select(dayAvailability => CreateDayAvailabilityViewModel(dayAvailability, cultureInfo, TimeSpan.FromHours(StartTime), TimeSpan.FromHours(EndTime)))
                .ToList();

            return new TimeSlotCalendarViewModel
            {
                StartHour = StartTime,
                EndHour = EndTime,
                RowLabelList = rowLabelList,
                DayAvailabilityList = dayAvailabilityViewModelList
            };
        }

        protected virtual string GetFormattedRowLabel(int hour, CultureInfo cultureInfo)
        {
            var rowLabelFormat = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Grocery",
                Key = "L_TimeSlotRow",
                CultureInfo = cultureInfo
            });

            var startHour = new DateTime().AddHours(hour).ToString("hh:mm");
            var endHour = new DateTime().AddHours(hour + 1).ToString("hh:mmtt");
            return string.Format(cultureInfo, rowLabelFormat, startHour, endHour);
        }

        public virtual DayAvailabilityViewModel CreateDayAvailabilityViewModel(DayAvailability dayAvailability, CultureInfo cultureInfo, TimeSpan startTime, TimeSpan endTime)
        {
            var vm = ViewModelMapper.MapTo<DayAvailabilityViewModel>(dayAvailability, cultureInfo);

            vm.SlotList = new List<TimeSlotViewModel>();
            dayAvailability.SlotInstances.ForEach(slotInstance =>
            {
                if (startTime != slotInstance.Slot.SlotBeginTime)
                {
                    vm.SlotList.Add(CreateEmptyTimeSlotViewModel(startTime, slotInstance.Slot.SlotBeginTime));
                }

                vm.SlotList.Add(CreateTimeSlotViewModel(slotInstance, cultureInfo));
                startTime = slotInstance.Slot.SlotEndTime;
            });

            if (startTime != endTime)
            {
                vm.SlotList.Add(CreateEmptyTimeSlotViewModel(startTime, endTime));
            }

            return vm;
        }

        protected virtual TimeSlotViewModel CreateEmptyTimeSlotViewModel(TimeSpan startTime, TimeSpan endTime)
        {
            return new TimeSlotViewModel()
            {
                SlotState = SlotState.Unavailable,
                SlotBeginTime = startTime,
                SlotEndTime = endTime
            };
        }

        public virtual TimeSlotViewModel CreateTimeSlotViewModel(SlotInstance slotInstance, CultureInfo cultureInfo)
        {
            var vm = ViewModelMapper.MapTo<TimeSlotViewModel>(slotInstance.Slot, cultureInfo);

            vm.SlotState = slotInstance.State;
            vm.Hint = slotInstance.Hint;

            return vm;
        }

        public virtual TimeSlotReservationViewModel CreateTimeSlotReservationViewModel(TimeSlotReservation timeSlotReservation, CultureInfo cultureInfo)
        {
            var vm = ViewModelMapper.MapTo<TimeSlotReservationViewModel>(timeSlotReservation, cultureInfo);
            return vm;
        }

        public virtual TimeSlotReservationViewModel GetTimeSlotReservationViewModel(Shipment shipment)
        {
            if (shipment == null) { return null; }

            return new TimeSlotReservationViewModel()
            {
                Id = Guid.TryParse(shipment.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId) ? timeSlotReservationId : default,
                ReservationDate = shipment.FulfillmentScheduleReservationDate ?? default,
                FulfillmentLocationId = shipment.FulfillmentLocationId
            };
        }

    }
}
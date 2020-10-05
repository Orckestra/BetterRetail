using System;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public sealed class TimeSlotViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the current state (availability) of the slot.
        /// </summary>
        public SlotState SlotState { get; set; }

        // <summary>
        /// Gets the current state (availability) of the slot in string type.
        /// </summary>
        public string SlotStateString => SlotState.ToString();

        /// <summary>
        ///  Gets or sets an optional hint that the provider may provide to indicate why the
        /// </summary>
        public string Hint { get; set; }


        /// <summary>
        /// Gets or sets the unique identifier of the time slot.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a fulfillment location.
        /// </summary>
        public Guid FulfillmentLocationlId { get; set; }

        /// <summary>
        /// Gets or sets the fulfillment location type.
        /// </summary>
        [MapTo("Type")]
        public FulfillmentMethodType FulfillmentMethodType { get; set; }

        /// <summary>
        /// Gets the fulfillment location string type.
        /// </summary>
        public string FulfillmentMethodTypeString => FulfillmentMethodType.ToString();

        /// <summary>
        /// Gets or sets the day of the time slot.
        /// </summary>
        [MapTo("Day")]
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets the day of the time slot in string type.
        /// </summary>
        public string DayOfWeekString => DayOfWeek.ToString();

        /// <summary>
        /// Gets or sets the start time of the time slot.
        /// </summary>
        public TimeSpan SlotBeginTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the time slot.
        /// </summary>
        public TimeSpan SlotEndTime { get; set; }

        /// <summary>
        /// Gets or sets the comment of the time slot.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the quota of the time slot.
        /// </summary>
        public int? Quota { get; set; }

        public string DisplayBeginTime => new DateTime().Add(SlotBeginTime).ToString("hh:mmtt");

        public string DisplayEndTime => new DateTime().Add(SlotEndTime).ToString("hh:mmtt");

        public bool IsAvailable => SlotState == SlotState.Available;
        public bool IsFull => SlotState == SlotState.Full;
        public bool IsBlocked => !IsAvailable && !IsFull;

        public double StartDecimalTime => SlotBeginTime.TotalHours;
        public double EndDecimalTime => SlotEndTime.TotalHours;
    }
}

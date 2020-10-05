using System;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public sealed class TimeSlotReservationViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the reservation expiry warning date time
        /// </summary>
        public DateTime? ExpiryWarningDateTime { get; set; }

        /// <summary>
        /// Gets or sets the reservation expiry date time
        /// </summary>
        public DateTime? ExpiryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the reservation date
        /// </summary>
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// Gets or sets the reservation status
        /// </summary>
        public TimeslotReservationStatus ReservationStatus { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a fulfillment location.
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a fulfillment location timeslot
        /// </summary>
        public Guid FulfillmentLocationTimeSlotId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the time slot reservation
        /// </summary>
        public Guid Id { get; set; }

        public string DayOfWeekString => ReservationDate.ToString("ddd");
        public string DisplayMonth => ReservationDate.ToString("MMMM yyyy");
        public string Day => ReservationDate.Day.ToString();
    }
}

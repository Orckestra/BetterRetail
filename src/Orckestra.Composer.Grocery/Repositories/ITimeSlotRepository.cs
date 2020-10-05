using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Repositories
{
    public interface ITimeSlotRepository
    {
        /// <summary>
        /// Adds a time slot reservation for a specific time slot.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<TimeSlotReservation> AddFulfillmentLocationTimeSlotReservationAsync(AddFulfillmentLocationTimeSlotReservationParam param);

        /// <summary>
        /// Combines a schedule and slot plan to calculate the availability of slots for the specified dates.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<DayAvailabilityResult> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param);

        /// <summary>
        /// Deletes a time slot reservation
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<HttpWebResponse> DeleteFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param);

        /// <summary>
        /// Retrieves a time slot from a fulfillment location for a specific scope.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<TimeSlot> GetFulfillmentLocationTimeSlotByIdAsync(GetFulfillmentLocationTimeSlotByIdParam param);

        /// <summary>
        /// Retrieves a time slot reservation
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<TimeSlotReservation> GetFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param);

        /// <summary>
        /// Adds or renew a timeslot reservation for a cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> ReserveTimeSlotAsync(ReserveTimeSlotParam param);
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Providers
{
    /// <summary>
    /// An interface to store and retrieve selected store/fulfillment method/day and time slot for a customer.
    /// </summary>
    public interface IStoreAndFulfillmentSelectionProvider
    {
        /// <summary>
        /// Get an active store of a certain customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Store object</returns>
        Task<Orckestra.Overture.ServiceModel.Customers.Stores.Store> GetSelectedStoreAsync(GetSelectedFulfillmentParam param);

        /// <summary>
        /// Set an active store of a certain customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Orckestra.Overture.ServiceModel.Customers.Stores.Store> SetSelectedStoreAndFulfillmentMethodTypeAsync(SetSelectedFulfillmentParam param);

        /// <summary>
        /// Sets the selected times slot.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> SetSelectedTimeSlotAsync(SetSelectedTimeSlotParam param);

        /// <summary>
        /// Get time slot reservation by Id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<TimeSlotReservation> GetFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param);

        /// <summary>
        /// Gets the selected time slot for a given customer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Slot object</returns>
        Task<(TimeSlotReservation TimeSlotReservation, TimeSlot TimeSlot)?> GetSelectedTimeSlotAsync(GetSelectedTimeSlotParam param);

        /// <summary>
        /// Combines a schedule and slot plan to calculate the availability of slots for the specified dates.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<List<DayAvailability>> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param);

        /// <summary>
        /// Determines whether a customer can view the products, or should be forced to select a store or to browse in "No specified store" mode.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<bool> CustomerShouldSelectStore(GetSelectedFulfillmentParam param);

        /// <summary>
        /// Switches the session into "browsing without a specified store" mode.
        /// </summary>
        void EnableBrowsingWithoutStoreSelection();

        /// <summary>
        /// Get default fulfilled method type
        /// </summary>
        /// <returns>FulfillmentMethodType</returns>
        Task<FulfillmentMethodType> GetSelectedFulfillmentMethodTypeAsync();

        /// <summary>
        /// Update preferred store of a customer
        /// </summary>
        /// <param name="customerId">Guid of a customer</param>
        /// <param name="storeNumber">Number of a store to be set as preferred</param>
        /// <returns></returns>
        Task UpdatePreferredStoreAsync(Guid customerId, string storeNumber);

        /// <summary>
        /// Removes information about selected store and time slot from cookies.
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Removes information about selected time slot.
        /// </summary>
        void ClearTimeSlotSelection();

        /// <summary>
        /// Recover empty selection from cart after login, otherwise recover cart for current selection
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task RecoverSelection(RecoverSelectionDataParam param);

        String GetSelectedStoreNumber();
    }
}
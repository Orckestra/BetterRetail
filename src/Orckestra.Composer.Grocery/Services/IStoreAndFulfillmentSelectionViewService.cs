using System.Threading.Tasks;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Grocery.Services
{
    /// <summary>
    /// Provides functionality to set and get an active store for a certain customer
    /// </summary>
    public interface IStoreAndFulfillmentSelectionViewService
    {
        /// <summary>
        /// Returns currently selected store.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<StoreAndFulfillmentSelectionViewModel> GetSelectedFulfillmentAsync(GetSelectedFulfillmentParam param);

        /// <summary>
        /// Sets the selected store for the given customer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task SetSelectedStoreAsync(SetSelectedStoreParam param);

        /// <summary>
        /// Sets the selected fulfillment day and TimeSlot for a given customer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CartViewModel> SetSelectedTimeSlotAsync(SetSelectedTimeSlotParam param);

        /// <summary>
        /// Combines a schedule and slot plan to calculate the availability of slots for the specified dates.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<TimeSlotCalendarViewModel> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param);

        /// <summary>
        /// Set default fulfilled method type
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task SetSelectedFulfilledMethodTypeAsync(SetSelectedFulfillmentMethodTypeParam param);
    }
}
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public class StoreAndFulfillmentSelectionViewService : IStoreAndFulfillmentSelectionViewService
    {
        public StoreAndFulfillmentSelectionViewService(
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider,
            ICartService cartService,
            IStoreViewModelFactory storeViewModelFactory,
            ITimeSlotViewModelFactory timeSlotViewModelFactory)
        {
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            StoreViewModelFactory = storeViewModelFactory ?? throw new ArgumentNullException(nameof(storeViewModelFactory));
            TimeSlotViewModelFactory = timeSlotViewModelFactory ?? throw new ArgumentNullException(nameof(timeSlotViewModelFactory));
        }

        protected IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; private set; }
        protected ICartService CartService { get; private set; }
        public IStoreViewModelFactory StoreViewModelFactory { get; }
        public ITimeSlotViewModelFactory TimeSlotViewModelFactory { get; }

        public virtual async Task<StoreAndFulfillmentSelectionViewModel> GetSelectedFulfillmentAsync(GetSelectedFulfillmentParam param)
        {
            var store = await StoreAndFulfillmentSelectionProvider.GetSelectedStoreAsync(param);
            var fulfillmentMethodType = await StoreAndFulfillmentSelectionProvider.GetSelectedFulfillmentMethodTypeAsync().ConfigureAwait(false);

            if (store == null) return new StoreAndFulfillmentSelectionViewModel
            {
                FulfillmentMethodType = fulfillmentMethodType
            };

            var selectedTimeslot = await StoreAndFulfillmentSelectionProvider.GetSelectedTimeSlotAsync(new GetSelectedTimeSlotParam
            {
                StoreId = store.Id,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var storeVMParams = new CreateStoreViewModelParam
            {
                Store = store,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            };

            return new StoreAndFulfillmentSelectionViewModel
            {
                Store = StoreViewModelFactory.CreateStoreViewModel(storeVMParams),
                TimeSlotReservation = selectedTimeslot?.TimeSlotReservation != null ? TimeSlotViewModelFactory.CreateTimeSlotReservationViewModel(selectedTimeslot.Value.TimeSlotReservation, param.CultureInfo) : null,
                TimeSlot = selectedTimeslot?.TimeSlot != null ? TimeSlotViewModelFactory.CreateTimeSlotViewModel(new SlotInstance { Slot = selectedTimeslot.Value.TimeSlot }, param.CultureInfo) : null,
                FulfillmentMethodType = fulfillmentMethodType
            };
        }

        public virtual async Task<StoreAndFulfillmentSelectionViewModel> SetSelectedFulfillmentAsync(SetSelectedFulfillmentParam param)
        {
            var store = await StoreAndFulfillmentSelectionProvider.SetSelectedStoreAndFulfillmentMethodTypeAsync(param).ConfigureAwait(false);
            var storeVMParams = new CreateStoreViewModelParam
            {
                Store = store,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            };

            return new StoreAndFulfillmentSelectionViewModel
            {
                Store = StoreViewModelFactory.CreateStoreViewModel(storeVMParams),
                TimeSlotReservation = null,
                TimeSlot = null,
                FulfillmentMethodType = await StoreAndFulfillmentSelectionProvider.GetSelectedFulfillmentMethodTypeAsync().ConfigureAwait(false)
            };
        }

        public virtual async Task<CartViewModel> SetSelectedTimeSlotAsync(SetSelectedTimeSlotParam param)
        {
            var cart = await StoreAndFulfillmentSelectionProvider.SetSelectedTimeSlotAsync(param);

            var shipment = cart?.Shipments.FirstOrDefault();
            if (!Guid.TryParse(shipment?.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId)) return null;

            var getTimeSlotParam = new BaseFulfillmentLocationTimeSlotReservationParam
            {
                SlotReservationId = timeSlotReservationId,
                Scope = param.Scope,
                FulfillmentLocationId = param.FulfillmentLocationId
            };
            var timeSlot = await StoreAndFulfillmentSelectionProvider.GetFulfillmentLocationTimeSlotReservationByIdAsync(getTimeSlotParam);

            var cartVM = await CartService.CreateCartViewModelAsync(new CreateCartViewModelParam()
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            });
            var groceryCartVM = cartVM.AsExtensionModel<IGroceryCartViewModel>();
            groceryCartVM.TimeSlotReservation = TimeSlotViewModelFactory.CreateTimeSlotReservationViewModel(timeSlot, param.CultureInfo);

            return cartVM;
        }

        public virtual async Task<TimeSlotCalendarViewModel> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param)
        {
            var dayAvailabilityList = await StoreAndFulfillmentSelectionProvider.CalculateScheduleAvailabilitySlotsAsync(param);

            return TimeSlotViewModelFactory.CreateTimeSlotCalendarViewModel(dayAvailabilityList, param.CultureInfo);
        }

    }
}

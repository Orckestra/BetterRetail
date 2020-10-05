using System;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Repositories;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryOrderHistoryViewService : OrderHistoryViewService
    {
        public GroceryOrderHistoryViewService(IOrderHistoryViewModelFactory orderHistoryViewModelFactory,
            IOrderRepository orderRepository,
            IOrderUrlProvider orderUrlProvider,
            ILookupService lookupService,
            IOrderDetailsViewModelFactory orderDetailsViewModelFactory,
            IImageService imageService,
            IShippingTrackingProviderFactory shippingTrackingProviderFactory,
            ICustomerRepository customerRepository,
            ITimeSlotRepository timeSlotRepository,
            ITimeSlotViewModelFactory timeSlotViewModelFactory)
            :
            base(orderHistoryViewModelFactory,
            orderRepository,
            orderUrlProvider,
            lookupService,
            orderDetailsViewModelFactory,
            imageService,
            shippingTrackingProviderFactory,
            customerRepository)
        {
            TimeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
            TimeSlotViewModelFactory = timeSlotViewModelFactory ?? throw new ArgumentNullException(nameof(timeSlotViewModelFactory));
        }

        public ITimeSlotRepository TimeSlotRepository { get; }
        public ITimeSlotViewModelFactory TimeSlotViewModelFactory { get; }

        protected override async Task<OrderDetailViewModel> BuildOrderDetailViewModelAsync(Order order, GetOrderParam getOrderParam)
        {
            var vm = await base.BuildOrderDetailViewModelAsync(order, getOrderParam).ConfigureAwait(false);

            foreach (var shippingVm in vm.Shipments)
            {
                if (Guid.TryParse(shippingVm.FulfillmentScheduleReservationNumber, out Guid slotReservationId) && slotReservationId != default)
                {
                    var timeSlotReservation = await TimeSlotRepository.GetFulfillmentLocationTimeSlotReservationByIdAsync(new BaseFulfillmentLocationTimeSlotReservationParam
                    {
                        FulfillmentLocationId = shippingVm.FulfillmentLocationId,
                        SlotReservationId = slotReservationId,
                        Scope = getOrderParam.Scope
                    }).ConfigureAwait(false);

                    if (timeSlotReservation != null)
                    {
                        var timeslot = await TimeSlotRepository.GetFulfillmentLocationTimeSlotByIdAsync(new GetFulfillmentLocationTimeSlotByIdParam
                        {
                            FulfillmentLocationId = shippingVm.FulfillmentLocationId,
                            SlotId = timeSlotReservation.FulfillmentLocationTimeSlotId,
                            FulfillmentMethodType = shippingVm.ShippingMethod.FulfillmentMethodType,
                            Scope = getOrderParam.Scope
                        }).ConfigureAwait(false);

                        if (timeslot != null)
                        {
                            var extendedShippingVm = shippingVm.AsExtensionModel<IGroceryOrderShipmentDetailViewModel>();
                            extendedShippingVm.TimeSlot = TimeSlotViewModelFactory.CreateTimeSlotViewModel(new SlotInstance { Slot = timeslot }, getOrderParam.CultureInfo);
                            extendedShippingVm.TimeSlotReservation = TimeSlotViewModelFactory.CreateTimeSlotReservationViewModel(timeSlotReservation, getOrderParam.CultureInfo);
                        }
                    }
                }
            }
            return vm;
        }
    }
}
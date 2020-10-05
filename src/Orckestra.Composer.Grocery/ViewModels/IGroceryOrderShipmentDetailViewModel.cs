using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryOrderShipmentDetailViewModel : IExtensionOf<OrderShipmentDetailViewModel>
    {
        TimeSlotReservationViewModel TimeSlotReservation { get; set; }
        TimeSlotViewModel TimeSlot { get; set; }
    }
}
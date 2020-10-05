using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryCartViewModel :  IExtensionOf<CartViewModel>
    {
        /// <summary>
        /// Reservation info.
        /// </summary>
        TimeSlotReservationViewModel TimeSlotReservation { get; set; }
    }
}
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public class StoreAndFulfillmentSelectionViewModel
    {
        public StoreViewModel Store { get; set; }
        public TimeSlotReservationViewModel TimeSlotReservation { get; set; }
        public TimeSlotViewModel TimeSlot { get; set; }
        public FulfillmentMethodType FulfillmentMethodType { get; set; }
        public string FulfillmentMethodTypeString => FulfillmentMethodType.ToString();
    }
}

using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryStoreViewModel : IExtensionOf<StoreViewModel>
    {
        StoreScheduleViewModel PickUpSchedule { get; set; }
        StoreScheduleViewModel DeliverySchedule { get; set; }
    }
}

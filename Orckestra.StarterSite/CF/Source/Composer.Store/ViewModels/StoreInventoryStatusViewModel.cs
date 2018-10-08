
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products.Inventory;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreInventoryStatusViewModel : BaseViewModel
    {
        public string DisplayName { get; set; }
        public InventoryStatusEnum Status { get; set; }

    }
}

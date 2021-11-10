using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.Repositories;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.Grocery.Settings;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.Store.Factory;
using Orckestra.Overture;

namespace Orckestra.Composer.Grocery
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<StoreAndFulfillmentSelectionViewService, IStoreAndFulfillmentSelectionViewService>();
            host.Register<StoreAndFulfillmentSelectionProvider, IStoreAndFulfillmentSelectionProvider>();
            host.Register<GroceryOrderHistoryViewService, IOrderHistoryViewService>();
            host.Register<GroceryMembershipViewService, IMembershipViewService>();
            host.Register<GroceryInventoryLocationProvider, IInventoryLocationProvider>();
            host.Register<CartMoveProvider, ICartMoveProvider>();
            host.Register<GroceryCartViewModelFactory, ICartViewModelFactory>();
            host.Register<GroceryStoreViewModelFactory, IStoreViewModelFactory>();
            host.Register<TimeSlotViewModelFactory, ITimeSlotViewModelFactory>();
            host.Register<TimeSlotRepository, ITimeSlotRepository>();
            host.Register<GrocerySettings, IGrocerySettings>(ComponentLifestyle.PerRequest);
            host.Register<ConverterProvider, IConverterProvider>();
            host.Register<GroceryProductViewModelFactory, IProductViewModelFactory>();
            host.Register<GrocerySearchQueryViewService, ISearchQueryViewService>();
            host.Register<GrocerySearchViewService, ISearchViewService>();
            host.Register<GroceryCategoryBrowsingViewService, ICategoryBrowsingViewService>();
            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(Plugin).Assembly);
            host.RegisterApiControllers(typeof(Plugin).Assembly);
        }
    }
}
using OOrckestra.Composer.Grocery.Services;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.ExceptionFilters;
using Orckestra.Composer.Grocery.Repositories;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.Recipes;
using Orckestra.Composer.Recipes.Services;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Services;

namespace Orckestra.Composer.Grocery.Website
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            ComposerConfig.RegisterConfigurations();
            host.Register<GroceryComposerContext, IComposerContext>();
            host.Register<SalesScopeStoreLocatorViewService, IStoreLocatorViewService>();
            host.Register<SalesScopeStoreViewService, IStoreViewService>();
            host.Register<SalesScopeInventoryRepository, IInventoryRepository>();
            host.Register<SalesScopeOrderRepository, IOrderRepository>();
            host.Register<SalesScopeCustomerRepository, ICustomerRepository>();
            host.Register<SalesScopeCustomerAddressesRepository, ICustomerAddressRepository>();
            host.Register<SalesScopeWishListRepository, IWishListRepository>();
            host.Register<CookieBasedFulfillmentContext, IFulfillmentContext>();
            host.Register<SalesScopeFulfillmentMethodRepository, IFulfillmentMethodRepository>();
            host.Register<SalesScopePaymentRepository, IPaymentRepository>();
            host.Register<RecipeUrlProvider, IRecipeUrlProvider>();
            host.Register<RecipesViewService, IRecipesViewService>();
            host.RegisterControllers(GetType().Assembly);
            host.RegisterApiControllers(GetType().Assembly);
            host.RegisterExceptionFiltersForApiControllers(typeof(AggregatedComposerExceptionFilter), typeof(ComposerExceptionFilter));
            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(GetType().Assembly);
            RegisterPaymentProviders();
        }

        private void RegisterPaymentProviders()
        {
            CartConfiguration.PaymentProviderRegistry.RegisterProvider<OnSitePOSPaymentProvider>("Onsite payment");
            CartConfiguration.PaymentProviderRegistry.RegisterProvider<MonerisCanadaPaymentProvider>("Moneris");
        }
    }
}
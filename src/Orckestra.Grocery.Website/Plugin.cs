using OOrckestra.Composer.Grocery.Services;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.ExceptionFilters;
using Orckestra.Composer.Grocery.Services;
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
            host.Register<SalesScopeWishListViewService, IWishListViewService>();
            host.Register<CookieBasedFulfillmentContext, IFulfillmentContext>();
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
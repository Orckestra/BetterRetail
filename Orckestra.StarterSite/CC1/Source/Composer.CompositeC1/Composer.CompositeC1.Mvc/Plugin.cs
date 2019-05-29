using System.Web.Http;
using System.Web.Routing;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.ExceptionFilters;

namespace Orckestra.Composer.CompositeC1.Mvc
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            ComposerConfig.RegisterConfigurations();

            host.RegisterControllers(GetType().Assembly);
            host.RegisterApiControllers(GetType().Assembly);
            host.RegisterExceptionFiltersForApiControllers(typeof(AggregatedComposerExceptionFilter), typeof(ComposerExceptionFilter));
            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(GetType().Assembly);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            RegisterPaymentProviders();
        }

        private void RegisterPaymentProviders()
        {
            CartConfiguration.PaymentProviderRegistry.RegisterProvider<OnSitePOSPaymentProvider>("WebStoreOnSitePOS");
            CartConfiguration.PaymentProviderRegistry.RegisterProvider<MonerisCanadaPaymentProvider>("MonerisCanadaPaymentProvider");
        }
    }
}
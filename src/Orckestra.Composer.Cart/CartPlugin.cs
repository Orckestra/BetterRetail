using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Providers.CartMerge;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture;

namespace Orckestra.Composer.Cart
{
    public class CartPlugin : IComposerPlugin
    {
        /// <summary>
        /// Autowireup this plugin
        /// </summary>
        /// <param name="host"></param>
        public void Register(IComposerHost host)
        {
            host.Register<CartService, ICartService>();
            host.Register<CouponViewService, ICouponViewService>();
            host.Register<CartRepository, ICartRepository>();
            host.Register<CheckoutService, ICheckoutService>();
            host.Register<FulfillmentMethodRepository, IFulfillmentMethodRepository>();
            host.Register<CartViewModelFactory, ICartViewModelFactory>();
            host.Register<ShippingMethodViewService, IShippingMethodViewService>();
            host.Register<FulfillmentMethodRepository, IFulfillmentMethodRepository>();
            host.Register<PaymentViewService, IPaymentViewService>();
            host.Register<PaymentRepository, IPaymentRepository>();
            host.Register<CheckoutBreadcrumbViewService, ICheckoutBreadcrumbViewService>();
            host.Register<VaultProfileRepository, IVaultProfileRepository>();
            host.Register<VaultProfileViewService, IVaultProfileViewService>();

            host.Register<OrderRepository, IOrderRepository>();
            host.Register<OrderHistoryViewService, IOrderHistoryViewService>();
            host.Register<OrderHistoryViewModelFactory, IOrderHistoryViewModelFactory>();
            host.Register<FindOrdersRequestFactory, IFindOrdersRequestFactory>();
            host.Register<OrderDetailsViewModelFactory, IOrderDetailsViewModelFactory>();
            host.Register<LineItemService, ILineItemService>();
            host.Register<TaxViewModelFactory, ITaxViewModelFactory>();
            host.Register<LineItemViewModelFactory, ILineItemViewModelFactory>();
            host.Register<RewardViewModelFactory, IRewardViewModelFactory>();
            host.Register<FixCartService, IFixCartService>();

            host.Register<CartMergeProvider, ICartMergeProvider>();
            host.Register<SurfacedErrorLineItemValidationProvider, ILineItemValidationProvider>();

            host.Register<WishListViewService, IWishListViewService>();
            host.Register<WishListRepository, IWishListRepository>();

            host.Register<RecurringOrderCartViewModelFactory, IRecurringOrderCartViewModelFactory>();
            host.Register<RecurringOrderCartsViewService, IRecurringOrderCartsViewService>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(CartPlugin).Assembly);

            host.RegisterApiControllers(typeof(CartPlugin).Assembly);

            RegisterPaymentProviders(host);
            RegisterShippingTrackingProviders(host);
        }

        private static void RegisterPaymentProviders(IDependencyRegister host)
        {
            host.Register<IPaymentProviderRegistry>(CartConfiguration.PaymentProviderRegistry);
            host.Register<PaymentProviderFactory, IPaymentProviderFactory>(ComponentLifestyle.Transient);
            host.Register<MonerisCanadaPaymentProvider>(ComponentLifestyle.Transient);
            host.Register<OnSitePOSPaymentProvider>(ComponentLifestyle.Transient);
        }

        private static void RegisterShippingTrackingProviders(IDependencyRegister host)
        {
            host.Register<IShippingTrackingProviderRegistry>(CartConfiguration.ShippingTrackingProviderRegistry);
            host.Register<ShippingTrackingProviderFactory, IShippingTrackingProviderFactory>(ComponentLifestyle.Transient);
            host.Register<NullShippingTrackingProvider>(ComponentLifestyle.Transient);

            CartConfiguration.ShippingTrackingProviderRegistry.RegisterProvider<NullShippingTrackingProvider>(CartConfiguration.DefaultShippingTrackingProviderName);
        }
    }
}

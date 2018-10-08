using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Cart
{
    /// <summary>
    ///     Plugin configuations
    /// </summary>
    public static class CartConfiguration
    {
        [IgnoreCheck]
        public const string DefaultShippingTrackingProviderName = "NullShippingTrackingProvider";

        /// <summary>
        ///     Get/Set the Shopping CartName to use for storing a Shopping Cart in Overture
        /// </summary>
        public static string ShoppingCartName { get; set; } = "Default";

        /// <summary>
        ///     Get/Set the Wishlist CartName to use for storing a Wishlist Cart in Overture
        /// </summary>
        public static string WishlistCartName { get; set; } = "Wishlist";

        public static string WishListWorkflowToExecute { get; set; } = "DefaultCartWishlistWorkflow";

        public static bool WishListExecuteWorkflow { get; set; } = true;

        /// <summary>
        ///     Get/Set the ImageSize for dislaying thumbnails
        /// </summary>
        public static string ThumbnailImageSize { get; set; } = "M";

        /// <summary>
        /// Type registry for the Payment Providers.
        /// </summary>
        public static ProviderRegistry<IPaymentProvider> PaymentProviderRegistry { get; set; } = new PaymentProviderRegistry();

        /// <summary>
        /// Type registry for the shipping tracking Providers.
        /// </summary>
        public static ProviderRegistry<IShippingTrackingProvider> ShippingTrackingProviderRegistry { get; set; } = new ShippingTrackingProviderRegistry();

        /// <summary>
        ///     Get/Set the cart propertyBag key name for the last completed checkout step.
        /// </summary>
        public static string CartPropertyBagLastCheckoutStep { get; set; } = "LastCheckoutStep";
    }
}
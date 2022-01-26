using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Providers;
using Orckestra.Overture.ServiceModel.Orders;
using System.Collections.Generic;

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

        [IgnoreCheck]
        public const string OrderDraftCartType = "OrderDraft";

        /// <summary>
        ///     Get/Set the RecurringOrder CartName to use for storing a RecurringOrder Cart in Overture
        /// </summary>
        public static string RecurringOrderCartType { get; set; } = "RecurringOrderCart";

        /// <summary>
        ///     Get/Set the Wishlist CartName to use for storing a Wishlist Cart in Overture
        /// </summary>
        public static string WishlistCartName { get; set; } = "Wishlist";

        public static string WishListWorkflowToExecute { get; set; } = "DefaultCartWishlistWorkflow";

        public static bool WishListExecuteWorkflow { get; set; } = true;

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

        /// <summary>
        ///     Get/Set the supported payment method types when doing a checkout with recurring order line items.
        /// </summary>
        public static IEnumerable<PaymentMethodType> SupportedRecurringOrderPaymentMethodTypes = new List<PaymentMethodType>
        {
            PaymentMethodType.CreditCard,
            PaymentMethodType.SavedCreditCard
        };

        /// <summary>
        /// If true, cart items will be grouped by product primary category
        /// </summary>
        public static bool GroupCartItemsByPrimaryCategory { get; set; }
    }
}
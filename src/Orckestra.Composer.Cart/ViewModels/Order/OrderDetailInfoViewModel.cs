using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderDetailInfoViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the identifier for an order.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        /// <value>
        /// The "raw" order status key, untranslated.
        /// </value>
        public string OrderStatusRaw { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the date the order was created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [MapTo("Created")]
        [Formatting("General", "ShortDateFormat")]
        public string OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        [Formatting("General", "PriceFormat")]
        [MapTo("Total")]
        public string PricePaid { get; set; }

        /// <summary>
        /// Gets or sets the billing currency.
        /// </summary>
        /// <value>
        /// The billing currency.
        /// </value>
        public string BillingCurrency { get; set; }

        /// <summary>
        /// Gets or sets the source of the order.
        /// </summary>
        /// <value>
        /// The source of the order.
        /// </value>
        public string Source { get; set; }
    }
}

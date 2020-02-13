namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrdersParam
    {
        /// <summary>
        /// Gets or sets the current page of orders.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the order tense.
        /// </summary>
        /// <value>
        /// The order tense.
        /// </value>
        public OrderTense OrderTense { get; set; }
    }
}
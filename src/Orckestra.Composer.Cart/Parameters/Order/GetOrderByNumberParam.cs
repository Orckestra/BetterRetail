namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderByNumberParam : GetOrderParam
    {
        /// <summary>
        /// Gets or sets the Number of the order to retrieve.
        /// </summary>
        /// <value>
        /// The Order Number.
        /// </value>
        public string OrderNumber { get; set; }
    }
}

namespace Orckestra.Composer.Cart.Parameters
{
    /// <summary>
    /// Parameters for adding an item to the cart
    /// </summary>
    public class AddLineItemParam : BaseCartParam
    {
        /// <summary>
        /// The product id to add
        /// Required
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The variant id
        /// Optionnal if the product doesn't have any.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// Quantity to add
        /// Required, must be positive
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// The name of the recurring order program frequency
        /// </summary>
        public string RecurringOrderFrequencyName { get; set; }

        /// <summary>
        /// The name of the recurring order program.
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        /// <summary>
        /// The base Url associated with the current page
        /// </summary>
        public string BaseUrl { get; set; }

        public AddLineItemParam Clone()
        {
            var param = (AddLineItemParam)MemberwiseClone();
            return param;
        }
    }
}

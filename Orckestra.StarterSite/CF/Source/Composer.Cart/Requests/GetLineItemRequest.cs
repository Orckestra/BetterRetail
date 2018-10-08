
namespace Orckestra.Composer.Cart.Requests
{
    /// <summary>
    /// Parameters for adding an item to the cart
    /// </summary>
    public class GetLineItemRequest
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

    }
}

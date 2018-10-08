namespace Orckestra.Composer.Providers.Dam
{
    /// <summary>
    /// Exposes the URL of a product or variant's main image.
    /// </summary>
    public class ProductMainImage
    {
        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the variant id.
        /// </summary>
        /// <value>
        /// The variant id.
        /// </value>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the fallback Image URL to display when no ImageUrl are available
        /// </summary>
        /// <value>
        /// The fallback image URL.
        /// </value>
        public string FallbackImageUrl { get; set; }
    }
}

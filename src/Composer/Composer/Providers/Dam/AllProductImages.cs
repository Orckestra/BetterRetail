using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Providers.Dam
{
    public class AllProductImages
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
        /// Gets or sets the sequence number for ordering this image.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the Thumbnail URL.
        /// </summary>
        /// <value>
        /// The thumbnail URL.
        /// </value>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the Product Zoom Image URL.
        /// </summary>
        /// <value>
        /// The Product Zoom Image URL.
        /// </value>
        public string ProductZoomImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the fallback Image URL to display when no ImageUrl are available
        /// </summary>
        /// <value>
        /// The fallback image URL.
        /// </value>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// Get the status if product zoom image URL is defined
        /// </summary>        
        public bool IsProductZoomImageUrlDefined
        {
            get { return ProductZoomImageUrl != null; }
        }

        /// <summary>
        /// Get or set alternate text for image
        /// </summary>        
        public LocalizedString Alt { get; set; }
    }
}
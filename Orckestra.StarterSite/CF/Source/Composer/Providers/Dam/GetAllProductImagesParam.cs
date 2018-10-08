using System.Collections.Generic;

namespace Orckestra.Composer.Providers.Dam
{
    public class GetAllProductImagesParam
    {
        public GetAllProductImagesParam()
        {
            PropertyBag = new Dictionary<string, object>();
            Variants = new List<VariantKey>();
        }

        /// <summary>
        /// Size hint to use for finding the ImageUrl
        /// ("Medium", "Small", "Large", ...)
        /// </summary>
        public string ImageSize { get; set; }

        /// <summary>
        /// Size hint to use for finding the ThumbnailImageUrl
        /// ("Medium", "Small", "Large", ...)
        /// </summary>
        public string ThumbnailImageSize { get; set; }

        /// <summary>
        /// Size hint to use for finding the ThumbnailImageUrl
        /// ("Medium", "Small", "Large", ...)
        /// </summary>
        public string ProductZoomImageSize { get; set; }

        /// <summary>
        /// ProductId to request the image For
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Variant Unique identification including the Id and the KVAs
        /// </summary>
        public IList<VariantKey> Variants { get; set; }

        /// <summary>
        /// Bag of additionnal properties related to the Product
        /// Those additionnal properties are forwarded to the Dam Provider
        /// To allows different url resolving based on the product info.
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }

        /// <summary>
        /// Gets or sets the name of the product definition.
        /// </summary>
        /// <value>
        /// The name of the product definition.
        /// </value>
        public string ProductDefinitionName { get; set; }        
    }
}

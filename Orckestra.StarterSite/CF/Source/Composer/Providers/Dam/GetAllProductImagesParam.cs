using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Providers.Dam
{
    public class GetAllProductImagesParam
    {
        public GetAllProductImagesParam()
        {
            PropertyBag = new Dictionary<string, object>();
            Variants = new List<Variant>();
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
        public IList<Variant> Variants { get; set; }

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

        /// <summary>
        /// The list of media of the Product
        /// </summary>
        public List<ProductMedia> MediaSet { get; set; }

        /// <summary>
        /// The list of variant media of the Product
        /// </summary>
        public List<VariantMediaSet> VariantMediaSet { get; set; }
    }
}

using System.Collections.Generic;

namespace Orckestra.Composer.Providers.Dam
{
    /// <summary>
    /// Represents a request encapsulating the information of a product image.
    /// </summary>
    public class ProductImageRequest
    {
        public ProductImageRequest()
        {
            PropertyBag = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Variant Unique identification including the Id and the KVAs
        /// </summary>
        /// <value>
        /// The variant keys.
        /// </value>
        public VariantKey Variant { get; set; }

        /// <summary>
        /// Property of either the Product or the Variant
        /// It contains the Key variant attributes, the attributes and product properties
        /// </summary>
        /// <value>
        /// Property values
        /// </value>
        public Dictionary<string,object> PropertyBag { get; set; }

        /// <summary>
        /// Gets or sets the name of the product definition.
        /// </summary>
        /// <value>
        /// The name of the product definition.
        /// </value>
        public string ProductDefinitionName { get; set; }
    }
}

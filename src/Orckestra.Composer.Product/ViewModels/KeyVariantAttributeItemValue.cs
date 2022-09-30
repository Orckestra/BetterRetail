using System.Collections.Generic;

namespace Orckestra.Composer.Product.ViewModels
{
    /// <summary>
    /// Values contained in the KeyVariantItem.  Represents all possible values for a certain product
    /// kva item.  Eg. list of available colors values.
    /// </summary>
    public class KeyVariantAttributeItemValue
    {
        /// <summary>
        /// Localized Display name for this Key Variant Attribute values
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Raw value for this Key variant attribute value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Configured value for this Key variant attribute value, for example custom color value
        /// </summary>
        public object ConfiguredValue { get; set; }

        /// <summary>
        /// Selected State of this Key variant attribute
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Reachable state of this Key variant attribute.
        /// A disabled Kva cannot be selected
        /// </summary>
        public bool Disabled { get; set; }

        public List<string> RelatedVariantIds { get; set; }

        /// <summary>
        /// ImageUrl to display this key variant attribute value or null if none available
        /// </summary>
        public string ImageUrl { get; set; }

        public KeyVariantAttributeItemValue()
        {
            RelatedVariantIds = new List<string>();
        }
    }
}

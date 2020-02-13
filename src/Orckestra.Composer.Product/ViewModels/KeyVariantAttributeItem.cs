using System.Collections.Generic;

namespace Orckestra.Composer.Product.ViewModels
{
    /// <summary>
    /// Represent a key variant item.
    /// 
    /// This object is used to store product's possible key variant attribute.
    /// For instance, if a product has colors part of it's kva, this object will
    /// contains the color and all it's possible values for the specific product.
    /// </summary>
    public class KeyVariantAttributeItem
    {
        public string DisplayName { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDataType { get; set; }

        public IList<KeyVariantAttributeItemValue> Values { get; set; }

        public KeyVariantAttributeItem()
        {
            Values = new List<KeyVariantAttributeItemValue>();
        }
    }
}

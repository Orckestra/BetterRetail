using System.Collections.Generic;

namespace Orckestra.Composer.Providers.Dam
{
    public class VariantKey
    {
        public static readonly VariantKey Empty = new VariantKey();

        public VariantKey()
        {
            KeyVariantAttributeValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// VariantId to request the image For
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Key Variant Attribute values
        /// Can be used to identify this variant
        /// </summary>
        public Dictionary<string, object> KeyVariantAttributeValues { get; set; }
    }
}

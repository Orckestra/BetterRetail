using Orckestra.Composer.Providers.Dam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Utils
{
    public static class LineItemHelper
    {
        /// <summary>
        /// Quick Access lookup for images
        /// Group by Product then by VariantId
        /// </summary>
        public static IDictionary<(string ProductId, string VariantId), ProductMainImage> BuildImageDictionaryFor(IList<ProductMainImage> images)
        {
            return images == null
                ? new Dictionary<(string ProductId, string VariantId), ProductMainImage>()
                : images.GroupBy(image => (image.ProductId, image.VariantId))
                .ToDictionary(img => img.Key, img => img.FirstOrDefault());
        }
    }
}

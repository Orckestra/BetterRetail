using System.Collections.Generic;
using Orckestra.Composer.Providers.Dam;

namespace Orckestra.Composer.Cart.Parameters
{
    public class ProductImageInfo
    {
        public List<ProductMainImage> ImageUrls { get; set; }

        public ProductImageInfo()
        {
            ImageUrls = new List<ProductMainImage>();
        }
    }
}

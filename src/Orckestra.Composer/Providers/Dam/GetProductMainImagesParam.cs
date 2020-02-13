using System.Collections.Generic;

namespace Orckestra.Composer.Providers.Dam
{
    /// <summary>
    /// Parameter object which encapsulates the information related to a product image used by the DAM provider during a query.
    /// </summary>
    public class GetProductMainImagesParam
    {
        public string ImageSize { get; set; }
        public IList<ProductImageRequest> ProductImageRequests { get; set; }

        public GetProductMainImagesParam()
        {
            ProductImageRequests = new List<ProductImageRequest>();
        }
    }
}

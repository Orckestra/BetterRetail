using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Interface of the DAM provider.
    /// </summary>
    public interface IDamProvider
    {
        /// <summary>
        /// Asynchronously returns a list of main images associated to a product or a variant.
        /// </summary>
        /// <param name="param">The product or variant information which will be used to retrieve a list of images associated to them.</param>
        /// <returns>A list of main images associated to a product or a variant.</returns>
        Task<List<ProductMainImage>> GetProductMainImagesAsync(GetProductMainImagesParam param);

        /// <summary>
        /// Asynchronously returns a list of all kinds of images (such as main images and thumbnails) associated to a product or a variant.
        /// </summary>
        /// <param name="param">The image, product and/or variant information which will be used to retrieve a list of images associated to them.</param>
        /// <returns>A list of all kinds of images associated to a product or variants.</returns>
        Task<List<AllProductImages>> GetAllProductImagesAsync(GetAllProductImagesParam param);

        /// <summary>
        /// Get main media image from product
        /// </summary>
        /// <param name="product">Full product entity</param>
        /// <param name="variantId">Variant id key</param>
        /// <returns>Url main media image</returns>
        string GetMediaImageUrl(Product product, string variantId);
    }
}

using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Services
{
    public class ImageService : IImageService
    {
        protected IDamProvider DamProvider { get; private set; }
        protected IProductRepository ProductRepository { get; }
        protected IComposerContext ComposerContext { get; private set; }

        public ImageService(IDamProvider damProvider, IProductRepository productRepository, IComposerContext composerContext)
        {
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException("composerContext");
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems)
        {
            return GetImageUrlsAsync(lineItems.Select(lineItem => (lineItem.ProductId, lineItem.VariantId)).ToList());
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list)
        {
            return GetImageUrlsAsync(list.RecurringOrderLineItems.Select(lineItem => (lineItem.ProductId, lineItem.VariantId)).ToList());
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(RecurringOrderLineItem lineItem)
        {
            return GetImageUrlsAsync(new[] { (lineItem.ProductId, lineItem.VariantId) });
        }

        private async Task<List<ProductMainImage>> GetImageUrlsAsync(ICollection<(string productId, string variantId)> products)
        {
            var imageUrls = await GetImageUrlsForProducts(products).ConfigureAwait(false);
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.CartThumbnailImageSize,
                ProductImageRequests = new List<ProductImageRequest>(products.Count),
            };

            foreach (var product in products)
            {
                var productImageRequest = new ProductImageRequest
                {
                    ProductId = product.productId,
                    Variant = new VariantKey {Id = product.variantId},
                };
                var imageUrl = imageUrls[product];
                if (imageUrl != null)
                {
                    productImageRequest.PropertyBag = new Dictionary<string, object>
                    {
                        ["ImageUrl"] = imageUrl
                    };
                }

                getImageParam.ProductImageRequests.Add(productImageRequest);
            }

            return await DamProvider.GetProductMainImagesAsync(getImageParam).ConfigureAwait(false);
        }

        private async Task<Dictionary<(string productId, string variantId), string>> GetImageUrlsForProducts(
            IEnumerable<(string productId, string variantId)> products)
        {
            var tasks = products
                .Distinct()
                .Select(async product =>
                    new
                    {
                        key = product,
                        image = await ProductRepository.GetImageUrlAsync(product.productId, product.variantId, ComposerContext.Scope)
                    });

            var imagesList = await Task.WhenAll(tasks);
            return imagesList.ToDictionary(x => x.key, x => x.image);
        }
    }
}

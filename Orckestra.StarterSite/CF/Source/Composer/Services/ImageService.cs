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
        protected IComposerRequestContext ComposerContext { get; private set; }

        public ImageService(IDamProvider damProvider, IProductRepository productRepository, IComposerRequestContext composerContext)
        {
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException("composerContext");
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems)
        {
            return GetImageUrlsAsync(lineItems, ImageConfiguration.CartThumbnailImageSize);
        }

        public virtual async Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems, string imageSize)
        {
            var ImageUrls = await GetImageUrlsBySkuAsync(lineItems.Select(ln => ln.Sku)).ConfigureAwait(false);

            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = imageSize,
                ProductImageRequests = lineItems
                    .Select(li =>
                    {
                        if (ImageUrls.TryGetValue(li.Sku, out string imageUrl))
                        {
                            li.PropertyBag.Add("ImageUrl", imageUrl);
                        }

                        return new ProductImageRequest
                        {
                            ProductId = li.ProductId,
                            Variant = new VariantKey
                            {
                                Id = li.VariantId,
                                KeyVariantAttributeValues = li.KvaValues

                            },
                            PropertyBag = li.PropertyBag,
                            ProductDefinitionName = li.ProductDefinitionName
                        };
                    }).ToList()
            };
            return await DamProvider.GetProductMainImagesAsync(getImageParam).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, string>> GetProductsImageUrls(IEnumerable<string> productIds)
        {
            var productsData = await ProductRepository.SearchProductByIdsAsync(productIds.ToList(), ComposerContext.Scope, ComposerContext.CultureInfo.Name).ConfigureAwait(false);

            return productsData.Documents.Select(document =>
            {
                var productId = document.PropertyBag["ProductId"].ToString();
                var imageUrl = document.PropertyBag["ImageUrl"].ToString();
                return new
                {
                    ProductId = productId,
                    ImageUrl = imageUrl
                };
            }).ToDictionary(x => x.ProductId, x => x.ImageUrl);
        }

        private async Task<Dictionary<string, string>> GetImageUrlsBySkuAsync(IEnumerable<string> skuIds)
        {
            var mediaList = await Task.WhenAll(skuIds.Select(async sku =>
            {
                var mediaSetResult = await ProductRepository.GetProductMediaAsync(sku, ComposerContext.Scope, ComposerContext.CultureInfo.Name, "Image");
                var mediaSet = mediaSetResult.MediaSet.Where(x => x.IsCover ?? true).ToList();

                if (mediaSet.Count == 0) return null;

                return new
                {
                    Sku = sku,
                    ImageUrl = mediaSet.Last().Url
                };
            }));

            return mediaList.Where(x => x != null).ToDictionary(x => x.Sku, x => x.ImageUrl);
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list)
        {
            return GetImageUrlsAsync(list.RecurringOrderLineItems);
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(RecurringOrderLineItem lineitem)
        {
            return GetImageUrlsAsync(new List<RecurringOrderLineItem> { lineitem });
        }

        private async Task<List<ProductMainImage>> GetImageUrlsAsync(List<RecurringOrderLineItem> lineItemList)
        {
            var ImageUrls = await GetImageUrlsBySkuAsync(lineItemList.Select(ln => ln.Sku)).ConfigureAwait(false);

            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.CartThumbnailImageSize,
                ProductImageRequests = lineItemList
                  .Select(li =>
                  {
                      var PropertyBag = ImageUrls.TryGetValue(li.Sku, out string imageUrl) ? new Dictionary<string, object>() { { "ImageUrl", imageUrl } } : null;

                      return new ProductImageRequest
                      {
                          ProductId = li.ProductId,
                          Variant = new VariantKey
                          {
                              Id = li.VariantId,
                          },
                          PropertyBag = PropertyBag
                      };
                  }).ToList()
            };
            return await DamProvider.GetProductMainImagesAsync(getImageParam).ConfigureAwait(false);
        }
    }
}

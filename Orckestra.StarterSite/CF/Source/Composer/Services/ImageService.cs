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
            return GetImageUrlsAsync(lineItems, ImageConfiguration.CartThumbnailImageSize);
        }

        public virtual async Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems, string imageSize)
        {
            var ImageUrls = await GetProductsImageUrls(lineItems.Select(ln => ln.ProductId)).ConfigureAwait(false);

            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = imageSize,
                ProductImageRequests = lineItems
                    .Select(li => {
                        var imageUrl = ImageUrls[li.ProductId];
                        if (imageUrl != null)
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

            return productsData.Documents.Select(document => {
                var productId = document.PropertyBag["ProductId"].ToString();
                var imageUrl = document.PropertyBag["ImageUrl"].ToString();
                return new
                {
                    ProductId = productId,
                    ImageUrl = imageUrl
                };
            }).ToDictionary(x => x.ProductId, x => x.ImageUrl);
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
            var ImageUrls = await GetProductsImageUrls(lineItemList.Select(ln => ln.ProductId)).ConfigureAwait(false);

            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.CartThumbnailImageSize,
                ProductImageRequests = lineItemList
                  .Select(li => {
                      string imageUrl = ImageUrls[li.ProductId];

                      return new ProductImageRequest
                      {
                          ProductId = li.ProductId,
                          Variant = new VariantKey
                          {
                              Id = li.VariantId,
                          },
                          PropertyBag = !string.IsNullOrEmpty(imageUrl) ? new Dictionary<string, object>(){ { "ImageUrl", imageUrl } } : null
                      };
                  }).ToList()
            };
            return await DamProvider.GetProductMainImagesAsync(getImageParam).ConfigureAwait(false);
        }
    }
}

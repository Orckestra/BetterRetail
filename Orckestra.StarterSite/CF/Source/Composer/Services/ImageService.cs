using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public class ImageService : IImageService
    {
        protected IDamProvider DamProvider { get; private set; }

        public ImageService(IDamProvider damProvider)
        {
            if (damProvider == null) { throw new ArgumentNullException(nameof(damProvider)); }

            DamProvider = damProvider;
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems)
        {
            return GetImageUrlsAsync(lineItems, ImageConfiguration.CartThumbnailImageSize);
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems, string imageSize)
        {
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = imageSize,
                ProductImageRequests = lineItems
                    .Select(li => new ProductImageRequest
                    {
                        ProductId = li.ProductId,
                        Variant = new VariantKey
                        {
                            Id = li.VariantId,
                            KeyVariantAttributeValues = li.KvaValues

                        },
                        PropertyBag = li.PropertyBag,
                        ProductDefinitionName = li.ProductDefinitionName
                    })
                    .ToList()
            };
            return DamProvider.GetProductMainImagesAsync(getImageParam);
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list)
        {
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.CartThumbnailImageSize,
                ProductImageRequests = list.RecurringOrderLineItems
                    .Select(li => new ProductImageRequest
                    {
                        ProductId = li.ProductId,
                        Variant = new VariantKey
                        {
                            Id = li.VariantId,
                        },
                    })
                    .ToList()
            };
            return DamProvider.GetProductMainImagesAsync(getImageParam);
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(RecurringOrderLineItem lineitem)
        {
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.CartThumbnailImageSize,
                ProductImageRequests = new List<ProductImageRequest> {
                    new ProductImageRequest
                    {
                        ProductId = lineitem.ProductId,
                        Variant = new VariantKey
                        {
                            Id = lineitem.VariantId,
                        },
                    }}.ToList()
            };
            return DamProvider.GetProductMainImagesAsync(getImageParam);
        }

    }
}

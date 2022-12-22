using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public class ImageService : IImageService
    {
        protected IDamProvider DamProvider { get; private set; }
        protected IProductRepository ProductRepository { get; }
        protected IComposerContext ComposerContext { get; private set; }

        private const string VariantPropertyBagKey = "VariantId";
        private const string DefinitionNamePropertyBagKey = "DefinitionName";

        public ImageService(IDamProvider damProvider, IProductRepository productRepository, IComposerContext composerContext)
        {
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems)
        {
            return GetImageUrlsAsync(lineItems.Select(lineItem => (lineItem.ProductId, lineItem.VariantId, lineItem.PropertyBag != null && lineItem.PropertyBag.ContainsKey("ImageUrl") ? lineItem.PropertyBag["ImageUrl"].ToString() : null)).ToList());
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list)
        {
            return GetImageUrlsAsync(list.RecurringOrderLineItems.Select(lineItem =>  (lineItem.ProductId, lineItem.VariantId, lineItem.PropertyBag != null && lineItem.PropertyBag.ContainsKey("ImageUrl") ? lineItem.PropertyBag["ImageUrl"].ToString(): null)).ToList());
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(RecurringOrderLineItem lineItem)
        {
            return GetImageUrlsAsync(new[] { (lineItem.ProductId, lineItem.VariantId, lineItem.PropertyBag != null && lineItem.PropertyBag.ContainsKey("ImageUrl") ? lineItem.PropertyBag["ImageUrl"].ToString() : null) });
        }

        public virtual async Task<List<ProductMainImage>> GetImageUrlsAsync(ICollection<(string productId, string variantId, string imageUrl)> products)
        {
            var productsWithoutImageUrls = products.Where(p => string.IsNullOrEmpty(p.imageUrl)).Select(p => (p.productId, p.variantId));
            var imageUrls = await GetMediaImageUrlsForProducts(productsWithoutImageUrls).ConfigureAwait(false);
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.ProductTileImageSize,
                ProductImageRequests = new List<ProductImageRequest>(products.Count),
            };

            foreach (var product in products)
            {
                var productImageRequest = new ProductImageRequest
                {
                    ProductId = product.productId,
                    Variant = new VariantKey { Id = product.variantId },
                 };

                var imageUrl = product.imageUrl ?? (imageUrls != null ? imageUrls[(product.productId, product.variantId)]: null);
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

        protected virtual async Task<Dictionary<(string productId, string variantId), string>> GetMediaImageUrlsForProducts(
            IEnumerable<(string productId, string variantId)> products)
        {
            if(products == null || products.Count() == 0) return null;
            var productIds = products.Select(p => p.productId).ToArray();
            var productsList = await ProductRepository.GetProductsByIdsAsync(productIds, ComposerContext.Scope).ConfigureAwait(false);

            var imagesList = new Dictionary<(string productId, string variantId), string>();
            foreach (var product in products)
            {
                var productEntity = productsList.Products.FirstOrDefault(p => p.Id == product.productId);
                var image = DamProvider.GetMediaImageUrl(productEntity, product.variantId);
                imagesList.Add(product, image);

            }
            return imagesList;
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsFromProperty(List<ProductDocument> documents)
        {
            return GetImageUrlsFromProperty(documents.Select(doc => (doc.ProductId, doc.PropertyBag.ContainsKey(VariantPropertyBagKey) ? doc.PropertyBag[VariantPropertyBagKey].ToString() : string.Empty, doc.PropertyBag)));
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsFromProperty(List<LineItem> documents)
        {
            return GetImageUrlsFromProperty(documents.Select(doc => (doc.ProductId, doc.VariantId, doc.PropertyBag)));
        }

        protected virtual async Task<List<ProductMainImage>> GetImageUrlsFromProperty(IEnumerable<(string productId, string variantId, PropertyBag propertyBag)> documents)
        {
            var imagesParam = new GetProductMainImagesParam
            {
                ImageSize = ImageConfiguration.ProductTileImageSize,
                ProductImageRequests = documents
                   .Select(document => new ProductImageRequest
                   {
                       ProductId = document.productId,
                       Variant = new VariantKey { Id = document.variantId },
                       ProductDefinitionName = document.propertyBag.ContainsKey(DefinitionNamePropertyBagKey)
                           ? document.propertyBag[DefinitionNamePropertyBagKey].ToString()
                           : string.Empty,
                       PropertyBag = document.propertyBag
                   }).ToList()
            };

            return await DamProvider.GetProductMainImagesAsync(imagesParam).ConfigureAwait(false);
        }
    }
}

using Orckestra.Composer.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Providers.Dam
{
    public class ProductMediaProvider : IDamProvider
    {
        protected IProductMediaSettingsRepository ProductMediaSettingsRepository { get; private set; }

        public ProductMediaProvider (IProductMediaSettingsRepository productMediaSettingsRepository)
        {
            ProductMediaSettingsRepository = productMediaSettingsRepository;
        }

        public async Task<List<ProductMainImage>> GetProductMainImagesAsync(GetProductMainImagesParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param", "The method parameter is required.");
            }
            if (string.IsNullOrWhiteSpace(param.ImageSize))
            {
                throw new ArgumentException("The image size is required.");
            }
            if (param.ProductImageRequests == null)
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProductImageRequests"), "param");
            }
            if (param.ProductImageRequests.Any(request => string.IsNullOrWhiteSpace(request.ProductId)))
            {
                throw new ArgumentException("The product id must be specified for each ProductImageRequests object.");
            }

            var _productMediaSettings = await ProductMediaSettingsRepository.GetProductMediaSettings().ConfigureAwait(false);

            var result = param.ProductImageRequests.Select(request => new ProductMainImage
            {
                ImageUrl = null, //GetImageUrl(_productMediaSettings, param.ImageSize, request.ProductId, request.Variant.Id, MainImageSequenceNumber),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl(_productMediaSettings)
            }).ToList();

            return result;
        }

        public async Task<List<AllProductImages>> GetAllProductImagesAsync(GetAllProductImagesParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param", "The method parameter is required.");
            }
            if (string.IsNullOrWhiteSpace(param.ImageSize))
            {
                throw new ArgumentException("The image size is required.");
            }
            if (string.IsNullOrWhiteSpace(param.ThumbnailImageSize))
            {
                throw new ArgumentException("The thumbnail image size is required.");
            }
            if (string.IsNullOrWhiteSpace(param.ProductZoomImageSize))
            {
                throw new ArgumentException("The product zoom image size is required.");
            }
            if (string.IsNullOrWhiteSpace(param.ProductId))
            {
                throw new ArgumentException("The product id is required.");
            }
            if (param.MediaSet == null)
            {
                throw new ArgumentException("The MediaSet is required.");
            }

            var _productMediaSettings = await ProductMediaSettingsRepository.GetProductMediaSettings().ConfigureAwait(false);

            var result = param.MediaSet.Select(productMedia => CreateAllProductImages(productMedia, _productMediaSettings, param, null)).ToList();

            if (param.Variants != null)
            {
                foreach(Variant variant in param.Variants)
                {
                    var globalVariant = param.VariantMediaSet?.FirstOrDefault(mediaVariant => mediaVariant.AttributesToMatch.Any(atribute => variant.PropertyBag.Contains(atribute)));

                    var localVariantMediaSet = variant.MediaSet != null && variant.MediaSet.Count > 0 ? variant.MediaSet : null;
                    var globalVariantMediaSet = globalVariant != null && globalVariant.Media != null && globalVariant.Media.Length > 0 ? globalVariant.Media.ToList() : null;

                    result.AddRange(
                        (localVariantMediaSet ?? globalVariantMediaSet ?? param.MediaSet).Select(productMedia => CreateAllProductImages(productMedia, _productMediaSettings, param, variant.Id))
                    );
                }
            }
            return result;
        }

        private AllProductImages CreateAllProductImages(ProductMedia productMedia, MediaSettings mediaSettings, GetAllProductImagesParam param, string variantId)
        {
            return new AllProductImages
            {
                ImageUrl = GetSizedImageUrl(productMedia, mediaSettings, param.ImageSize),
                ThumbnailUrl = GetSizedImageUrl(productMedia, mediaSettings, param.ThumbnailImageSize),
                ProductZoomImageUrl = GetSizedImageUrl(productMedia, mediaSettings, param.ProductZoomImageSize),
                ProductId = param.ProductId,
                VariantId = variantId,
                SequenceNumber = 0,
                FallbackImageUrl = GetFallbackImageUrl(mediaSettings),
            };
        }

        private string GetFallbackImageUrl(MediaSettings mediaSettings) => mediaSettings.MediaServerUrl + mediaSettings.MediaFallbackImageName;
        private string GetImageUrl(string imagePath, MediaSettings mediaSettings) => imagePath.Replace("~/", mediaSettings.MediaServerUrl);

        private string GetSizedImageUrl(ProductMedia productMedia, MediaSettings mediaSettings, string size)
        {
            if(productMedia.ResizedInstances != null && productMedia.ResizedInstances.Length > 0 && !string.IsNullOrEmpty(size))
            {
                var resizedImage = productMedia.ResizedInstances.FirstOrDefault(resizedImg => resizedImg.Size == size);

                if(resizedImage != null)
                    return GetImageUrl(resizedImage.Url, mediaSettings);
            }

            return GetImageUrl(productMedia.Url, mediaSettings);
        }
    }
}

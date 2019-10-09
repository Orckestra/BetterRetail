using Orckestra.Composer.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Providers.Dam
{
    public class ConventionBasedDamProvider : IDamProvider
    {
        private readonly ICdnDamProviderSettings _cdnDamProviderSettings;
        protected IProductMediaSettingsRepository ProductMediaSettingsRepository { get; private set; }

        private const string ProductIdFieldName = "{productId}";
        private const string VariantIdFieldName = "{variantId}";
        private const string ImageSizeFieldName = "{imageSize}";
        private const string SequenceNumberFieldName = "{sequenceNumber}";
        private const int MainImageSequenceNumber = 0;

        public ConventionBasedDamProvider(ISiteConfiguration siteConfiguration, IProductMediaSettingsRepository productMediaSettingsRepository)
        {
            _cdnDamProviderSettings = siteConfiguration.CdnDamProviderSettings;
            ProductMediaSettingsRepository = productMediaSettingsRepository;
        }

        protected string ServerUrl
        {
            get { return _cdnDamProviderSettings.ServerUrl; }
        }

        protected virtual string ImageFolderName
        {
            get { return _cdnDamProviderSettings.ImageFolderName; }
        }

        protected virtual string FallbackImageUrl
        {
            get { return _cdnDamProviderSettings.FallbackImage; }
        }

        protected virtual bool IsProductZoomImageEnabled
        {
            get { return _cdnDamProviderSettings.SupportXLImages; }
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

            var result = param.ProductImageRequests.Select(request =>
            {
                return request.PropertyBag.ContainsKey("ImageUrl")
                ? GetProductMainMediaImage(request, _productMediaSettings)
                : GetProductMainLocalImage(request, param.ImageSize);
            }).ToList();

            return result;
        }

        private ProductMainImage GetProductMainLocalImage(ProductImageRequest request, string imageSize)
        {
            return new ProductMainImage
            {
                ImageUrl = GetImageUrl(imageSize, request.ProductId, request.Variant.Id, MainImageSequenceNumber),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl()
            };
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
                return GetAllProductLocalImages(param);
            }

            return await GetAllProductMediaImages(param).ConfigureAwait(false); 
        }

        private List<AllProductImages> GetAllProductLocalImages(GetAllProductImagesParam param)
        {
            var result = new List<AllProductImages>();
            for (var sequenceNumber = 0; sequenceNumber < ConventionBasedDamProviderConfiguration.MaxThumbnailImages; sequenceNumber++)
            {
                result.Add(CreateAllProductImages(param, null, sequenceNumber));

                if (param.Variants != null)
                {
                    result.AddRange(
                        param.Variants.Select(variantKey => CreateAllProductImages(param, variantKey.Id, sequenceNumber))
                    );
                }

            }
            return result;
        }

        private AllProductImages CreateAllProductImages(GetAllProductImagesParam param, string variantId, int sequenceNumber)
        {
            return new AllProductImages
            {
                ImageUrl = GetImageUrl(param.ImageSize, param.ProductId, variantId, sequenceNumber),
                ThumbnailUrl = GetImageUrl(param.ThumbnailImageSize, param.ProductId, variantId, sequenceNumber),
                // If support for XL images is disabled put "null" in the ProductZoomImageUrl property.
                ProductZoomImageUrl = IsProductZoomImageEnabled ? GetImageUrl(param.ProductZoomImageSize, param.ProductId, variantId, sequenceNumber) : null,
                ProductId = param.ProductId,
                VariantId = variantId,
                SequenceNumber = sequenceNumber,
                FallbackImageUrl = GetFallbackImageUrl()
            };
        }

        public string GetFallbackImageUrl()
        {
            return GetFormattedImageUrl(FallbackImageUrl);
        }

        private string GetImageUrl(string imageSize, string productId, string variantId, int sequenceNumber)
        {
            string imagePath;

            if (!string.IsNullOrEmpty(variantId))
            {
                // Resolve the image path for a variant.
                imagePath = _cdnDamProviderSettings.VariantImageFilePathPattern
                                                      .Replace(ProductIdFieldName, productId)
                                                      .Replace(VariantIdFieldName, variantId)
                                                      .Replace(SequenceNumberFieldName, sequenceNumber.ToString(CultureInfo.InvariantCulture))
                                                      .Replace(ImageSizeFieldName, imageSize);

            }
            else
            {
                // Resolve the image path for a product.
                imagePath = _cdnDamProviderSettings.ProductImageFilePathPattern
                                                      .Replace(ProductIdFieldName, productId)
                                                      .Replace(SequenceNumberFieldName, sequenceNumber.ToString(CultureInfo.InvariantCulture))
                                                      .Replace(ImageSizeFieldName, imageSize);
            }

            return GetFormattedImageUrl(imagePath);
        }

        private string GetFormattedImageUrl(string imageFilename)
        {
            return string.Format("{0}/{1}/{2}", ServerUrl, ImageFolderName, imageFilename);
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
            if (productMedia.ResizedInstances != null && productMedia.ResizedInstances.Length > 0 && !string.IsNullOrEmpty(size))
            {
                var resizedImage = productMedia.ResizedInstances.FirstOrDefault(resizedImg => resizedImg.Size == size);

                if (resizedImage != null)
                    return GetImageUrl(resizedImage.Url, mediaSettings);
            }

            return GetImageUrl(productMedia.Url, mediaSettings);
        }

        private async Task<List<AllProductImages>> GetAllProductMediaImages(GetAllProductImagesParam param)
        {
            var _productMediaSettings = await ProductMediaSettingsRepository.GetProductMediaSettings().ConfigureAwait(false);

            var result = param.MediaSet.Select(productMedia => CreateAllProductImages(productMedia, _productMediaSettings, param, null)).ToList();

            if (param.Variants != null)
            {
                foreach (Variant variant in param.Variants)
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

        private ProductMainImage GetProductMainMediaImage(ProductImageRequest request, MediaSettings mediaSettings)
        {
            return new ProductMainImage
            {
                ImageUrl = GetImageUrl(request.PropertyBag["ImageUrl"].ToString(), mediaSettings),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl(mediaSettings)
            };
        }
    }
}

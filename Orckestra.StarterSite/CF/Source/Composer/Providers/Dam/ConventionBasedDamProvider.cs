using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;
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

        private const string ProductIdFieldName = "{productId}";
        private const string VariantIdFieldName = "{variantId}";
        private const string ImageSizeFieldName = "{imageSize}";
        private const string SequenceNumberFieldName = "{sequenceNumber}";
        private const int MainImageSequenceNumber = 0;

        public ConventionBasedDamProvider()
        {
            _cdnDamProviderSettings = SiteConfiguration.CdnDamProviderSettings;
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

        public Task<List<ProductMainImage>> GetProductMainImagesAsync(GetProductMainImagesParam param)
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

            var result = param.ProductImageRequests.Select(request => new ProductMainImage
            {
                ImageUrl = GetImageUrl(param.ImageSize, request.ProductId, request.Variant.Id, MainImageSequenceNumber),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl()
            }).ToList();

            return Task.FromResult(result);
        }

        public Task<List<AllProductImages>> GetAllProductImagesAsync(GetAllProductImagesParam param)
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
            return Task.FromResult(result);
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
    }
}

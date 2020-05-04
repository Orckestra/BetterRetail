using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Repositories;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;
using Orckestra.Overture.ServiceModel.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Providers.Dam
{
    public class ConventionBasedDamProvider : IDamProvider
    {
        protected ICdnDamProviderSettings CdnDamProviderSettings { get; private set; }
        protected IProductMediaSettingsRepository ProductMediaSettingsRepository { get; private set; }

        private const string ProductIdFieldName = "{productId}";
        private const string VariantIdFieldName = "{variantId}";
        private const string ImageSizeFieldName = "{imageSize}";
        private const string SequenceNumberFieldName = "{sequenceNumber}";
        private const int MainImageSequenceNumber = 0;
        private MediaSettings _productMediaSettings;

        public ConventionBasedDamProvider(ISiteConfiguration siteConfiguration, IProductMediaSettingsRepository productMediaSettingsRepository)
        {
            CdnDamProviderSettings = siteConfiguration.CdnDamProviderSettings;
            ProductMediaSettingsRepository = productMediaSettingsRepository;
        }

        protected string ServerUrl
        {
            get { return CdnDamProviderSettings.ServerUrl; }
        }

        protected virtual string ImageFolderName
        {
            get { return CdnDamProviderSettings.ImageFolderName; }
        }

        protected virtual string FallbackImageUrl
        {
            get { return CdnDamProviderSettings.FallbackImage; }
        }

        protected virtual bool IsProductZoomImageEnabled
        {
            get { return CdnDamProviderSettings.SupportXLImages; }
        }

        public virtual string GetFallbackImageUrl()
        {
            if (string.IsNullOrEmpty(_productMediaSettings?.MediaFallbackImageName)) return GetLocalFallbackImageUrl();

            return GetMediaFallbackImageUrl(_productMediaSettings);
        }

        public virtual async Task<List<ProductMainImage>> GetProductMainImagesAsync(GetProductMainImagesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ImageSize)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ImageSize)), nameof(param)); }
            if (param.ProductImageRequests == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageRequests)), nameof(param)); }

            if (param.ProductImageRequests.Any(request => string.IsNullOrWhiteSpace(request.ProductId)))
            {
                throw new ArgumentException("The product id must be specified for each ProductImageRequests object.");
            }

            _productMediaSettings = await ProductMediaSettingsRepository.GetProductMediaSettings().ConfigureAwait(false);

            var result = param.ProductImageRequests.Select(request =>
            {
                return request.PropertyBag.ContainsKey("ImageUrl")
                ? GetProductMainMediaImage(request)
                : GetProductMainLocalImage(request, param.ImageSize);
            }).ToList();

            return result;
        }

        public virtual async Task<List<AllProductImages>> GetAllProductImagesAsync(GetAllProductImagesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ImageSize)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ImageSize)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ThumbnailImageSize)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ThumbnailImageSize)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductZoomImageSize)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductZoomImageSize)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductId)), nameof(param)); }

            _productMediaSettings = await ProductMediaSettingsRepository.GetProductMediaSettings().ConfigureAwait(false);

            if (IsProductHaveMedia(param.MediaSet, param.VariantMediaSet, param.Variants)) { return GetAllProductMediaImages(param); }

            return GetAllProductLocalImages(param);
        }

        protected virtual bool IsProductHaveMedia(IList<ProductMedia> mediaSet, IList<VariantMediaSet> variantMediaSet, IList<Variant> variants)
        {
            return (mediaSet?.Any(x => x.MediaType == nameof(MediaTypeEnum.Image)) ?? false)
                || (variantMediaSet?.Any(var => var.Media?.Any(x => x.MediaType == nameof(MediaTypeEnum.Image)) ?? false) ?? false)
                || (variants?.Any(variant => variant.MediaSet?.Any(x => x.MediaType == nameof(MediaTypeEnum.Image)) ?? false) ?? false);
        }

        #region Local Image functions
        protected virtual ProductMainImage GetProductMainLocalImage(ProductImageRequest request, string imageSize)
        {
            return new ProductMainImage
            {
                ImageUrl = GetImageUrl(imageSize, request.ProductId, request.Variant.Id, MainImageSequenceNumber),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl()
            };
        }

        protected virtual List<AllProductImages> GetAllProductLocalImages(GetAllProductImagesParam param)
        {
            var result = new List<AllProductImages>();
            for (var sequenceNumber = 0; sequenceNumber < ConventionBasedDamProviderConfiguration.MaxThumbnailImages; sequenceNumber++)
            {
                result.Add(CreateAllProductImages(param, null, sequenceNumber));

                if (param.Variants != null)
                {
                    result.AddRange(param.Variants.Select(variantKey => CreateAllProductImages(param, variantKey.Id, sequenceNumber)));
                }

            }
            return result;
        }

        protected virtual AllProductImages CreateAllProductImages(GetAllProductImagesParam param, string variantId, int sequenceNumber)
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

        public virtual string GetLocalFallbackImageUrl()
        {
            return GetFormattedImageUrl(FallbackImageUrl);
        }

        protected virtual string GetImageUrl(string imageSize, string productId, string variantId, int sequenceNumber)
        {
            string imagePath;

            if (!string.IsNullOrEmpty(variantId))
            {
                // Resolve the image path for a variant.
                imagePath = CdnDamProviderSettings.VariantImageFilePathPattern
                                                      .Replace(ProductIdFieldName, productId)
                                                      .Replace(VariantIdFieldName, variantId)
                                                      .Replace(SequenceNumberFieldName, sequenceNumber.ToString(CultureInfo.InvariantCulture))
                                                      .Replace(ImageSizeFieldName, imageSize);

            }
            else
            {
                // Resolve the image path for a product.
                imagePath = CdnDamProviderSettings.ProductImageFilePathPattern
                                                      .Replace(ProductIdFieldName, productId)
                                                      .Replace(SequenceNumberFieldName, sequenceNumber.ToString(CultureInfo.InvariantCulture))
                                                      .Replace(ImageSizeFieldName, imageSize);
            }

            return GetFormattedImageUrl(imagePath);
        }

        protected virtual string GetFormattedImageUrl(string imageFilename)
        {
            return string.Format("{0}/{1}/{2}", ServerUrl, ImageFolderName, imageFilename);
        }
        #endregion

        #region MediaSet functions
        protected virtual AllProductImages CreateAllProductImages(ProductMedia productMedia, MediaSettings mediaSettings, GetAllProductImagesParam param, string variantId)
        {
            return new AllProductImages
            {
                ImageUrl = productMedia != null ? GetSizedImageUrl(productMedia, mediaSettings, param.ImageSize) : string.Empty,
                ThumbnailUrl = productMedia != null ? GetSizedImageUrl(productMedia, mediaSettings, param.ThumbnailImageSize) : string.Empty,
                ProductZoomImageUrl = productMedia != null ? GetSizedImageUrl(productMedia, mediaSettings, param.ProductZoomImageSize) : string.Empty,
                ProductId = param.ProductId,
                VariantId = variantId,
                SequenceNumber = productMedia?.Position ?? 0,
                FallbackImageUrl = GetFallbackImageUrl(),
                Alt = productMedia?.Title,
            };
        }

        protected virtual string GetMediaFallbackImageUrl(MediaSettings mediaSettings)
        {
            return mediaSettings.MediaServerUrl + mediaSettings.MediaFallbackImageName;
        }

        protected virtual string GetImageUrl(string imagePath, MediaSettings mediaSettings)
        {
            return imagePath.Replace("~/", mediaSettings.MediaServerUrl);
        }

        protected virtual string GetSizedImageUrl(ProductMedia productMedia, MediaSettings mediaSettings, string size)
        {
            if (productMedia.ResizedInstances != null && productMedia.ResizedInstances.Length > 0 && !string.IsNullOrEmpty(size))
            {
                var resizedImage = productMedia.ResizedInstances.FirstOrDefault(resizedImg => resizedImg.Size == size);

                if (resizedImage != null) return GetImageUrl(resizedImage.Url, mediaSettings);
            }

            return GetImageUrl(productMedia.Url, mediaSettings);
        }

        protected virtual List<AllProductImages> GetAllProductMediaImages(GetAllProductImagesParam param)
        {
            var globalMediaSet = FilterImages(param.MediaSet) ?? new List<ProductMedia>() { null };
            var result = globalMediaSet.Select(productMedia => CreateAllProductImages(productMedia, _productMediaSettings, param, null)).ToList();

            if (param.Variants != null)
            {
                foreach (Variant variant in param.Variants)
                {
                    var variantMediaSet = GetVariantMediaSet(param.VariantMediaSet, variant);
                    var mediaSet = variantMediaSet.Any() ? variantMediaSet : globalMediaSet;
                    result.AddRange(mediaSet.Select(productMedia => CreateAllProductImages(productMedia, _productMediaSettings, param, variant.Id)));
                }
            }
            return result;
        }

        protected virtual IEnumerable<ProductMedia> FilterImages(IEnumerable<ProductMedia> productMedias)
        {
            return productMedias?.Where(x => x.MediaType == nameof(MediaTypeEnum.Image) && x.IsRemoved != true);
        }
           
        protected virtual IEnumerable<ProductMedia> GetVariantMediaSet(List<VariantMediaSet> variantMediaSet, Variant variant)
        {
            if (variant != null)
            {
                var globalVariantMediaSet = new List<ProductMedia>();
                variantMediaSet?.ForEach(mediaVariant =>
                {
                    if (mediaVariant.AttributesToMatch.Any(atribute => variant.PropertyBag.Contains(atribute)))
                    {
                        globalVariantMediaSet.AddRange(FilterImages(mediaVariant.Media));
                    }
                });

                var localVariantMediaSet = FilterImages(variant.MediaSet) ?? new List<ProductMedia>();
                return localVariantMediaSet.Any() ? localVariantMediaSet : globalVariantMediaSet;
            }

            return new List<ProductMedia>();
        }

        public virtual string GetMediaImageUrl(Product product, string variantId)
        {
            if (product == null) return null;

            var variant = !string.IsNullOrEmpty(variantId) ? product.Variants?.Find(v => v.Id.ToLower() == variantId.ToLower()) : null;

            var variantMediaSet = GetVariantMediaSet(product.VariantMediaSet, variant);
            var mediaSet = variantMediaSet.Any() ? variantMediaSet : FilterImages(product.MediaSet);

            return mediaSet?
                .Where(m => m.IsCover == true)
                .Select(m => m.Url)
                .LastOrDefault() ?? (IsProductHaveMedia(product.MediaSet, product.VariantMediaSet, product.Variants) ? string.Empty : null);
        }

        protected virtual ProductMainImage GetProductMainMediaImage(ProductImageRequest request)
        {
            return new ProductMainImage
            {
                ImageUrl = GetImageUrl(request.PropertyBag["ImageUrl"].ToString(), _productMediaSettings),
                ProductId = request.ProductId,
                VariantId = request.Variant.Id,
                FallbackImageUrl = GetFallbackImageUrl()
            };
        }
        #endregion  
    }
}
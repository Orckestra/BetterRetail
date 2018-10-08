using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Util;

namespace Orckestra.Composer.Providers
{
    public abstract class DamProviderBase : IDamProvider
    {
        private readonly CdnDamProviderConfiguration _damProviderConfigurationSection;

        private const string ProductIdFieldName = "{productId}";
        private const string VariantIdFieldName = "{variantId}";
        private const string ImageTypeFieldName = "{imageType}";
        private const string ImageSizeFieldName = "{imageSize}";
        private const string ScopeIdFieldName = "{scopeId}";
        private const string SequenceNumberFieldName = "{sequenceNumber}";
        
        protected DamProviderBase()
        {
            var configurationSection = ConfigurationManager.GetSection(OvertureConfigurationSection.ConfigurationName) as OvertureConfigurationSection;
            const string MissingConfigurationSectionMessage = "Missing configuration section for {0}. Please make sure the configuration section has been properly defined within the configuration file.";
            
            if (configurationSection == null)
            {
                throw new ConfigurationException(string.Format(MissingConfigurationSectionMessage, OvertureConfigurationSection.ConfigurationName));
            }

            if (configurationSection.Composer == null)
            {
                throw new ConfigurationException(string.Format(MissingConfigurationSectionMessage, ComposerConfiguration.ConfigurationName));
            }

            if (configurationSection.Composer.CdnDamProvider == null)
            {
                throw new ConfigurationException(string.Format(MissingConfigurationSectionMessage, CdnDamProviderConfiguration.ConfigurationName));
            }

            _damProviderConfigurationSection = configurationSection.Composer.CdnDamProvider;
        }

        protected abstract string FileLocation { get; }

        protected string ServerUrl
        {
            get { return _damProviderConfigurationSection.ServerUrl; }
        }
        
        protected bool IsHttpsEnabled
        {
            get { return _damProviderConfigurationSection.EnableHttps; }
        }

        protected virtual string ImageFolderName
        {
            get { return _damProviderConfigurationSection.ImageFolderName; }
        }
        
        private static string GetImageSizeAsFirstLetterNotation(ImageSize size)
        {
            var imageSizeName = size.ToString().ToUpper();
            return imageSizeName[0].ToString();
        }

        private string ImageTypeExtension
        {
            get { return _damProviderConfigurationSection.SupportedImageTypeExtension; }
        }

        private int ImagesRetrievalLimit
        {
            get { return _damProviderConfigurationSection.ImagesRetrievalLimit; }
        }

        private string GetImageEndpoint(string scopeId, string productImagePath)
        {
            return string.Format("{0}/{1}/{2}", FileLocation, ImageFolderName.Replace(ScopeIdFieldName, scopeId.ToLower()), productImagePath);
        }

        private ImageFileInfo GetProductImageFileInfo(int sequenceNumber, ProductImageQueryParam queryParam)
        {
            var imageFilePath = _damProviderConfigurationSection.ProductImageFilePathPattern
                                                           .Replace(ProductIdFieldName, queryParam.ProductId)
                                                           .Replace(ImageSizeFieldName, GetImageSizeAsFirstLetterNotation(queryParam.ImageSize))
                                                           .Replace(SequenceNumberFieldName, sequenceNumber.ToString())
                                                           .Replace(ImageTypeFieldName, ImageTypeExtension);
            var result = new ImageFileInfo
            {
                Sequence = sequenceNumber,
                ImageSize = queryParam.ImageSize,
                Location = GetImageEndpoint(queryParam.ScopeId, imageFilePath)
            };
            
            return result;
        }

        private ImageFileInfo GetVariantImageFileInfo(int sequenceNumber, VariantImageQueryParam queryParam)
        {
            var imageFilePath = _damProviderConfigurationSection.VariantImageFilePathPattern
                                                           .Replace(ProductIdFieldName, queryParam.ProductId)
                                                           .Replace(VariantIdFieldName, queryParam.VariantId)
                                                           .Replace(ImageSizeFieldName, GetImageSizeAsFirstLetterNotation(queryParam.ImageSize))
                                                           .Replace(SequenceNumberFieldName, sequenceNumber.ToString())
                                                           .Replace(ImageTypeFieldName, ImageTypeExtension);
            var result = new ImageFileInfo
            {
                Sequence = sequenceNumber,
                ImageSize = queryParam.ImageSize,
                Location = GetImageEndpoint(queryParam.ScopeId, imageFilePath)
            };

            return result;
        }

        public Task<IList<Dictionary<ImageSize, ImageFileInfo>>> GetProductImagesLocationsAsync(ProductImageQueryParam queryParam)
        {
            IList<Dictionary<ImageSize, ImageFileInfo>> result = new List<Dictionary<ImageSize, ImageFileInfo>>();

            for (var imageSequenceNumber = 0; imageSequenceNumber < ImagesRetrievalLimit; imageSequenceNumber++)
            {
                var dictionary = new Dictionary<ImageSize, ImageFileInfo>();

                if (queryParam.ImageSize == ImageSize.All)
                {
                    var newQueryParam = new ProductImageQueryParam()
                    {
                        ImageSize = queryParam.ImageSize,
                        ProductId = queryParam.ProductId,
                        ScopeId = queryParam.ScopeId
                    };

                    // Taking care of the small image size.
                    newQueryParam.ImageSize = ImageSize.Small;
                    var smallProductImageFileInfo = GetProductImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Small, smallProductImageFileInfo);

                    // Taking care of the medium image size.
                    newQueryParam.ImageSize = ImageSize.Medium;
                    var mediumProductImageFileInfo = GetProductImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Medium, mediumProductImageFileInfo);

                    // Taking care of the large image size.
                    newQueryParam.ImageSize = ImageSize.Large;
                    var largeProductImageFileInfo = GetProductImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Large, largeProductImageFileInfo);
                }
                else
                {
                    // Simply add the product image at the specified image size.
                    var productImageFileInfo = GetProductImageFileInfo(imageSequenceNumber, queryParam);
                    dictionary.Add(queryParam.ImageSize, productImageFileInfo);
                }

                result.Add(dictionary);
            }
            return Task.FromResult(result);
        }

        public Task<IList<Dictionary<ImageSize, ImageFileInfo>>> GetVariantImagesLocationsAsync(VariantImageQueryParam queryParam)
        {
            IList<Dictionary<ImageSize, ImageFileInfo>> result = new List<Dictionary<ImageSize, ImageFileInfo>>();

            for (var imageSequenceNumber = 0; imageSequenceNumber < ImagesRetrievalLimit; imageSequenceNumber++)
            {
                var dictionary = new Dictionary<ImageSize, ImageFileInfo>();

                if (queryParam.ImageSize == ImageSize.All)
                {
                    var newQueryParam = new VariantImageQueryParam()
                    {
                        ImageSize = queryParam.ImageSize,
                        ProductId = queryParam.ProductId,
                        VariantId = queryParam.VariantId,
                        ScopeId = queryParam.ScopeId
                    };

                    // Taking care of the small image size.
                    newQueryParam.ImageSize = ImageSize.Small;
                    var smallVariantImageFileInfo = GetVariantImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Small, smallVariantImageFileInfo);

                    // Taking care of the medium image size.
                    newQueryParam.ImageSize = ImageSize.Medium;
                    var mediumVariantImageFileInfo = GetVariantImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Medium, mediumVariantImageFileInfo);

                    // Taking care of the large image size.
                    newQueryParam.ImageSize = ImageSize.Large;
                    var largeVariantImageFileInfo = GetVariantImageFileInfo(imageSequenceNumber, newQueryParam);
                    dictionary.Add(ImageSize.Large, largeVariantImageFileInfo);
                }
                else
                {
                    // Simply add the product image at the specified image size.
                    var variantImageFileInfo = GetVariantImageFileInfo(imageSequenceNumber, queryParam);
                    dictionary.Add(queryParam.ImageSize, variantImageFileInfo);
                }

                result.Add(dictionary);
            }
            return Task.FromResult(result);
        }
    }
}
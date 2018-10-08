using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public class CdnDamProviderConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "cdnDamProvider";

        const string ServerUrlPropertyName = "serverUrl";
        [ConfigurationProperty(ServerUrlPropertyName, IsRequired = true)]
        public string ServerUrl
        {
            get { return this[ServerUrlPropertyName] as string; }
            set { this[ServerUrlPropertyName] = value; }
        }

        const string ImageFolderNamePropertyName = "imageFolderName";
        [ConfigurationProperty(ImageFolderNamePropertyName)]
        public string ImageFolderName
        {
            get { return this[ImageFolderNamePropertyName] as string; }
            set { this[ImageFolderNamePropertyName] = value; }
        }

        const string ProductImageFilePathPatternPropertyName = "productImageFilePathPattern";
        [ConfigurationProperty(ProductImageFilePathPatternPropertyName, DefaultValue = "{productId}_{sequenceNumber}_{imageSize}.jpg")]
        public string ProductImageFilePathPattern
        {
            get { return this[ProductImageFilePathPatternPropertyName] as string; }
            set { this[ProductImageFilePathPatternPropertyName] = value; }
        }

        const string VariantImageFilePathPatternPropertyName = "variantImageFilePathPattern";
        [ConfigurationProperty(VariantImageFilePathPatternPropertyName, DefaultValue = "{productId}_{variantId}_{sequenceNumber}_{imageSize}.jpg")]
        public string VariantImageFilePathPattern
        {
            get { return this[VariantImageFilePathPatternPropertyName] as string; }
            set { this[VariantImageFilePathPatternPropertyName] = value; }
        }

        const string FallbackImagePropertyName = "fallbackImage";
        [ConfigurationProperty(FallbackImagePropertyName, DefaultValue = "image_not_found.jpg")]
        public string FallbackImage
        {
            get { return (string)this[FallbackImagePropertyName]; }
            set { this[FallbackImagePropertyName] = value; }
        }

        const string SupportExtraLargeImagePropertyName = "supportXL";
        [ConfigurationProperty(SupportExtraLargeImagePropertyName, DefaultValue = true)]        
        public bool SupportXLImages
        {
            get { return (bool)this[SupportExtraLargeImagePropertyName]; }
            set { this[SupportExtraLargeImagePropertyName] = value; }
        }
    }
}

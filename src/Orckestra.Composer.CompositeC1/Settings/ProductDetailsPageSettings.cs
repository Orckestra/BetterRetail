using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;
using System;
using System.Globalization;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class ProductDetailsPageSettings : IProductDetailsPageSettings
    {
        protected IMediaService MediaService { get; private set; }

        private readonly Lazy<DataTypes.ProductDetailsPageSettings> _productDetailsPageSettingsMeta;
        private DataTypes.ProductDetailsPageSettings ProductDetailsPageSettingsMeta => _productDetailsPageSettingsMeta.Value;

        public ProductDetailsPageSettings(IWebsiteContext websiteContext, IMediaService mediaService)
        {
            _productDetailsPageSettingsMeta = new Lazy<DataTypes.ProductDetailsPageSettings>(websiteContext.GetRootPageMetaData<DataTypes.ProductDetailsPageSettings>);
            MediaService = mediaService;
        }

        public bool VideosInSummaryEnabled => ProductDetailsPageSettingsMeta?.VideosInSummaryEnabled ?? false;
        public string DefaultVideoThumbnail
        {
            get
            {
                var mediaString = ProductDetailsPageSettingsMeta?.DefaultVideoThumbnail ?? null;
                var imageInfo = MediaService.GetImageInfo(mediaString, CultureInfo.CurrentUICulture);

                return imageInfo.Url;
            }
        }
    }
}
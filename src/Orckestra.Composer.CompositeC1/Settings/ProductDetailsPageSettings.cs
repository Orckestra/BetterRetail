using System;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class ProductDetailsPageSettings : IProductDetailsPageSettings
    {
        private readonly Lazy<DataTypes.ProductDetailsPageSettings> _productDetailsPageSettingsMeta;
        private DataTypes.ProductDetailsPageSettings ProductDetailsPageSettingsMeta => _productDetailsPageSettingsMeta.Value;

        public ProductDetailsPageSettings(IWebsiteContext websiteContext)
        {
            _productDetailsPageSettingsMeta = new Lazy<DataTypes.ProductDetailsPageSettings>(websiteContext.GetRootPageMetaData<DataTypes.ProductDetailsPageSettings>);
        }

        public bool VideosInSummaryEnabled => ProductDetailsPageSettingsMeta?.VideosInSummaryEnabled ?? false;
    }
}
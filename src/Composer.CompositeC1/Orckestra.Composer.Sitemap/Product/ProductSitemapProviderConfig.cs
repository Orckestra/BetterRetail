using Orckestra.Composer.Sitemap.Config;
using System;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapProviderConfig : ISitemapProviderConfig
    {
        private const string Default_SitemapFilePrefix = "products";

        public string SitemapFilePrefix
        {
            get
            {
                return ExtractSettingFromProductSitemapConfiguration((config) => config.SitemapFilePrefix, Default_SitemapFilePrefix);
            }
        }

        private static T ExtractSettingFromProductSitemapConfiguration<T>(Func<ProductSitemapConfiguration, T> extractor, T defaultValue = default)
        {
            var productSitemapConfig = SitemapConfiguration.Instance?.ProductSitemapConfiguration;

            return productSitemapConfig != null ? extractor(productSitemapConfig) : defaultValue;
        }
    }
}

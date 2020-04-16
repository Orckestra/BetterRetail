using Orckestra.Composer.CompositeC1.Sitemap;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapProvider : SitemapProvider
    {
        public ProductSitemapProvider(ProductSitemapEntryProvider entryProvider, ProductSitemapProviderConfig config, IC1SitemapConfiguration mainConfig) 
            : base(entryProvider, config, mainConfig)
        {
        }
    }
}

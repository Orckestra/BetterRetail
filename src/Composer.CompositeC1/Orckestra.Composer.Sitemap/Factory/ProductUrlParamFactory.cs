using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel;
using System.Globalization;

namespace Orckestra.Composer.Sitemap.Factory
{
    public class ProductUrlParamFactory : IProductUrlParamFactory
    {
        public virtual GetProductUrlParam GetProductUrlParams(SitemapParams sitemapParams, CultureInfo culture, PropertyBag propertyBag)
        {
            return new WebsiteGetProductUrlParam
            {
                WebsiteId = sitemapParams.Website,
                CultureInfo = culture,
                ProductId = (string)propertyBag["ProductId"],
                ProductName = (string)propertyBag["DisplayName"],
                SKU = (string)propertyBag["Sku"]
            };
        }
    }
}

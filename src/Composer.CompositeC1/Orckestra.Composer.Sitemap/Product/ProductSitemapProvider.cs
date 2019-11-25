using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapProvider : SitemapProvider
    {
        public ProductSitemapProvider(ProductSitemapEntryProvider entryProvider, ProductSitemapNamer namer, ProductSitemapProviderConfig config) 
            : base(entryProvider, namer, config)
        {
        }
    }
}

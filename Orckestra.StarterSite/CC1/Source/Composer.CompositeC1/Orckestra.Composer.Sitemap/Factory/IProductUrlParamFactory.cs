using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Sitemap.Factory
{
    public interface IProductUrlParamFactory
    {
        GetProductUrlParam GetProductUrlParams(SitemapParams sitemapParams, CultureInfo culture, PropertyBag propertyBag);
    }
}

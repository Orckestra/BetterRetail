using Orckestra.Composer.Sitemap;
using Orckestra.Composer.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public interface IC1ContentSitemapDataTypesIncluder
    {
        IEnumerable<SitemapEntry> GetEntries(SitemapParams sitemapParams, CultureInfo culture);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public interface ISitemapProvider
    {
        IEnumerable<Models.Sitemap> GenerateSitemaps(SitemapParams sitemapParams, CultureInfo culture);

        bool IsMatch(string sitemapFilename);
    }
}

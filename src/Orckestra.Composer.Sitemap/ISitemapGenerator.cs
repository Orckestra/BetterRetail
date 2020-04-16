using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public interface ISitemapGenerator
    {
        void GenerateSitemaps(Guid website, string baseUrl, string baseSitemapUrl, params CultureInfo[] cultures);
    }
}

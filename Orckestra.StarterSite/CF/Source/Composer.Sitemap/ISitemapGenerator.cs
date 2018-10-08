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
        void GenerateSitemaps(string baseUrl, string baseSitemapUrl, string scope, params CultureInfo[] cultures);
    }
}

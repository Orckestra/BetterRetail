using Orckestra.Composer.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public interface ISitemapIndexGenerator
    {
        SitemapIndex Generate(string baseSitemapUrl, IEnumerable<string> sitemapNames);
    }
}

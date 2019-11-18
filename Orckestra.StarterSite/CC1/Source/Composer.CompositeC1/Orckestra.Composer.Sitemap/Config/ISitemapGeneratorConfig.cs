using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Config
{
    public interface ISitemapGeneratorConfig
    {
        string GetSitemapDirectory(SitemapParams sitemapParams);

        string GetSitemapIndexDirectory(SitemapParams sitemapParams);

        string GetWorkingDirectory(SitemapParams sitemapParams);
    }
}

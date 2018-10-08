using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Config
{
    public interface ISitemapGeneratorConfig
    {
        string SitemapDirectory { get; }

        string SitemapIndexDirectory { get; }

        string WorkingDirectory { get; }
    }
}

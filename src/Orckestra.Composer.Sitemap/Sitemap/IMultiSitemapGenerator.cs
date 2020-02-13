using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public interface IMultiSitemapGenerator
    {
        SitemapResponse GenerateSitemaps();
    }
}

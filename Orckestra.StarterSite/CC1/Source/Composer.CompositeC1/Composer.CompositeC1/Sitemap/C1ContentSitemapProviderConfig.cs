using Orckestra.Composer.Sitemap.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapProviderConfig : ISitemapProviderConfig
    {
        public int NumberOfEntriesPerSitemap
        {
            get
            {
                // We assume that all the content in Composite C1 can be included in 1 sitemap
                // So returning int.MaxValue should be fine.
                return int.MaxValue;
            }
        }
    }
}

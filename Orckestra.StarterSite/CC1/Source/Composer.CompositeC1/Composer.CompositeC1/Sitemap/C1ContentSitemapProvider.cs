using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Sitemap.Config;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapProvider : SitemapProvider
    {
        public C1ContentSitemapProvider(C1ContentSitemapEntryProvider entryProvider, C1ContentSitemapNamer namer, C1ContentSitemapProviderConfig config) 
            : base(entryProvider, namer, config)
        {
        }
    }
}

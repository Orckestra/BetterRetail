using Orckestra.Composer.Sitemap;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapProvider : SitemapProvider
    {
        public C1ContentSitemapProvider(C1ContentSitemapEntryProvider entryProvider, C1ContentSitemapProviderConfig config, IC1SitemapConfiguration mainConfig) 
            : base(entryProvider, config, mainConfig)
        {
        }
    }
}

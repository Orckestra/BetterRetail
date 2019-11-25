using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapProvider : ISitemapProvider
    {
        private readonly ISitemapEntryProvider _entryProvider;
        private readonly ISitemapNamer _namer;
        private readonly int _numberOfEntriesPerSitemap;

        public SitemapProvider(ISitemapEntryProvider entryProvider, ISitemapNamer namer, ISitemapProviderConfig config)
        {
            Guard.NotNull(entryProvider, nameof(entryProvider));
            Guard.NotNull(namer, nameof(namer));            
            Guard.NotNull(config, nameof(config));
            if (config.NumberOfEntriesPerSitemap < 1) throw new ArgumentException("Must be greater than zero.", nameof(config.NumberOfEntriesPerSitemap));

            _entryProvider = entryProvider;
            _namer = namer;
            _numberOfEntriesPerSitemap = config.NumberOfEntriesPerSitemap;
        }

        public ISitemapNamer Namer
        {
            get { return _namer; }
        }

        public IEnumerable<Models.Sitemap> GenerateSitemaps(SitemapParams sitemapParams, CultureInfo culture)
        {
            Guard.NotNullOrWhiteSpace(sitemapParams.BaseUrl, nameof(sitemapParams.BaseUrl));
            Guard.NotNullOrWhiteSpace(sitemapParams.Scope, nameof(sitemapParams.Scope));
            Guard.NotNull(culture, nameof(culture));

            var iterationIndex = 1;
            var offset = 0;
            var done = false;

            do
            {                
                var entries = _entryProvider.GetEntriesAsync(
                    sitemapParams,
                    culture: culture, 
                    offset: offset, 
                    count: _numberOfEntriesPerSitemap
                ).Result;

                if (entries.Any())
                {
                    yield return new Models.Sitemap
                    {
                        Name = _namer.GetSitemapName(culture, iterationIndex),
                        Entries = entries.ToArray(),
                    };

                    offset = offset + _numberOfEntriesPerSitemap;
                    iterationIndex = iterationIndex + 1;

                    if (entries.Count() != _numberOfEntriesPerSitemap)
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }
            }
            while (!done);
        }
    }
}

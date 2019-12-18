using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using Orckestra.Composer.CompositeC1.Sitemap;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapProvider : ISitemapProvider
    {
        public ISitemapEntryProvider EntryProvider { get; }

        public int NumberOfEntriesPerSitemap { get; }
        public string SitemapFilePrefix { get; }

        public SitemapProvider(ISitemapEntryProvider entryProvider, ISitemapProviderConfig config, IC1SitemapConfiguration mainConfig)
        {
            Guard.NotNull(entryProvider, nameof(entryProvider));
            Guard.NotNull(config, nameof(config));
            if (mainConfig.NumberOfEntriesPerFile < 1) throw new ArgumentException("Must be greater than zero.", nameof(mainConfig.NumberOfEntriesPerFile));

            EntryProvider = entryProvider;
            NumberOfEntriesPerSitemap = mainConfig.NumberOfEntriesPerFile;
            SitemapFilePrefix = config.SitemapFilePrefix;
        }

        public IEnumerable<Models.Sitemap> GenerateSitemaps(SitemapParams sitemapParams, CultureInfo culture)
        {
            Guard.NotNullOrWhiteSpace(sitemapParams.BaseUrl, nameof(sitemapParams.BaseUrl));
            Guard.NotNullOrWhiteSpace(sitemapParams.Scope, nameof(sitemapParams.Scope));
            Guard.NotNull(culture, nameof(culture));

            var iterationIndex = 1;
            var offset = 0;

            do
            {
                var entries = EntryProvider.GetEntriesAsync(
                    sitemapParams,
                    culture: culture,
                    offset: offset,
                    count: NumberOfEntriesPerSitemap
                ).Result;

                var isEntriesNotEnough = entries.Count() < NumberOfEntriesPerSitemap;

                if (entries.Any())
                {
                    yield return new Models.Sitemap
                    {
                        Name = isEntriesNotEnough && iterationIndex == 1 ? GetSitemapName(culture) : GetSitemapName(culture, iterationIndex),
                        Entries = entries.ToArray(),
                    };

                    offset += NumberOfEntriesPerSitemap;
                    iterationIndex += 1;

                    if (isEntriesNotEnough)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        public virtual bool IsMatch(string sitemapFilename)
        {
            if (sitemapFilename == null)
            {
                return false;
            }

            // Source: http://stackoverflow.com/questions/3962543/how-can-i-validate-a-culture-code-with-a-regular-expression
            var cultureRegex = "[a-z]{2,3}(?:-[A-Z]{2,3}(?:-(?:Cyrl|Latn))?)?";

            return Regex.IsMatch(sitemapFilename, $@"sitemap-{cultureRegex}-{SitemapFilePrefix}");
        }

        protected virtual string GetSitemapName(CultureInfo culture, int index)
        {
            return $"sitemap-{culture.Name}-{SitemapFilePrefix}-{index}.xml";
        }

        protected virtual string GetSitemapName(CultureInfo culture)
        {
            return $"sitemap-{culture.Name}-{SitemapFilePrefix}.xml";
        }
    }
}

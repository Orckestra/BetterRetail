using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Sitemap.Config;

using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapProvider : ISitemapProvider
    {
        public ISitemapEntryProvider EntryProvider { get; }

        public int NumberOfEntriesPerSitemap { get; }
        public string SitemapFilePrefix { get; }

        public SitemapProvider(ISitemapEntryProvider entryProvider, ISitemapProviderConfig config, IC1SitemapConfiguration param)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (param.NumberOfEntriesPerFile < 1) 
                throw new ArgumentOutOfRangeException(nameof(param), param.NumberOfEntriesPerFile, GetMessageOfZeroNegative(nameof(param.NumberOfEntriesPerFile)));

            EntryProvider = entryProvider;
            NumberOfEntriesPerSitemap = param.NumberOfEntriesPerFile;
            SitemapFilePrefix = config.SitemapFilePrefix;
        }

        public IEnumerable<Models.Sitemap> GenerateSitemaps(SitemapParams param)
        {
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.Culture == null) {throw new ArgumentException(GetMessageOfNull(nameof(param.Culture)), nameof(param)); }

            var iterationIndex = 1;
            var offset = 0;

            do
            {
                var entries = EntryProvider.GetEntriesAsync(
                    param,
                    culture: param.Culture,
                    offset: offset,
                    count: NumberOfEntriesPerSitemap
                ).Result;

                var isEntriesNotEnough = entries.Count() < NumberOfEntriesPerSitemap;

                if (entries.Any())
                {
                    yield return new Models.Sitemap
                    {
                        Name = isEntriesNotEnough && iterationIndex == 1 ? GetSitemapName(param.Culture) : GetSitemapName(param.Culture, iterationIndex),
                        Entries = entries.ToArray(),
                    };

                    offset += NumberOfEntriesPerSitemap;
                    iterationIndex += 1;

                    if (isEntriesNotEnough) { break; }
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        //TODO: fix. If a code will be iu-Cans-CA? Also, Cyrl|Latn etc part will be in the middle
        public virtual bool IsMatch(string sitemapFilename)
        {
            if (sitemapFilename == null) { return false; }
            
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
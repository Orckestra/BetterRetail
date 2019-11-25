using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Sitemap.Models;
using System.Globalization;
using System.IO;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapIndexGenerator : ISitemapIndexGenerator
    {
        public SitemapIndex Generate(string baseSitemapUrl, IEnumerable<string> sitemapNames)
        {
            var lastModification = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            var entries = sitemapNames.Select(sitemapName =>
            {   
                var location = new Uri(Path.Combine(baseSitemapUrl, sitemapName)).ToString();                

                return new SitemapIndexEntry
                {
                    Location = location,
                    LastModification = lastModification,
                };
            });

            return new SitemapIndex
            {
                Entries = entries.ToArray(),
            };
        }
    }
}

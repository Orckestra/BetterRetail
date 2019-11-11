using Orckestra.Composer.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public interface ISitemapEntryProvider
    {
        Task<IEnumerable<SitemapEntry>> GetEntriesAsync(SitemapParams sitemap, CultureInfo culture, int offset, int count);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public interface IC1ContentSitemapPageExcludeProvider
    {
        IEnumerable<Guid> GetPageIdsToExclude(Guid websiteId, CultureInfo culture);
    }
}

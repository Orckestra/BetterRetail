using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap
{
    public interface ISitemapNamer
    {
        bool IsMatch(string sitemapFilename);

        string GetSitemapName(CultureInfo culture, int index);
    }
}

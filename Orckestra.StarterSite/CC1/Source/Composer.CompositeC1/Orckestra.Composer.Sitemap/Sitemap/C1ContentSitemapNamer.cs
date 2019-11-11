using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapNamer : ISitemapNamer
    {
        public string GetSitemapName(CultureInfo culture, int index)
        {
            return $"sitemap-{culture.Name}-content.xml";
        }

        public bool IsMatch(string sitemapFilename)
        {
            if (sitemapFilename == null)
            {
                return false;
            }

            // Source: http://stackoverflow.com/questions/3962543/how-can-i-validate-a-culture-code-with-a-regular-expression
            var cultureRegex = "[a-z]{2,3}(?:-[A-Z]{2,3}(?:-(?:Cyrl|Latn))?)?";
            return Regex.IsMatch(sitemapFilename, $@"sitemap-{cultureRegex}-content.xml");
        }
    }
}

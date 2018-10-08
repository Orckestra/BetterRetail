using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapNamer : ISitemapNamer
    {
        public bool IsMatch(string sitemapFilename)
        {
            if (sitemapFilename == null)
            {
                return false;
            }

            // Source: http://stackoverflow.com/questions/3962543/how-can-i-validate-a-culture-code-with-a-regular-expression
            var cultureRegex = "[a-z]{2,3}(?:-[A-Z]{2,3}(?:-(?:Cyrl|Latn))?)?";
            return Regex.IsMatch(sitemapFilename, $@"sitemap-{cultureRegex}-products-(\d+).xml");
        }

        public string GetSitemapName(CultureInfo culture, int index)
        {
            return $"sitemap-{culture.Name}-products-{index}.xml";
        }
    }
}

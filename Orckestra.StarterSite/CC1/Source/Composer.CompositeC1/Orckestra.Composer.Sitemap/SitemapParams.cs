using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Configuration;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapParams
    {
        public Guid Website { get; set; }
        public string Scope { get; set; }
        public string BaseUrl { get; set; }
}
}

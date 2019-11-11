using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class WebsiteGetProductUrlParam : GetProductUrlParam
    {
        public Guid WebsiteId { get; set; }
    }
}

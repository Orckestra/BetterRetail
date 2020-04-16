using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Config
{
    public class ProductSitemapConfiguration : ConfigurationElement
    {
        internal const string ConfigurationName = "productSitemap";

        const string SitemapFilePrefixKey = "sitemapFilePrefix";
        [ConfigurationProperty(SitemapFilePrefixKey, IsRequired = true)]
        [StringValidator]
        public string SitemapFilePrefix
        {            
            get { return (string)this[SitemapFilePrefixKey]; }
            set { this[SitemapFilePrefixKey] = value; }            
        }
    }
}

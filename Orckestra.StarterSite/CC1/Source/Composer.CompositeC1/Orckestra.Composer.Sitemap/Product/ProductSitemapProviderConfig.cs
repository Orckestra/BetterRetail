using Orckestra.Composer.Configuration;
using Orckestra.Composer.Sitemap.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapProviderConfig : ISitemapProviderConfig
    {
        private const int Default_NumberOfEntriesPerSitemap = 2500;

        public int NumberOfEntriesPerSitemap
        {
            get
            {
                var conf = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;

                if (conf.SitemapConfiguration != null && conf.SitemapConfiguration.ProductSitemapConfiguration != null)
                {
                    return conf.SitemapConfiguration.ProductSitemapConfiguration.NumberOfEntriesPerFile;
                }
                
                // Default value;
                return Default_NumberOfEntriesPerSitemap;
            }
        }
    }
}

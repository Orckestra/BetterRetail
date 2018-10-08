using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Configuration
{
    public class ProductSitemapConfiguration : ConfigurationElement
    {
        internal const string ConfigurationName = "productSitemap";

        const string NumberOfEntriesPerFileKey = "numberOfEntriesPerFile";
        [ConfigurationProperty(NumberOfEntriesPerFileKey, IsRequired = true)]
        [IntegerValidator]
        public int NumberOfEntriesPerFile
        {            
            get { return (int)this[NumberOfEntriesPerFileKey]; }
            set { this[NumberOfEntriesPerFileKey] = value; }            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Orckestra.Composer.Sitemap.Models
{
    public class SitemapIndexEntry
    {
        [XmlElement(ElementName = "loc")]
        public string Location { get; set; }

        [XmlElement(ElementName = "lastmod")]
        public string LastModification { get; set; }
    }
}

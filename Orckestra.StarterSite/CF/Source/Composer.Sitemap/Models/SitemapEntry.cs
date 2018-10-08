using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Orckestra.Composer.Sitemap.Models
{    
    public class SitemapEntry
    {
        [XmlElement(ElementName = "loc")]
        public string Location { get; set; }

        [XmlElement(ElementName = "lastmod")]
        public string LastModification { get; set; }

        [XmlElement(ElementName = "changefreq")]
        public string ChangeFrequency { get; set; }

        [XmlElement(ElementName = "priority")]
        public string Priority { get; set; }
    }
}

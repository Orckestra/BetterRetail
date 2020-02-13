using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Orckestra.Composer.Sitemap.Models
{
    [XmlRoot(ElementName = RootElementName, Namespace = Namespace)]
    public class Sitemap : SitemapNamespace
    {
        public const string RootElementName = "urlset";

        [XmlIgnore]
        public string Name { get; set; }

        [XmlElement(ElementName = "url")]
        public SitemapEntry[] Entries { get; set; }
    }
}

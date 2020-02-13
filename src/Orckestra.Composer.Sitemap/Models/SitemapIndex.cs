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
    public class SitemapIndex : SitemapNamespace
    {
        public const string RootElementName = "sitemapindex";
       
        [XmlElement(ElementName = "sitemap")]
        public SitemapIndexEntry[] Entries { get; set; }
    }
}

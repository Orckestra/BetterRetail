using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Orckestra.Composer.Sitemap.Models
{
    public abstract class SitemapNamespace
    {
        public const string Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        private readonly XmlSerializerNamespaces _namespaces;

        public SitemapNamespace()
        {
            // NOTE (SIMON.BERUBE)
            // Specifying namespace and settings to ensure that the following namespaces are not included in the XML document :
            //   - xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
            //   - xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
            // Source: http://stackoverflow.com/questions/760262/xmlserializer-remove-unnecessary-xsi-and-xsd-namespaces
            _namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new XmlQualifiedName(string.Empty, Namespace),
            });
        }

        // NOTE (SIMON.BERUBE):
        // Specifying namespace and settings to ensure that the following namespaces are not included in the XML document :
        //   - xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
        //   - xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
        // Source: http://stackoverflow.com/questions/760262/xmlserializer-remove-unnecessary-xsi-and-xsd-namespaces
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces
        {
            get { return this._namespaces; }
        }
    }
}

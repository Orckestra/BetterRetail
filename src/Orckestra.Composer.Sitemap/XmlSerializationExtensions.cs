using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Orckestra.Composer.Sitemap.Models
{
    public static class SitemapExtensions
    {
        public static void WriteToXml(this Sitemap sitemap, string filePath)
        {
            WriteToXml(Sitemap.RootElementName, SitemapNamespace.Namespace, filePath, sitemap, sitemap.Namespaces);
        }

        public static void WriteToXml(this SitemapIndex sitemapIndex, string filePath)
        {
            WriteToXml(SitemapIndex.RootElementName, SitemapNamespace.Namespace, filePath, sitemapIndex, sitemapIndex.Namespaces);
        }

        private static void WriteToXml(string rootElementName, 
            string @namespace, 
            string filePath, 
            object content, 
            XmlSerializerNamespaces namespaces)
        {
            var serializer = new XmlSerializer(
                content.GetType(),
                 new XmlRootAttribute(rootElementName) { Namespace = @namespace }
            );

            // NOTE (SIMON.BERUBE):
            // Specifying namespace and settings to ensure that the following namespaces are not included in the XML document :
            //   - xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
            //   - xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
            // Source: http://stackoverflow.com/questions/760262/xmlserializer-remove-unnecessary-xsi-and-xsd-namespaces
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
            };

            using (var writer = XmlWriter.Create(filePath, settings))
            {
                serializer.Serialize(writer, content, namespaces);
            }
        }
    }
}

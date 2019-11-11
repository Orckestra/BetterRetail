using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Orckestra.Composer.Sitemap.Tests
{
    internal static class XsdValidator
    {
        // Source: http://stackoverflow.com/questions/10025986/validate-xml-against-xsd-in-a-single-method
        public static bool IsValidXml(string xmlFilePath, string xsdFilePath, string @namespace)
        {
            var xdoc = XDocument.Load(xmlFilePath);
            var schemas = new XmlSchemaSet();
            schemas.Add(@namespace, xsdFilePath);

            bool isValid = true;
            xdoc.Validate(schemas, (sender, e) =>
            {
                if (e.Severity == XmlSeverityType.Error || e.Severity == XmlSeverityType.Warning)
                {
                    Trace.WriteLine($"Line: {e.Exception.LineNumber}, Position: {e.Exception.LinePosition} \"{e.Exception.Message}\"");
                }

                isValid = false;
            });

            return isValid;
        }

    }
}

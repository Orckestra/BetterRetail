using System.IO;
using System.Linq;
using NUnit.Framework;
using Orckestra.Composer.Sitemap.Models;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class Sitemap_Serialize
    {
        [Test]
        public void WHEN_SitemapIsSerialized_SHOULD_RespectSitemapXsd()
        {
            // ARRANGE  
            var xsdResourceName = "Orckestra.Composer.Sitemap.Tests.sitemap.xsd";
            var targetNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var xmlFilePath = Path.GetTempFileName();
            var sitemap = CreateSitemap();
            
            try
            {
                // ACT
                sitemap.WriteToXml(xmlFilePath);

                // ASSERT
                Assert.DoesNotThrow(() => XsdValidator.ValidateXml(xmlFilePath, xsdResourceName, targetNamespace));
            }
            finally
            {
                File.Delete(xmlFilePath);
            }   
        }

        private static Models.Sitemap CreateSitemap()
        {
            return new Models.Sitemap
            {
                Name = "sitemap",
                Entries = Enumerable.Range(0, 10).Select(i => new SitemapEntry
                {
                    Location = "http://orckestra.dev.local/product.html",
                    Priority = "0.5",
                    LastModification = "2005-05-10T17:33:30+08:00",
                    ChangeFrequency = "daily"
                }).ToArray()
            };
        }
    }
}

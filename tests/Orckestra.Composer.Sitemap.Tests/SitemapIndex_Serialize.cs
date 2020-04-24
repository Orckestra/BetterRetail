using System.IO;
using System.Linq;
using NUnit.Framework;
using Orckestra.Composer.Sitemap.Models;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class SitemapIndex_Serialize
    {
        [Test]
        public void WHEN_SitemapIndexIsSerialized_SHOULD_RespectSitemapXsd()
        {
            // ARRANGE            
            var xsdResourceName = "Orckestra.Composer.Sitemap.Tests.siteindex.xsd"; 
            var targetNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var xmlFilePath = Path.GetTempFileName();
            var sitemapIndex = CreateSitemapIndex();

            try
            {
                // ACT
                sitemapIndex.WriteToXml(xmlFilePath);

                // ASSERT
                Assert.DoesNotThrow(()=> XsdValidator.ValidateXml(xmlFilePath, xsdResourceName, targetNamespace));
            }
            finally
            {
                File.Delete(xmlFilePath);
            }
        }

        private static SitemapIndex CreateSitemapIndex()
        {
            return new SitemapIndex
            {   
                Entries = Enumerable.Range(0, 10).Select(i => new SitemapIndexEntry
                {
                    Location = "http://orckestra.dev.local/product.html",                    
                    LastModification = "2005-05-10T17:33:30+08:00",                    
                }).ToArray()
            };
        }
    }
}

using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class Sitemap_Serialize
    {
        [Test]
        public void WHEN_SitemapIsSerialized_SHOULD_RespectSitemapXsd()
        {
            // ARRANGE            
            var urlSitemapXsd = "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";
            var @namespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var filePath = Path.GetTempFileName();
            var sitemap = CreateSitemap();
            
            try
            {
                // ACT
                sitemap.WriteToXml(filePath);

                // ASSERT
                XsdValidator.IsValidXml(filePath, urlSitemapXsd, @namespace).Should().BeTrue("Validation against XSD failed. Check test output for more details.");
            }
            finally
            {
                File.Delete(filePath);
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

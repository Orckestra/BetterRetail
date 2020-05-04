using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class SitemapIndexGenerator_Generate
    {
        private AutoMocker _container;

        [SetUp]
        public void Init()
        {
            _container = new AutoMocker();

        }

        [Test]
        public void WHEN_BaseUrlEndWithSlash_SHOULD_LocationBeValidUri()
        {
            // ARRANGE
            var baseSitemapUrl = "https://hostname.com/baseUrl/";            
            var sitemapNames = new[] { "sitemap1" };
            var sut = _container.CreateInstance<SitemapIndexGenerator>();

            // ACT
            var sitemapIndex = sut.Generate(baseSitemapUrl, sitemapNames);

            // ASSERT
            IsLocationValid(sitemapIndex.Entries.First().Location).Should().BeTrue();
        }

        [Test]
        public void WHEN_BaseUrlDoesntEndWithSlash_SHOULD_LocationBeValidUri()
        {
            // ARRANGE
            var baseSitemapUrl = "https://hostname.com/baseUrl";            
            var sitemapNames = new[] { "sitemap1" };
            var sut = _container.CreateInstance<SitemapIndexGenerator>();

            // ACT
            var sitemapIndex = sut.Generate(baseSitemapUrl, sitemapNames);

            // ASSERT
            IsLocationValid(sitemapIndex.Entries.First().Location).Should().BeTrue();
        }

        [Test]
        public void WHEN_MultipleSitename_SHOULD_CreateIndexWithCorrectEntries()
        {
            // ARRANGE
            var baseSitemapUrl = "https://hostname.com/baseUrl/";            
            var sitemapNames = new[] { "sitemap1", "sitemap2" };
            var sut = _container.CreateInstance<SitemapIndexGenerator>();

            // ACT
            var sitemapIndex = sut.Generate(baseSitemapUrl, sitemapNames);

            // ASSERT
            sitemapIndex.Entries.Length.Should().Be(2);
            sitemapIndex.Entries.All(entry =>
                !string.IsNullOrWhiteSpace(entry.Location) &&
                !string.IsNullOrWhiteSpace(entry.LastModification)).Should().BeTrue();
        }

        [Test]
        public void WHEN_RelativeSitemapUrlIsSpecicied_SHOULD_IndexShouldContainsAbsoluteUrl()
        {
            // ARRANGE
            var baseSitemapUrl = "https://hostname.com/baseUrl/";            
            var sitemapNames = new[] { "sitemap1", "sitemap2" };
            var sut = _container.CreateInstance<SitemapIndexGenerator>();

            // ACT
            var sitemapIndex = sut.Generate(baseSitemapUrl, sitemapNames);

            // ASSERT
            sitemapIndex.Entries.Length.Should().Be(2);
            sitemapIndex.Entries.ElementAt(0).Location.Should().Be("https://hostname.com/baseUrl/sitemap1");
            sitemapIndex.Entries.ElementAt(1).Location.Should().Be("https://hostname.com/baseUrl/sitemap2");            
        }

        // Source: http://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
        private static bool IsLocationValid(string uriString)
        {
            bool result = Uri.TryCreate(uriString, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }
    }
}

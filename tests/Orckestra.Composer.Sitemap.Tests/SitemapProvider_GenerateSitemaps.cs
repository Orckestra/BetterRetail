using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Sitemap;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Sitemap.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class SitemapProvider_GenerateSitemaps
    {
        private AutoMocker _container;

        private const string sitemapFilePrefix = "test";

        [SetUp]
        public void Init()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_1003EntriesTotal_1000EntriesPerSitemap_SHOULD_ReturnTwoSitemaps()
        {
            // ARRANGE
            var numberOfEntriesPerSitemap = 1000;
            MockSitemapProviderConfig(_container, numberOfEntriesPerSitemap);

            var provider = _container.GetMock<ISitemapEntryProvider>();
            provider.Setup(p => p.GetEntriesAsync(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>(), 0, numberOfEntriesPerSitemap))
                .Returns(Task.FromResult(Enumerable.Range(1, 1000).Select(i => new SitemapEntry())));
            provider.Setup(p => p.GetEntriesAsync(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>(), 1000, numberOfEntriesPerSitemap))
                .Returns(Task.FromResult(Enumerable.Range(1, 3).Select(i => new SitemapEntry())));

            var sut = _container.CreateInstance<SitemapProvider>();

            // ACT
            var sitemaps = sut.GenerateSitemaps(new SitemapParams()
            {
                BaseUrl = "baseUrl", 
                Scope = "scope",
                Culture = new CultureInfo("en"),
            }).ToArray();

            // ASSERT
            sitemaps.Length.Should().Be(2);
            sitemaps.ElementAt(0).Entries.Length.Should().Be(1000);
            sitemaps.ElementAt(1).Entries.Length.Should().Be(3);

            // Verify that entry provider has been called 2 times
            provider.Verify(p => p.GetEntriesAsync(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void WHEN_500EntriesTotal_1000EntriesPerSitemap_SHOULD_ReturnOneSitemap()
        {
            // ARRANGE
            var numberOfEntriesPerSitemap = 1000;
            MockSitemapProviderConfig(_container, numberOfEntriesPerSitemap);
            
            var provider = _container.GetMock<ISitemapEntryProvider>();
            provider.Setup(p => p.GetEntriesAsync(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>(), 0, numberOfEntriesPerSitemap))
                .Returns(Task.FromResult(Enumerable.Range(1, 500).Select(i => new SitemapEntry())));

            var sut = _container.CreateInstance<SitemapProvider>();

            // ACT
            var sitemaps = sut.GenerateSitemaps(new SitemapParams()
            {
                BaseUrl = "baseUrl", 
                Scope = "scope",
                Culture = new CultureInfo("en")
            }).ToArray();

            // ASSERT            
            sitemaps.Length.Should().Be(1);
            sitemaps.ElementAt(0).Entries.Length.Should().Be(500);

            // Verify that entry provider has not been called 1 time
            provider.Verify(p => p.GetEntriesAsync(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void WHEN_BaseUrlIsNull_SHOULD_ThrowArgumentException()
        {
            // ARRANGE
            var numberOfEntriesPerSitemap = 1000;
            MockSitemapProviderConfig(_container, numberOfEntriesPerSitemap);

            var sut = _container.CreateInstance<SitemapProvider>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams()
            {
                Scope = "scope",
                Culture = new CultureInfo("en")
            }).ToArray();

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_CultureIsNull_SHOULD_ThrowArgumentException()
        {
            // ARRANGE
            var numberOfEntriesPerSitemap = 1000;
            MockSitemapProviderConfig(_container, numberOfEntriesPerSitemap);

            var sut = _container.CreateInstance<SitemapProvider>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams()
            {
                BaseUrl = "baseUrl",
                Scope = "scope",
                Culture = null
            }).ToArray();

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_ScopeIsNull_SHOULD_ThrowArgumentException()
        {
            // ARRANGE
            var numberOfEntriesPerSitemap = 1000;
            MockSitemapProviderConfig(_container, numberOfEntriesPerSitemap);

            var sut = _container.CreateInstance<SitemapProvider>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams()
            {
                BaseUrl = "baseUrl",
                Culture = new CultureInfo("en")
            }).ToArray();

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        private static Mock<ISitemapProviderConfig> MockSitemapProviderConfig(AutoMocker container, int numberOfEntriesPerSitemap, string sitemapFilePrefix = sitemapFilePrefix)
        {
            var config = container.GetMock<ISitemapProviderConfig>();
            config.Setup(c => c.SitemapFilePrefix)
                    .Returns(sitemapFilePrefix);

            container.GetMock<IC1SitemapConfiguration>()
                .Setup(c => c.NumberOfEntriesPerFile)
                .Returns(numberOfEntriesPerSitemap);

            return config;
        }
    }
}

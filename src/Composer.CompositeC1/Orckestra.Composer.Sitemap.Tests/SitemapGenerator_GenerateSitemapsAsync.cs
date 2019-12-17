using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class SitemapGenerator_GenerateSitemapsAsync
    {
        private AutoMocker _container;

        private string SitemapDirectory { get; set; }

        private string WorkingDirectory { get; set; }

        private string WorkingRootDirectory { get; set; }

        [SetUp]
        public void Init()
        {
            _container = new AutoMocker();

            // Sitemap generator config
            SitemapDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            WorkingRootDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            WorkingDirectory = Path.Combine(WorkingRootDirectory, Path.GetRandomFileName());
            var config = _container.GetMock<ISitemapGeneratorConfig>();

            config.Setup(c => c.GetSitemapDirectory(It.IsAny<SitemapParams>())).Returns(SitemapDirectory);
            config.Setup(c => c.GetWorkingRootDirectory()).Returns(WorkingRootDirectory);
            config.Setup(c => c.GetWorkingDirectory(It.IsAny<SitemapParams>())).Returns(WorkingDirectory);

            // Sitemap index generator
            var indexGenerator = _container.GetMock<ISitemapIndexGenerator>();
            indexGenerator.Setup(generator => generator.Generate(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new SitemapIndex());
        }

        [Test]
        public void WHEN_BaseUrlNotSpecified_SHOULD_ThrowArgumentException()
        {
            // ARRANGE          
            string baseUrl = null;
            string baseSitemapUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams(), baseSitemapUrl, cultures);

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_ScopeNotSpecified_SHOULD_ThrowArgumentException()
        {
            // ARRANGE                
            string baseUrl = "baseUrl";
            string baseSitemapUrl = "relativeUrl";
            string scope = null;
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope}, baseSitemapUrl, cultures);

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_CultureNotSpecified_SHOULD_ThrowArgumentException()
        {
            // ARRANGE                
            string baseUrl = "baseUrl";
            string relativeUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new CultureInfo[0];

            var sut = _container.CreateInstance<SitemapGenerator>();

            // ACT
            Action action = () => sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope}, relativeUrl, cultures);

            // ASSERT
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_GenerateSitemapWithMultipleProviders_SHOULD_CreateMultipleSitemaps()
        {
            // ARRANGE    
            var providerMock1 = MockSitemapProvider(1);
            var providerMock2 = MockSitemapProvider(2);
            var providerMock3 = MockSitemapProvider(3);
            _container.Use<IEnumerable<ISitemapProvider>>(new[] { providerMock1.Mock.Object, providerMock2.Mock.Object, providerMock3.Mock.Object });

            Directory.CreateDirectory(SitemapDirectory);

            string baseUrl = "baseUrl";
            string baseSitemapUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            try
            {
                // ACT
                sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope }, baseSitemapUrl, cultures);

                // ASSERT
                Assert_SitemapExists(SitemapDirectory, providerMock1.Sitemaps.Concat(providerMock2.Sitemaps).Concat(providerMock3.Sitemaps));
            }
            finally
            {
                Cleanup();                
            }
        }

        [Test]
        public void WHEN_GenerateSitemap_SHOULD_SitemapIndexGeneratorIsCalled()
        {
            // ARRANGE    
            var providerMock1 = MockSitemapProvider(1);
            _container.Use<IEnumerable<ISitemapProvider>>(new[] { providerMock1.Mock.Object });

            Directory.CreateDirectory(SitemapDirectory);

            string baseUrl = "baseUrl";
            string baseSitemapUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            try
            {
                // ACT
                sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope }, baseSitemapUrl, cultures);

                // ASSERT
                _container.GetMock<ISitemapIndexGenerator>()
                    .Verify(generator => generator.Generate(baseSitemapUrl, It.IsAny<IEnumerable<string>>()), Times.Once());
            }
            finally
            {
                Cleanup();
            }
        }

        [Test]
        public void WHEN_GenerateSitemap_SHOULD_DeleteWorkingDirectory()
        {
            // ARRANGE                
            var providerMock = MockSitemapProvider(1);
            _container.Use<IEnumerable<ISitemapProvider>>(new[] { providerMock.Mock.Object });

            Directory.CreateDirectory(SitemapDirectory);

            string baseUrl = "baseUrl";
            string relativeUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            try
            {
                // ACT
                sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope }, relativeUrl, cultures);

                // ASSERT
                Directory.Exists(WorkingRootDirectory).Should().BeFalse();
            }
            finally
            {
                Cleanup();
            }
        }

        [Test]
        public void WHEN_ExceptionOccurs_SHOULD_DeleteWorkingDirectory()
        {
            // ARRANGE                
            var provider = _container.GetMock<ISitemapProvider>();
            provider.Setup(p => p.GenerateSitemaps(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>()))
              .Throws<Exception>();
            _container.Use<IEnumerable<ISitemapProvider>>(new[] { provider.Object });

            Directory.CreateDirectory(SitemapDirectory);

            string baseUrl = "baseUrl";
            string relativeUrl = "relativeUrl";
            string scope = "scope";
            CultureInfo[] cultures = new[] { new CultureInfo("en") };

            var sut = _container.CreateInstance<SitemapGenerator>();

            try
            {
                // ACT
                Action action = () => sut.GenerateSitemaps(new SitemapParams() { BaseUrl = baseUrl, Scope = scope }, relativeUrl, cultures);

                // ASSERT
                action.ShouldThrow<Exception>();
                Directory.Exists(WorkingRootDirectory).Should().BeFalse();
            }
            finally
            {
                Cleanup();
            }
        }

        private static ProviderMock MockSitemapProvider(int providerIndex)
        {
            var provider = new Mock<ISitemapProvider>();

            var sitemaps = Enumerable.Range(1, 3).Select(sitemapIndex => CreateSitemap($"sitemap-provider-{providerIndex}.{sitemapIndex}.xml"));

            provider.Setup(p => p.GenerateSitemaps(It.IsAny<SitemapParams>(), It.IsAny<CultureInfo>()))
              .Returns(sitemaps);

            return new ProviderMock
            {
                Mock = provider,
                Sitemaps = sitemaps,
            };
        }

        private static Models.Sitemap CreateSitemap(string name)
        {
            return new Models.Sitemap
            {
                Name = name,
                Entries = Enumerable.Range(0, 10).Select(i => new SitemapEntry
                {
                    Location = "http://orckestra.dev.local/product.html",
                    Priority = "0.5",
                    LastModification = "2005-05-10T17:33:30+08:00",
                    ChangeFrequency = "daily"
                }).ToArray()
            };
        }

        private void Cleanup()
        {
            DeleteDirectoryIfExists(SitemapDirectory);         
        }

        private void DeleteDirectoryIfExists(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }
        }

        private void Assert_SitemapExists(string directory, IEnumerable<Models.Sitemap> sitemaps)
        {
            foreach (var sitemap in sitemaps)
            {
                var path = Path.Combine(directory, sitemap.Name);
                File.Exists(path).Should().BeTrue();
            }
        }

        private class ProviderMock
        {
            public Mock<ISitemapProvider> Mock { get; set; }

            public IEnumerable<Models.Sitemap> Sitemaps { get; set; }
        }
    }
}

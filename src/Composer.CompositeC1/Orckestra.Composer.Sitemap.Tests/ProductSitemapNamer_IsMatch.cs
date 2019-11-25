using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Sitemap.Product;

namespace Orckestra.Composer.Sitemap.Tests
{
    [TestFixture]
    public class ProductSitemapNamer_IsMatch
    {
        private AutoMocker _container;
        private ProductSitemapNamer _sut;

        [SetUp]
        public void Init()
        {
            _container = new AutoMocker();
            _sut = _container.CreateInstance<ProductSitemapNamer>();
        }

        [Test]
        public void WHEN_GoodFileName_SHOULD_ReturnTrue()
        {
            // ARRANGE
            string filenameA = "sitemap-en-CA-products-245.xml";
            string filenameB = "sitemap-en-products-245.xml";            

            // ACT
            var resultA = _sut.IsMatch(filenameA);
            var resultB = _sut.IsMatch(filenameB);

            // ASSERT
            resultA.Should().BeTrue();
            resultB.Should().BeTrue();
        }

        [Test]
        public void WHEN_BadFileName_SHOULD_ReturnFalse()
        {
            // ARRANGE
            string filename = "sitemap-en-products-245.json";            

            // ACT
            var result = _sut.IsMatch(filename);

            // ASSERT
            result.Should().BeFalse();
        }

        [Test]
        public void WHEN_NullFileName_SHOULD_ReturnFalse()
        {
            // ARRANGE
            string filename = null;            

            // ACT
            var result = _sut.IsMatch(filename);

            // ASSERT
            result.Should().BeFalse();
        }

        [Test]
        public void WHEN_EmptyFileName_SHOULD_ReturnFalse()
        {
            // ARRANGE
            string filename = string.Empty;            

            // ACT
            var result = _sut.IsMatch(filename);

            // ASSERT
            result.Should().BeFalse();
        }
    }
}

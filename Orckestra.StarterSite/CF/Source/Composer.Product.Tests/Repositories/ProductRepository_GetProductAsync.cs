using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using ServiceStack;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture(Category = "ProductRepository")]
    public class ProductRepositoryGetProductAsync
    {
        private AutoMocker _container;
        private CultureInfo _englishCultureInfo;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _englishCultureInfo = CultureInfo.GetCultureInfo("en-US");

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Overture.ServiceModel.Products.Product>>>(),
                    It.IsAny<Func<Overture.ServiceModel.Products.Product, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Overture.ServiceModel.Products.Product>>, 
                        Func<Overture.ServiceModel.Products.Product, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [TestCase(null, "123456")]
        [TestCase("  ", "123456")]
        [TestCase("TestScope", null)]
        [TestCase("TestScope", "    ")]
        public void WhenAnyStringArgumentNullOrWhitespace_ThrowsArgumentException(string scope, string productId)
        {
            //Arrange
            var productRepository = _container.CreateInstance<ProductRepository>();

            //Act & Assert
            Assert.Throws<ArgumentException>(async () =>
            {
                await productRepository.GetProductAsync(new GetProductParam { Scope = scope, CultureInfo = _englishCultureInfo, ProductId = productId });
            });
        }

        [Test]
        public async void WhenOkParameters_InvokesOvertureClientSendAsync()
        {
            //Arrange
            var overtureClientMock = new Mock<IOvertureClient>();

            overtureClientMock
                .Setup(oc => oc.SendAsync(It.IsNotNull<IReturn<Overture.ServiceModel.Products.Product>>()))
                .ReturnsAsync(new Overture.ServiceModel.Products.Product() { Active = true })
                .Verifiable();
            _container.Use(overtureClientMock);

            var productRepository = _container.CreateInstance<ProductRepository>();

            //Act
            var product = await productRepository.GetProductAsync(new GetProductParam { Scope = GetRandom.String(10), CultureInfo = _englishCultureInfo, ProductId = GetRandom.String(10)});

            //Assert
            product.Should().NotBeNull();
            overtureClientMock.Verify();
        }

        [Test]
        public async void WHEN_Ok_parameters_but_product_is_inactive_SHOULD_return_null()
        {
            //Arrange
            var overtureClientMock = new Mock<IOvertureClient>();
            overtureClientMock
                .Setup(oc => oc.SendAsync(It.IsNotNull<IReturn<Overture.ServiceModel.Products.Product>>()))
                .ReturnsAsync(new Overture.ServiceModel.Products.Product() { Active = false })
                .Verifiable();
            _container.Use(overtureClientMock);

            var productRepository = _container.CreateInstance<ProductRepository>();

            //Act
            var product = await productRepository.GetProductAsync(new GetProductParam { Scope = GetRandom.String(10), CultureInfo = _englishCultureInfo, ProductId = GetRandom.String(10) });

            //Assert
            product.Should().BeNull();
            overtureClientMock.Verify();
        }
    }
}

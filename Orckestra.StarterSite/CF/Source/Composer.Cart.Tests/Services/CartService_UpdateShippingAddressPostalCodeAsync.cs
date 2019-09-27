using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CartService_UpdateShippingAddressPostalCodeAsync
    {
        private const string CanadianPostalCodeRegex = @"^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$"; // 

        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();

            _container.Use(ViewModelMapperFactory.Create());
            _container.Use(CartViewModelFactoryMock.Create());
            _container.Use(LocalizationProviderFactory.Create());
            _container.Use(RegionCodeProviderFactory.Create());

            UseCartRepository();
        }

        private void UseCartRepository()
        {
            var fakeCart = new ProcessedCart
            {
                CustomerId = Guid.NewGuid(),
                Shipments = new List<Shipment> { new Shipment() },
                BillingCurrency = GetRandom.String(1),
                CartType = GetRandom.String(1),
                Name = GetRandom.String(1),
                ScopeId = GetRandom.String(1),
                Status = GetRandom.String(1)
            };

            var cartRepositoryMock = new Mock<ICartRepository>();

            cartRepositoryMock.Setup(repo => repo.GetCartAsync(It.IsNotNull<GetCartParam>()))
                              .ReturnsAsync(fakeCart);

            cartRepositoryMock.Setup(repo => repo.UpdateCartAsync(It.IsNotNull<UpdateCartParam>()))
                              .ReturnsAsync(fakeCart);

            _container.Use(cartRepositoryMock);
        }

        [Test]
        public void WHEN_postal_code_does_not_matches_regex_SHOULD_result_not_be_null()
        {
            // Arrange
            _container.Use(CountryServiceMock.Create(CanadianPostalCodeRegex));

            var service = _container.CreateInstance<CartService>();

            // Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateShippingAddressPostalCodeAsync(new UpdateShippingAddressPostalCodeParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                PostalCode = "any",
                CountryCode = GetRandom.String(1)
            }));
        }

        [Test]
        public async Task WHEN_postal_code_matches_regex_SHOULD_result_not_be_null()
        {
            // Arrange
            _container.Use(CountryServiceMock.Create(CanadianPostalCodeRegex));

            var service = _container.CreateInstance<CartService>();

            // Act
            var result = await service.UpdateShippingAddressPostalCodeAsync(new UpdateShippingAddressPostalCodeParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                PostalCode = "J5R 5Y5",
                CountryCode = GetRandom.String(1)
            });

            // Assert
            result.Should().NotBeNull();
        }
    }
}

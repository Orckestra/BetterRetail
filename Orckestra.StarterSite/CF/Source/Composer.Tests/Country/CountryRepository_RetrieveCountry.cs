using System;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Country;
using Orckestra.Composer.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;

namespace Orckestra.Composer.Tests.Country
{
    [TestFixture]
    public class CountryRepositoryRetrieveCountry
    {
        private AutoMocker _container;
        private CountryRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Overture.ServiceModel.Country>>>(),
                    It.IsAny<Func<Overture.ServiceModel.Country, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Overture.ServiceModel.Country>>, Func<Overture.ServiceModel.Country, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();

            var overtureClient = _container.GetMock<IOvertureClient>();
            var dummyCountry = new Overture.ServiceModel.Country();

            overtureClient
                .Setup(client => client.SendAsync(
                    It.IsNotNull<GetCountryRequest>()))
                .ReturnsAsync(dummyCountry)
                .Verifiable();

            _repository = _container.CreateInstance<CountryRepository>();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());

            // Act
            var result = _repository.RetrieveCountry(new RetrieveCountryParam
            {
                IsoCode = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture()
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            // Arrange
            _container.Use(OvertureClientFactory.CreateWithNullValues());

            // Act
            var result = _repository.RetrieveCountry(new RetrieveCountryParam
            {
                IsoCode = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture()
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_IsoCode_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string isoCode)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await _repository.RetrieveCountry(new RetrieveCountryParam
                {
                    IsoCode = isoCode,
                    CultureInfo = TestingExtensions.GetRandomCulture()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("IsoCode");
        }
    }
}

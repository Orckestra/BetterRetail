using System;
using System.Collections.Generic;
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
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;

namespace Orckestra.Composer.Tests.Country
{
    [TestFixture]
    public class CountryRepositoryRetrieveRegions
    {
        private AutoMocker _container;
        private CountryRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var overtureClient = _container.GetMock<IOvertureClient>();

            overtureClient
                .Setup(client => client.SendAsync(
                    It.IsNotNull<GetCountryRequest>()))
                .ReturnsAsync(new Overture.ServiceModel.Country())
                .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetRegionsRequest>()))
                         .ReturnsAsync(new List<Region>())
                         .Verifiable();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<List<Region>>>>(),
                    It.IsAny<Func<List<Region>, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<List<Region>>>, Func<List<Region>, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();

            _repository = _container.CreateInstance<CountryRepository>();
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var param = new RetrieveCountryParam
            {
                IsoCode = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture()
            };

            // Act
            var result = await _repository.RetrieveRegions(param).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_IsoCode_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string isoCode)
        {
            var param = new RetrieveCountryParam
            {
                IsoCode = isoCode,
                CultureInfo = TestingExtensions.GetRandomCulture()
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _repository.RetrieveRegions(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("IsoCode");
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var param = new RetrieveCountryParam
            {
                IsoCode = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _repository.RetrieveRegions(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CultureInfo");
        }
    }
}

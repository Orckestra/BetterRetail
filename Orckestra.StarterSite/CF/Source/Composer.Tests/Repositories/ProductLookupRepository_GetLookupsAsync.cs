using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Country;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Requests;
using ServiceStack;

namespace Orckestra.Composer.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    public class ProductLookupRepository_GetLookupsAsync
    {
        private AutoMocker _container;
        private ProductLookupRepository _repository;

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

            _repository = _container.CreateInstance<ProductLookupRepository>();
        }

        [Test]
        public async Task WHEN_GetLookupsAsync_Called_SHOULD_Call_CacheProvider()
        {
            // Act
            await _repository.GetLookupsAsync();

            // Assert
            _container.Verify<ICacheProvider>(oc => oc.GetOrAddAsync(
                It.IsNotNull<CacheKey>(),
                It.IsNotNull<Func<Task<List<Lookup>>>>(),
                It.IsAny<Func<List<Lookup>, Task>>(),
                It.IsAny<CacheKey>()));
        }
    }
}

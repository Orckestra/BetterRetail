using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Providers;
using Orckestra.Overture.ServiceModel.Requests.Providers;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryGetPaymentProviders
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<GetPaymentProvidersResponse>>>(),
                    It.IsAny<Func<GetPaymentProvidersResponse, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<GetPaymentProvidersResponse>>,
                        Func<GetPaymentProvidersResponse, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [Test]
        public async Task When_Passing_Valid_ScopeId_SHOULD_Return_PaymentProviders()
        {
            //Arrange
            var paymentProviderInfos = new GetPaymentProvidersResponse
            {
                PaymentProviders = new List<PaymentProvider>
                {
                    new PaymentProvider(),
                }
            };
            MockPaymentProvidersRequest(paymentProviderInfos);

            //Act
            var scope = GetRandom.String(32);
            var repository = _container.CreateInstance<PaymentRepository>();
            var result = await repository.GetPaymentProviders(scope).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void When_Passing_Empty_ScopeId_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var paymentProviderInfos = new GetPaymentProvidersResponse
            {
                PaymentProviders = new List<PaymentProvider>
                {
                    new PaymentProvider(),
                }
            };
            MockPaymentProvidersRequest(paymentProviderInfos);

            //Act & Assert
            var repository = _container.CreateInstance<PaymentRepository>();
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPaymentProviders(null));
        }

        private void MockPaymentProvidersRequest(GetPaymentProvidersResponse paymentProviderInfos)
        {
            var overtureClient = _container.GetMock<IOvertureClient>();
            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetPaymentProvidersRequest>()))
                .ReturnsAsync(paymentProviderInfos)
                .Verifiable();
        }
    }
}

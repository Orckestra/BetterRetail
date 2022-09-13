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
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    extern alias occ;

    [TestFixture]
    public class PaymentRepositoryGetPaymentMethodsAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            //
            var cacheProvider = _container.GetMock<occ::Orckestra.Overture.Caching.ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<occ::Orckestra.Overture.Caching.CacheKey>(),
                    It.IsNotNull<Func<Task<List<PaymentMethod>>>>(),
                    It.IsAny<Func<List<PaymentMethod>, Task>>(),
                    It.IsAny<occ::Orckestra.Overture.Caching.CacheKey>()))
                .Returns<occ::Orckestra.Overture.Caching.CacheKey, Func<Task<List<PaymentMethod>>>,
                        Func<List<PaymentMethod>, Task>, occ::Orckestra.Overture.Caching.CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [Test]
        public async Task When_Passing_Valid_GetPaymentMethodsParam_SHOULD_Return_PaymentMethods()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            //Act
            var param = new GetPaymentMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ProviderNames = new List<string> { GetRandom.String(7), GetRandom.String(7) },
                Scope = GetRandom.String(32),

            };

            var repository = _container.CreateInstance<PaymentRepository>();
            var result = await repository.GetPaymentMethodsAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task When_Overture_Return_Empty_List_SHOULD_Not_Throw()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            //Act
            var param = new GetPaymentMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ProviderNames = new List<string> { GetRandom.String(7), GetRandom.String(7) },
                Scope = GetRandom.String(32),

            };

            var repository = _container.CreateInstance<PaymentRepository>();
            var result = await repository.GetPaymentMethodsAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void When_Passing_null_param_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            var repository = _container.CreateInstance<PaymentRepository>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPaymentMethodsAsync(null));
        }

        

        [Test]
        public void When_Passing_null_CartName_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            var repository = _container.CreateInstance<PaymentRepository>();
            var param = new GetPaymentMethodsParam
            {
                CartName = null,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ProviderNames = new List<string> { GetRandom.String(7), GetRandom.String(7) },
                Scope = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetPaymentMethodsAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CartName");
        }

        [Test]
        public void When_Passing_null_ProviderName_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            var repository = _container.CreateInstance<PaymentRepository>();
            var param = new GetPaymentMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ProviderNames = null,
                Scope = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetPaymentMethodsAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("ProviderName");
        }

        [Test]
        public void When_Passing_null_Scope_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var paymentMethods = new List<PaymentMethod> { new PaymentMethod() };
            MockFindCartPaymentMethodsRequest(paymentMethods);

            var repository = _container.CreateInstance<PaymentRepository>();
            var param = new GetPaymentMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ProviderNames = new List<string> { GetRandom.String(7), GetRandom.String(7) },
                Scope = null,
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetPaymentMethodsAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("Scope");
        }
        
        private void MockFindCartPaymentMethodsRequest(List<PaymentMethod> paymentMethods)
        {
            var overtureClient = _container.GetMock<IComposerOvertureClient>();
            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCartPaymentMethodsRequest>()))
                .ReturnsAsync(paymentMethods)
                .Verifiable();
        }
    }
}

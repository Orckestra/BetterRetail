using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Caching;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    internal class CustomerRepository_GetCustomerByIdAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IComposerOvertureClient>(MockBehavior.Strict));
            _container.Use(new Mock<ICacheProvider>(MockBehavior.Strict));

            //Testing with cache always empty 
            //3.8 upgrade
            //object someDummyOutput;
            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Customer>>>(),
                    It.IsAny<Func<Customer, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Customer>>, Func<Customer, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_return_customer()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var expectedScope = GetRandom.String(32);
            var expectedCultureInfo = TestingExtensions.GetRandomCulture();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IComposerOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<GetCustomerRequest>(
                    param => param.CustomerId == expectedCustomer.Id)))
                .ReturnsAsync(expectedCustomer);

            //3.8 upgrade
            _container.GetMock<ICacheProvider>()
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Customer>>>(),
                    It.IsAny<Func<Customer, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Customer>>, Func<Customer, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable("This value must be proprelly cached by Id");

            //Act
            var customer = await customerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = expectedCustomer.Id,
                Scope = expectedScope,
                CultureInfo = expectedCultureInfo
            });

            //Assert
            customer.Should().NotBeNull();
            customer.Id.Should().Be(expectedCustomer.Id);

            _container.GetMock<ICacheProvider>().VerifyAll();
        }

        [Test]
        public async Task WHEN_unknown_CustomerId_SHOULD_return_null()
        {
            //Arrange
            var expectedCustomerId = GetRandom.Guid();
            var expectedScope = GetRandom.String(32);
            var expectedCultureInfo = TestingExtensions.GetRandomCulture();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IComposerOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<GetCustomerRequest>(
                    param => param.CustomerId == expectedCustomerId)))
                .ReturnsAsync(null);

            //3.8 upgrade
            _container.GetMock<ICacheProvider>()
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Customer>>>(),
                    It.IsAny<Func<Customer, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Customer>>, Func<Customer, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable("This value must be proprelly cached by Id");

            //Act
            var customer = await customerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = expectedCustomerId,
                Scope = expectedScope,
                CultureInfo = expectedCultureInfo
            });

            //Assert
            customer.Should().BeNull();

            _container.GetMock<ICacheProvider>().VerifyAll();
        }


        [Test]
        public void WHEN_Param_is_Null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerRepository.GetCustomerByIdAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("getCustomerByIdParam");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.GetCustomerByIdAsync(
                new GetCustomerByIdParam
                {
                    CultureInfo = cultureInfo,
                    CustomerId = GetRandom.Guid(),
                    Scope = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_is_Empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerId = Guid.Empty;
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.GetCustomerByIdAsync(
                new GetCustomerByIdParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = customerId,
                    Scope = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_is_Empty_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var customerId = GetRandom.Guid();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.GetCustomerByIdAsync(
                new GetCustomerByIdParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = customerId,
                    Scope = scope
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }
    }
}

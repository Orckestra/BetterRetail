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
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories
{   
    [TestFixture]
    public class FulfillmentMethodRepositoryGetCalculatedFulfilmentMethods
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public async Task When_Passing_Valid_GetShippingMethodsParam_SHOULD_Return_FulfillmentMethods()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod>{ new FulfillmentMethod() };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            //Act
            GetShippingMethodsParam param = new GetShippingMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
            };

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var result = await repository.GetCalculatedFulfillmentMethods(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task When_Overture_Return_Empty_List_SHOULD_Not_Throw()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod> { null };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            //Act
            var param = new GetShippingMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
            };

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var result = await repository.GetCalculatedFulfillmentMethods(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void When_Passing_null_CartName_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod> { new FulfillmentMethod() };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var param = new GetShippingMethodsParam
            {
                CartName = null,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetCalculatedFulfillmentMethods(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CartName");
        }

        [Test]
        public void When_Passing_null_CultureInfo_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod> { new FulfillmentMethod() };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var param = new GetShippingMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetCalculatedFulfillmentMethods(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CultureInfo");
        }

        [Test]
        public void When_Passing_null_CustomerId_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod> { new FulfillmentMethod() };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var param = new GetShippingMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = new Guid(),
                Scope = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetCalculatedFulfillmentMethods(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CustomerId");
        }

        [Test]
        public void When_Passing_null_Scope_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var overtureClient = new Mock<IOvertureClient>();

            var fulfillmentMethods = new List<FulfillmentMethod> { new FulfillmentMethod() };

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                          .ReturnsAsync(fulfillmentMethods)
                          .Verifiable();

            _container.Use(overtureClient);

            var repository = _container.CreateInstance<FulfillmentMethodRepository>();
            var param = new GetShippingMethodsParam
            {
                CartName = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = null,
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.GetCalculatedFulfillmentMethods(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("Scope");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using FluentAssertions;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class FulfillmentMethodRepository_GetCalculatedFulfillmentMethods
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            GetShippingMethodsParam p = null;
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await sut.GetCalculatedFulfillmentMethods(p));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }


        [TestCase("    ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_Scope_is_null_or_whitespace_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var p = new GetShippingMethodsParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                Scope = scope,
                CustomerId = GetRandom.Guid(),
            };
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await sut.GetCalculatedFulfillmentMethods(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CustomerId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new GetShippingMethodsParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(10),
            };
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await sut.GetCalculatedFulfillmentMethods(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [TestCase("     ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_CartName_is_null_or_whitespace_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var p = new GetShippingMethodsParam()
            {
                CartName = cartName,
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            };
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await sut.GetCalculatedFulfillmentMethods(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CultureInfo_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var p = new GetShippingMethodsParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            };
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await sut.GetCalculatedFulfillmentMethods(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public async void WHEN_parameters_ok_SHOULD_invoke_Client()
        {
            //Arrange
            var p = new GetShippingMethodsParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            };
            var ovClientMock = Container.GetMock<IOvertureClient>();
            ovClientMock.Setup(m => m.SendAsync(It.IsNotNull<FindCalculatedFulfillmentMethodsRequest>()))
                .ReturnsAsync(new List<FulfillmentMethod>())
                .Verifiable();
            Container.Use(ovClientMock);
            var sut = Container.CreateInstance<FulfillmentMethodRepository>();


            //Act
            await sut.GetCalculatedFulfillmentMethods(p);

            //Assert
            ovClientMock.Verify();

        }
    }
}

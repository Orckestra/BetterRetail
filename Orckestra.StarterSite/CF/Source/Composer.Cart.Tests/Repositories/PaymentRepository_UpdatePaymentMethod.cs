using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryUpdatePaymentMethod
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var overtureClientMock = _container.GetMock<IOvertureClient>();
            overtureClientMock.Setup(ov => ov.SendAsync(It.IsNotNull<UpdatePaymentMethodRequest>()))
                .ReturnsAsync(new ProcessedCart())
                .Verifiable();

            _container.Use(overtureClientMock);
        }

        [Test]
        public async Task WHEN_param_ok_SHOULD_call_Overture_UpdatePaymentMethodRequest()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var cart = await sut.UpdatePaymentMethodAsync(param);

            //Assert
            cart.Should().NotBeNull();
            _container.Verify<IOvertureClient>(ov => ov.SendAsync(It.IsNotNull<UpdatePaymentMethodRequest>()));
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(async () => await sut.UpdatePaymentMethodAsync(null));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_CartName_is_invalid_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = cartName,
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("cartname");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_PaymentProviderName_is_invalid_SHOULD_throw_ArgumentException(string paymentProviderName)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = paymentProviderName,
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("PaymentProviderName");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_Scope_is_invalid_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = scope
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        public void WHEN_param_CultureInfo_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("cultureinfo");
        }

        [Test]
        public void WHEN_param_CustomerId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = Guid.Empty,
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("customerId");
        }

        [Test]
        public void WHEN_param_PaymentId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = Guid.Empty,
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentId");
        }

        [Test]
        public void WHEN_param_PaymentMethodId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = Guid.Empty,
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(async () => await sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentMethodId");
        }
    }
}

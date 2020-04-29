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
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

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
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdatePaymentMethodAsync(null));

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
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
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
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)));
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
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentMethodId");
        }
    }
}

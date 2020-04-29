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
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryRemovePaymentAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public async Task WHEN_calling_RemovePaymentAsync_SHOULD_invoke_Overture_Client()
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12),
                PaymentId = GetRandom.Guid()
            };
            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            await sut.RemovePaymentAsync(param);

            //Assert
            _container.Verify<IOvertureClient>(oc => oc.SendAsync(It.IsNotNull<RemovePaymentRequest>()));
        }


        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemovePaymentAsync(null));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("\r\n         \t")]
        [TestCase("          ")]
        public void WHEN_CartName_is_null_or_whitespace_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = cartName,
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12),
                PaymentId = GetRandom.Guid()
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.RemovePaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("\r\n         \t")]
        [TestCase("          ")]
        public void WHEN_scope_is_null_or_whitespace_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = scope,
                PaymentId = GetRandom.Guid()
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.RemovePaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_cultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = null,
                Scope = GetRandom.String(12),
                PaymentId = GetRandom.Guid()
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemovePaymentAsync(param));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
            exception.Message.Should().ContainEquivalentOf("cultureInfo");
        }

        [Test]
        public void WHEN_customerId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = GetRandom.String(7),
                CustomerId = Guid.Empty,
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12),
                PaymentId = GetRandom.Guid()
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemovePaymentAsync(param));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
            exception.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_paymentId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new VoidOrRemovePaymentParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12),
                PaymentId = Guid.Empty
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemovePaymentAsync(param));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
            exception.Message.Should().ContainEquivalentOf("PaymentId");
        }
    }
}

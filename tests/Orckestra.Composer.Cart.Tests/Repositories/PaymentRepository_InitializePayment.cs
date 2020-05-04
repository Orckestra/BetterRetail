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

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryInitializePayment
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var ovMock = _container.GetMock<IOvertureClient>();
            ovMock.Setup(ov => ov.SendAsync(It.IsNotNull<InitializePaymentRequest>()))
                .ReturnsAsync(new Overture.ServiceModel.Orders.Cart())
                .Verifiable("Overture was never called to Initialize the payment.");
        }

        [Test]
        public async Task WHEN_param_ok_SHOULD_call_Overture_InitializePaymentRequest()
        {
            //Arrange
            var param = new InitializePaymentParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var cart = await sut.InitializePaymentAsync(param);

            //Assert
            cart.Should().NotBeNull();
            _container.Verify<IOvertureClient>(ov => ov.SendAsync(It.IsNotNull<InitializePaymentRequest>()));
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => sut.InitializePaymentAsync(null));

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
            var param = new InitializePaymentParam
            {
                CartName = cartName,
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            Expression<Func<Task<Overture.ServiceModel.Orders.Cart>>> expression = () => sut.InitializePaymentAsync(param);
            var exception = Assert.Throws<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_Scope_is_invalid_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var param = new InitializePaymentParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = scope
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            Expression<Func<Task<Overture.ServiceModel.Orders.Cart>>> expression = () => sut.InitializePaymentAsync(param);
            var exception = Assert.Throws<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_param_CultureInfo_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new InitializePaymentParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.InitializePaymentAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("cultureinfo");
        }

        [Test]
        public void WHEN_param_CustomerId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new InitializePaymentParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = Guid.Empty,
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.InitializePaymentAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("customerId");
        }

        [Test]
        public void WHEN_param_PaymentId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new InitializePaymentParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = Guid.Empty,
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentRepository>();

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.InitializePaymentAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentId");
        }
    }
}

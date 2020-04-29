using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;
using static Orckestra.Composer.Utils.ExpressionUtility;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class CartRepositoryAddPaymentAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var clientMock = CreateOvertureClientMock();
            _container.Use(clientMock);
        }

        private Mock<IOvertureClient> CreateOvertureClientMock()
        {
            var clientMock = _container.GetMock<IOvertureClient>();

            clientMock.Setup(ov => ov.SendAsync(It.IsNotNull<AddPaymentRequest>()))
                .Returns((AddPaymentRequest r) =>
                {
                    var cart = new ProcessedCart
                    {
                        Id = GetRandom.Guid(),
                        ScopeId = r.ScopeId,
                        CustomerId = r.CustomerId,
                        Name = r.CartName,
                        CultureName = r.CultureName,
                        Payments = new List<Payment>
                        {
                            new Payment
                            {
                                PaymentStatus = PaymentStatus.New,
                                Id = GetRandom.Guid(),
                                BillingAddress = r.BillingAddress,
                                Amount = r.Amount.GetValueOrDefault()
                            }
                        }
                    };

                    return Task.FromResult(cart);
                }).Verifiable("Overture call for 'AddPaymentRequest' was not made.");
            return clientMock;
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.AddPaymentAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("       ")]
        public void WHEN_param_CartName_is_invalid_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();
            var param = new AddPaymentParam
            {
                Amount = GetRandom.PositiveDecimal(1000.0m),
                CartName = cartName,
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.AddPaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("       ")]
        public void WHEN_param_scope_is_invalid_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();
            var param = new AddPaymentParam
            {
                Amount = GetRandom.PositiveDecimal(1000.0m),
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = scope
            };

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.AddPaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_param_CultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();
            var param = new AddPaymentParam
            {
                Amount = GetRandom.PositiveDecimal(1000.0m),
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.AddPaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public void WHEN_param_CustomerId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();
            var param = new AddPaymentParam
            {
                Amount = GetRandom.PositiveDecimal(1000.0m),
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(7)
            };

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.AddPaymentAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task WHEN_param_ok_no_shipmentId_SHOULD_call_overture_with_AddPaymentRequest_and_SHOULD_return_ProcessedCart(bool includeBillingAddress)
        {
            //Arrange
            var sut = _container.CreateInstance<CartRepository>();
            var billingAddress = includeBillingAddress
                ? new Address
                {
                    City = GetRandom.String(7),
                    CountryCode = GetRandom.UpperCaseString(2),
                    RegionCode = GetRandom.Usa.State(),
                    Email = GetRandom.Email(),
                    FirstName = GetRandom.FirstName(),
                    LastName = GetRandom.LastName(),
                    IsPreferredBilling = GetRandom.Boolean(),
                    IsPreferredShipping = GetRandom.Boolean(),
                    Line1 = GetRandom.String(25),
                    PhoneNumber = GetRandom.Usa.PhoneNumber(),
                    PostalCode = GetRandom.String(6)
                } : null;

            var param = new AddPaymentParam
            {
                Amount = GetRandom.PositiveDecimal(1000.0m),
                CartName = GetRandom.String(10),
                BillingAddress = billingAddress,
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };

            //Act
            var cart = await sut.AddPaymentAsync(param);

            //Assert
            cart.Should().NotBeNull();
            cart.Payments.Should().NotBeNullOrEmpty();

            var payment = cart.Payments.First();
            payment.BillingAddress.Should().Be(billingAddress);

            _container.Verify<IOvertureClient>(c => c.SendAsync(It.IsAny<AddPaymentRequest>()));
        }
    }
}

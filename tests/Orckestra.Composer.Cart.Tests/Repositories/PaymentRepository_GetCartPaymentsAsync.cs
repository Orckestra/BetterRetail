using System;
using System.Collections.Generic;
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
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class PaymentRepositoryGetCartPaymentsAsync
    {
        private AutoMocker _container;
        private PaymentRepository _sut;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _sut = _container.CreateInstance<PaymentRepository>();

            var ovClientMock = _container.GetMock<IOvertureClient>();
            ovClientMock.Setup(ov => ov.SendAsync(It.IsNotNull<GetPaymentsInCartRequest>()))
                .ReturnsAsync(new List<Payment>());
                

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<List<Payment>>>>(),
                    It.IsAny<Func<List<Payment>, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<List<Payment>>>, Func<List<Payment>, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }
        
        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetCartPaymentsAsync(null));

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
            var param = new GetCartPaymentsParam
            {
                CartName = cartName,
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12)
            };

            //Act
            Expression<Func<Task<List<Payment>>>> expression = () => _sut.GetCartPaymentsAsync(param);
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
            var param = new GetCartPaymentsParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = scope
            };

            //Act
            Expression<Func<Task<List<Payment>>>> expression = () => _sut.GetCartPaymentsAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }
        
        [Test]
        public void WHEN_cultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new GetCartPaymentsParam
            {
                CartName = GetRandom.String(7),
                CustomerId = GetRandom.Guid(),
                CultureInfo = null,
                Scope = GetRandom.String(12)
            };

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetCartPaymentsAsync(param));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
            exception.Message.Should().ContainEquivalentOf("cultureInfo");
        }
        
        [Test]
        public void WHEN_customerId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new GetCartPaymentsParam
            {
                CartName = GetRandom.String(7),
                CustomerId = Guid.Empty,
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = GetRandom.String(12)
            };
            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetCartPaymentsAsync(param));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().Be("param");
            exception.Message.Should().ContainEquivalentOf("CustomerId");
        }
    }
}

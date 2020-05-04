using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Coupons;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class CartRepositoryRemoveCouponsAsync
    {
        public AutoMocker Container { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            Container.Use(GetOvertureClientMock());
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_NullArgumentException()
        {
            RemoveCouponsParam p = null;
            var sut = Container.CreateInstance<CartRepository>();

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemoveCouponsAsync(p));

            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [TestCase("")]
        [TestCase(null)]
        public void WHEN_scope_is_invalid_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var p = new RemoveCouponsParam()
            {
                CartName = GetRandom.String(7),
                CouponCodes = new List<string>(),
                CustomerId = GetRandom.Guid(),
                Scope = scope
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveCouponsAsync(p));

            //Assert
            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [TestCase("")]
        [TestCase(null)]
        public void WHEN_cartName_is_invalid_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var p = new RemoveCouponsParam()
            {
                CartName = cartName,
                CouponCodes = new List<string>(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveCouponsAsync(p));

            //Assert
            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [Test]
        public void WHEN_couponCodes_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new RemoveCouponsParam()
            {
                CartName = GetRandom.String(7),
                CouponCodes = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            Expression<Func<Task>> expression = () => sut.RemoveCouponsAsync(p);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(p.CouponCodes)));
        }

        [Test]
        public void WHEN_customerId_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new RemoveCouponsParam()
            {
                CartName = GetRandom.String(7),
                CouponCodes = new List<string>(),
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveCouponsAsync(p));

            //Assert
            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [Test]
        public void WHEN_request_is_valid_SHOULD_invoke_overture_client()
        {
            //Arrange
            var p = new RemoveCouponsParam()
            {
                CartName = GetRandom.String(7),
                CouponCodes = new List<string>()
                {
                    GetRandom.UpperCaseString(8),
                    GetRandom.UpperCaseString(5)
                },
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            Assert.DoesNotThrowAsync(() => sut.RemoveCouponsAsync(p));

            //Assert
            Container.Verify<IOvertureClient>();
        }

        private Mock<IOvertureClient> GetOvertureClientMock()
        {
            var mock = Container.GetMock<IOvertureClient>();

            mock.Setup(m => m.SendAsync(It.IsNotNull<RemoveCouponRequest>()))
                .ReturnsAsync(null)
                .Verifiable();

            return mock;
        }
    }
}

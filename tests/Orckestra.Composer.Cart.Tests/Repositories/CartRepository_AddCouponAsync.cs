using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Coupons;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class CartRepositoryAddCouponAsync
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
            CouponParam p = null;
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [TestCase("    ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_Scope_is_null_or_whitespace_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = scope
            };
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CustomerId_is_default_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(10)
            };
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [TestCase("     ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_CartName_is_null_or_whitespace_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = cartName,
                CouponCode = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [TestCase("     ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_CouponCode_is_null_or_whitespace_SHOULD_throw_ArgumentException(string couponCode)
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = couponCode,
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCouponAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public async Task WHEN_parameters_ok_SHOULD_invoke_Client()
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10)
            };
            var ovClientMock = GetClientMock();
            Container.Use(ovClientMock);
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var cart = await sut.AddCouponAsync(p);

            //Assert
            ovClientMock.Verify();
            cart.Should().NotBeNull();

            cart.Name.Should().Be(p.CartName);
            cart.Coupons.Should().NotBeNullOrEmpty();
            cart.Coupons.First().CouponCode.Should().Be(p.CouponCode);
            cart.CultureName.Should().Be(p.CultureInfo.Name);
            cart.CustomerId.Should().Be(p.CustomerId);
            cart.ScopeId.Should().Be(p.Scope);
        }

        private Mock<IOvertureClient> GetClientMock()
        {
            var mock = Container.GetMock<IOvertureClient>();

            mock.Setup(m => m.SendAsync(It.IsNotNull<AddCouponRequest>()))
                .Returns((AddCouponRequest r) => Task.FromResult(CartRepositoryFactory.CreateCartBasedOnAddCouponRequest(r, CouponState.Ok)))
                .Verifiable();

            return mock;
        }
    }
}

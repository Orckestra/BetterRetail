using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CouponViewServiceRemoveCouponAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_parameter_is_null_SHOULD_throw_NullArgumentException()
        {
            //Arrange
            CouponParam p = null;

            var sut = Container.CreateInstance<CouponViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await sut.RemoveCouponAsync(p));

            //Assert
            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [Test]
        public void WHEN_parameter_is_ok_SHOULD_return_cartViewModel()
        {
            //Arrange
            Container.Use(CartRepositoryFactory.Create());
            Container.Use(CartViewModelFactoryMock.Create());

            var p = new CouponParam()
            {
                CartName = GetRandom.String(7),
                CouponCode = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CouponViewService>();

            //Act
            var vm = sut.RemoveCouponAsync(p);

            //Assert
            vm.Should().NotBeNull();
        }
    }
}

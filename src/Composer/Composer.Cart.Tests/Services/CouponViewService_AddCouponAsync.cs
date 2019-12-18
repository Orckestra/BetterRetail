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
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Coupons;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CouponViewServiceAddCouponAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
            Container.Use(ViewModelMapperFactory.Create());
            Container.Use(LocalizationProviderFactory.Create());
            Container.Use(CartViewModelFactoryMock.Create());
        }

        [Test]
        public async Task WHEN_parameters_ok_SHOULD_call_repo_cleanup_invalid_coupons_and_add_msg()
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(6),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(5),
                BaseUrl = GetRandom.String(32)
            };

            Container.Use(CreateCartRepoMock(CartRepositoryFactory.CreateCartBasedOnAddCouponRequest, CouponState.Ok));

            var sut = Container.CreateInstance<CouponViewService>();

            //Act
            var vm = await sut.AddCouponAsync(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Coupons.Messages.Where(m => m.Level == CartMessageLevels.Success).Should().NotBeEmpty();
            Container.Verify<ICartRepository>();
        }

        [Test]
        public async Task WHEN_CouponCode_is_expired_SHOULD_call_repo_and_cleanup_invalid_coupons()
        {
            //Arrange
            var p = new CouponParam()
            {
                CartName = GetRandom.String(10),
                CouponCode = GetRandom.String(6),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(5),
                BaseUrl = GetRandom.String(32)
            };

            Container.Use(CreateCartRepoMock(CartRepositoryFactory.CreateCartBasedOnAddCouponRequest, CouponState.Expired));

            var sut = Container.CreateInstance<CouponViewService>();

            //Act
            var vm = await sut.AddCouponAsync(p);

            //Assert
            vm.Should().NotBeNull();
            Container.Verify<ICartRepository>();
        }

        private Mock<ICartRepository> CreateCartRepoMock(Func<AddCouponRequest, CouponState, ProcessedCart> cartCreator, CouponState couponState)
        {
            var mock = Container.GetMock<ICartRepository>();

            mock.Setup(m => m.AddCouponAsync(It.IsNotNull<CouponParam>()))
                .Returns((CouponParam p) => Task.FromResult(cartCreator.Invoke(MapParamToRequest(p), couponState)))
                .Verifiable();

            mock.Setup(m => m.RemoveCouponsAsync(It.IsNotNull<RemoveCouponsParam>()))
                .Returns(Task.FromResult(1))
                .Verifiable();

            return mock;
        }

        private AddCouponRequest MapParamToRequest(CouponParam parameters)
        {
            var request = new AddCouponRequest()
            {
                CartName = parameters.CartName,
                CouponCode = parameters.CouponCode,
                CultureName = parameters.CultureInfo.Name,
                CustomerId = parameters.CustomerId,
                ScopeId = parameters.Scope,
            };

            return request;
        }
    }
}

using System;
using System.Collections.Generic;
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
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class ShippingMethodViewServiceEstimateShippingAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            Container.Use(FulfillmentRepositoryMock.MockFulfillmentRepoWithRandomMethods());
            Container.Use(CartViewModelFactoryMock.MockGetShippingMethodViewModel());
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            EstimateShippingParam param = null;

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.EstimateShippingAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
        }

        [Test]
        public void WHEN_param_Cart_is_null_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            var param = new EstimateShippingParam
            {
                CultureInfo = CultureInfo.InvariantCulture,
                Cart = null,
                ForceUpdate = GetRandom.Boolean()
            };

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.EstimateShippingAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("Cart");
        }

        [Test]
        public void WHEN_param_CultureInfo_is_null_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            var param = new EstimateShippingParam
            {
                CultureInfo = null,
                Cart = new Overture.ServiceModel.Orders.Cart(),
                ForceUpdate = GetRandom.Boolean()
            };

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.EstimateShippingAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public async Task WHEN_NoFulfillmentMethod_SHOULD_choose_cheapest_shippingMethod()
        {
            //Arrange
            var cart = new Overture.ServiceModel.Orders.Cart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment()
                }
            };

            var methods =
                (await Container.Get<IFulfillmentMethodRepository>()
                    .GetCalculatedFulfillmentMethods(new GetShippingMethodsParam())).OrderBy(fm => fm.Cost);

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var vm = await sut.EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = CultureInfo.InvariantCulture,
                ForceUpdate = GetRandom.Boolean()
            });

            //Assert
            vm.Should().NotBeNull();
            vm.CostDouble.Should().Be(methods.First().Cost);
            cart.Shipments.First().FulfillmentMethod.Should().NotBeNull();

            Container.Verify((IFulfillmentMethodRepository repo) => repo.GetCalculatedFulfillmentMethods(It.IsNotNull<GetShippingMethodsParam>()));
        }

        [Test]
        public async Task WHEN_ForcingUpdate_SHOULD_choose_cheapest_shippingMethod()
        {
            //Arrange
            var cart = new Overture.ServiceModel.Orders.Cart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        FulfillmentMethod = new FulfillmentMethod
                        {
                            Id = GetRandom.Guid(),
                            Cost = GetRandom.Double(10.0, 35.0),
                            DisplayName = new LocalizedString()
                        }
                    }
                }
            };

            var methods =
                (await Container.Get<IFulfillmentMethodRepository>()
                    .GetCalculatedFulfillmentMethods(new GetShippingMethodsParam())).OrderBy(fm => fm.Cost);

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var vm = await sut.EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = CultureInfo.InvariantCulture,
                ForceUpdate = true
            });

            //Assert
            vm.Should().NotBeNull();
            vm.CostDouble.Should().Be(methods.First().Cost);
            cart.Shipments.First().FulfillmentMethod.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_not_forcing_SHOULD_keep_fulfillmentMethod()
        {
            //Arrange
            var fulfillmentMethod = new FulfillmentMethod
            {
                Id = GetRandom.Guid(),
                Cost = GetRandom.Double(10.0, 35.0),
                DisplayName = new LocalizedString(),
                Name = GetRandom.String(12)
            };

            var cart = new Overture.ServiceModel.Orders.Cart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        FulfillmentMethod = fulfillmentMethod
                    }
                }
            };

            var sut = Container.CreateInstance<ShippingMethodViewService>();

            //Act
            var vm = await sut.EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = CultureInfo.InvariantCulture,
                ForceUpdate = false
            });

            //Assert
            vm.Should().NotBeNull();
            cart.Shipments.First().FulfillmentMethod.Should().NotBeNull();
            cart.Shipments.First().FulfillmentMethod.Id.Should().Be(fulfillmentMethod.Id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class ShippingMethodViewService_SetCheapestShippingMethodAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        private void UseCartRepository()
        {
            var fakeCart = new ProcessedCart
            {
                CustomerId = Guid.NewGuid(),
                Shipments = new List<Shipment> { new Shipment() },
                BillingCurrency = GetRandom.String(1),
                CartType = GetRandom.String(1),
                Name = GetRandom.String(1),
                ScopeId = GetRandom.String(1),
                Status = GetRandom.String(1)
            };

            var cartRepositoryMock = new Mock<ICartRepository>();

            cartRepositoryMock.Setup(repo => repo.GetCartAsync(It.IsNotNull<GetCartParam>()))
                              .ReturnsAsync(fakeCart);

            _container.Use(cartRepositoryMock);
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_argument_null_exception()
        {
            //Arrange
            var cut = _container.CreateInstance<ShippingMethodViewService>();

            //Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => cut.SetCheapestShippingMethodAsync(null));
        }

        [Test]
        public void WHEN_no_fulfillment_method_SHOULD_throw_invalid_data_exception()
        {
            //Arrange
            UseCartRepository();

            var none = new List<FulfillmentMethod>();

            var fulfillmentMethodRepositoryMock = new Mock<IFulfillmentMethodRepository>();
            fulfillmentMethodRepositoryMock.Setup(r => r.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                                           .ReturnsAsync(none);

            _container.Use(fulfillmentMethodRepositoryMock);

            var cut = _container.CreateInstance<ShippingMethodViewService>();

            //Act and Assert
            Assert.ThrowsAsync<InvalidDataException>(() => cut.SetCheapestShippingMethodAsync(new SetCheapestShippingMethodParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(1)
            }));
        }

        [Test]
        public async Task WHEN_one_fulfillment_method_SHOULD_update_cart_using_that_method()
        {
            //Arrange
            UseCartRepository();

            var fulfillmentMethods = new List<FulfillmentMethod>
            {
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = 1
                }
            };

            var fulfillmentMethodRepositoryMock = new Mock<IFulfillmentMethodRepository>();
            fulfillmentMethodRepositoryMock.Setup(r => r.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                                           .ReturnsAsync(fulfillmentMethods);

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(service => service.UpdateCartAsync(It.Is<UpdateCartViewModelParam>(param => param.Shipments[0].FulfillmentMethod.Id.Equals(fulfillmentMethods[0].Id))))
                           .ReturnsAsync(It.IsAny<CartViewModel>())
                           .Verifiable();

            _container.Use(fulfillmentMethodRepositoryMock);
            _container.Use(cartServiceMock);

            var cut = _container.CreateInstance<ShippingMethodViewService>();

            //Act
            await cut.SetCheapestShippingMethodAsync(new SetCheapestShippingMethodParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(1)

            });

            //Assert
            cartServiceMock.Verify();
        }

        [Test]
        public async Task WHEN_many_fulfillment_methods_with_same_costs_SHOULD_update_cart_using_first_method()
        {
            //Arrange
            UseCartRepository();
            
            var fulfillmentMethods = new List<FulfillmentMethod>
            {
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = 1
                },
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = 1
                }
            };

            var fulfillmentMethodRepositoryMock = new Mock<IFulfillmentMethodRepository>();
            fulfillmentMethodRepositoryMock.Setup(r => r.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                                           .ReturnsAsync(fulfillmentMethods);

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(service => service.UpdateCartAsync(It.Is<UpdateCartViewModelParam>(param => param.Shipments[0].FulfillmentMethod.Id.Equals(fulfillmentMethods[0].Id))))
                           .ReturnsAsync(It.IsAny<CartViewModel>())
                           .Verifiable();

            _container.Use(fulfillmentMethodRepositoryMock);
            _container.Use(cartServiceMock);

            var cut = _container.CreateInstance<ShippingMethodViewService>();

            //Act
            await cut.SetCheapestShippingMethodAsync(new SetCheapestShippingMethodParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(1)

            });

            //Assert
            cartServiceMock.Verify();
        }

        [Test]
        public async Task WHEN_many_fulfillment_methods_with_different_costs_SHOULD_update_cart_using_method_with_lowest_cost()
        {
            //Arrange
            UseCartRepository();

            double lowerFulfillmentCost = 1;
            double higherFulfillmentCost = 2;
            
            var fulfillmentMethods = new List<FulfillmentMethod>
            {
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = lowerFulfillmentCost
                },
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = higherFulfillmentCost
                }
            };

            var fulfillmentMethodRepositoryMock = new Mock<IFulfillmentMethodRepository>();
            fulfillmentMethodRepositoryMock.Setup(r => r.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                                           .ReturnsAsync(fulfillmentMethods);

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(service => service.UpdateCartAsync(It.Is<UpdateCartViewModelParam>(param => param.Shipments[0].FulfillmentMethod.Cost.Equals(lowerFulfillmentCost))))
                           .ReturnsAsync(It.IsAny<CartViewModel>())
                           .Verifiable();

            _container.Use(fulfillmentMethodRepositoryMock);
            _container.Use(cartServiceMock);

            var cut = _container.CreateInstance<ShippingMethodViewService>();

            //Act
            await cut.SetCheapestShippingMethodAsync(new SetCheapestShippingMethodParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(1)

            });

            //Assert
            cartServiceMock.Verify();
        }
    }
}

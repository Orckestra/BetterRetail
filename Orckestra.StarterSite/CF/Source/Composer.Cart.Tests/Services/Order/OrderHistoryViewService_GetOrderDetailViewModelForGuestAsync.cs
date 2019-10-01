using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Services.Order
{
    public class OrderHistoryViewServiceGetOrderDetailViewModelForGuestAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOrderHistoryViewModelFactory>(MockBehavior.Strict));
            _container.Use(new Mock<IOrderRepository>(MockBehavior.Strict));
            _container.Use(new Mock<ILookupService>(MockBehavior.Strict));
            _container.Use(new Mock<IOrderUrlProvider>(MockBehavior.Strict));

            _container.GetMock<ILookupService>()
             .Setup(r => r.GetLookupDisplayNamesAsync(It.IsAny<GetLookupDisplayNamesParam>()))
             .ReturnsAsync(new Dictionary<string, string>());

            _container.GetMock<IOrderRepository>()
             .Setup(r => r.GetShipmentNotesAsync(It.IsAny<GetShipmentNotesParam>()))
             .ReturnsAsync(new List<Note>());

            _container.GetMock<IOrderRepository>()
             .Setup(r => r.GetOrderChangesAsync(It.IsAny<GetOrderChangesParam>()))
             .ReturnsAsync(new List<OrderHistoryItem>());

            _container.GetMock<IOrderDetailsViewModelFactory>()
            .Setup(r => r.CreateViewModel(It.IsAny<CreateOrderDetailViewModelParam>()))
            .Returns(new OrderDetailViewModel());

            _container.GetMock<IOrderUrlProvider>()
              .Setup(r => r.GetOrderDetailsBaseUrl(It.IsAny<CultureInfo>()))
               .Returns(GetRandom.String(32));

            _container.GetMock<ILineItemService>();
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var email = GetRandom.String(32);

            var orderHistoryViewModelService = _container.CreateInstance<OrderHistoryViewService>();

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderAsync(It.IsAny<GetOrderForGuestParam>()))
                .ReturnsAsync(new Overture.ServiceModel.Orders.Order
                {
                    Cart = new Overture.ServiceModel.Orders.Cart
                    {
                        Customer = new CustomerSummary
                        {
                            Email = email
                        }
                    }
                });

            //Act
            var param = new GetOrderForGuestParam
            {
                BaseUrl = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Email = email,
                OrderNumber = GetRandom.String(32),
            };
            var result =
                await orderHistoryViewModelService.GetOrderDetailViewModelForGuestAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_order_is_not_customer_one_SHOULD_return_null()
        {
            //Arrange
            var email = GetRandom.String(32);

            var orderHistoryViewModelService = _container.CreateInstance<OrderHistoryViewService>();

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderAsync(It.IsAny<GetOrderForGuestParam>()))
                .ReturnsAsync(new Overture.ServiceModel.Orders.Order
                {
                    Cart = new Overture.ServiceModel.Orders.Cart
                    {
                        Customer = new CustomerSummary
                        {
                            Email = GetRandom.String(32)
                        }
                    }
                });

            //Act
            var param = new GetOrderForGuestParam
            {
                BaseUrl = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Email = email,
                OrderNumber = GetRandom.String(32),
            };
            var result =
                await orderHistoryViewModelService.GetOrderDetailViewModelForGuestAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().BeNull();
        }
    }
}

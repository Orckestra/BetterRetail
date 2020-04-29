using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory.Order
{
    public class OrderDetailsViewModelFactoryCreateViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ILocalizationProvider>(MockBehavior.Strict));
            _container.Use(MockViewModelMapperFactory.Create(typeof(OrderDetailViewModel).Assembly));

            _container.GetMock<ILocalizationProvider>()
            .Setup(r => r.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
            .Returns(GetRandom.String(32));

            _container.GetMock<ICartViewModelFactory>()
            .Setup(r => r.GetOrderSummaryViewModel(It.IsAny<Overture.ServiceModel.Orders.Cart>(), It.IsAny<CultureInfo>()))
            .Returns(new OrderSummaryViewModel());

            _container.GetMock<ITaxViewModelFactory>()
            .Setup(r => r.CreateTaxViewModels(It.IsAny<List<Tax>>(), It.IsAny<CultureInfo>()))
            .Returns(new List<TaxViewModel>());
           
            _container.GetMock<ICartViewModelFactory>()
            .Setup(r => r.GetAddressViewModel(It.IsAny<Address>(), It.IsAny<CultureInfo>()))
            .Returns(new AddressViewModel());

            _container.GetMock<IPaymentProviderFactory>()
            .Setup(r => r.ResolveProvider(It.IsAny<string>()))
            .Returns(new FakePaymentProvider());

            _container.GetMock<IShippingTrackingProviderFactory>()
              .Setup(r => r.ResolveProvider(It.IsAny<string>()))
          .Returns(new FakeShippingTrackingProvider());
        }

        [Test]
        public void WHEN_param_is_correct_SHOULD_return_viewmodel()
        {
            //Arrange
            var factory = _container.CreateInstance<OrderDetailsViewModelFactory>();

            //Act
            var order = CreateOrder("InProgress", "Authorized");
            var vm = factory.CreateViewModel(CreateOrderDetailViewModelParam(order));

            //Assert
            vm.Should().NotBeNull();
            vm.OrderInfos.Should().NotBeNull();
            vm.OrderInfos.OrderNumber.ShouldAllBeEquivalentTo(order.OrderNumber);
            vm.OrderInfos.OrderStatus.ShouldAllBeEquivalentTo("In Progress");
            vm.OrderSummary.Should().NotBeNull();
            vm.Payments.Should().NotBeNull();
            vm.Payments.Count.ShouldBeEquivalentTo(2);
            vm.Shipments.Should().NotBeNull();
            vm.Shipments.Count.ShouldBeEquivalentTo(2);
            vm.ShippingAddress.Should().NotBeNull();
            vm.BillingAddress.Should().NotBeNull();
            vm.ShippingMethod.Should().NotBeNull();
        }

        [Test]
        public void WHEN_only_one_voided_payment_SHOULD_return_billing_address_and_payment()
        {
            //Arrange
           var factory = _container.CreateInstance<OrderDetailsViewModelFactory>();

            //Act
            var order = CreateOrder("Canceled", "Voided");
            var vm = factory.CreateViewModel(CreateOrderDetailViewModelParam(order));

            //Assert
            vm.Should().NotBeNull();
            vm.BillingAddress.Should().NotBeNull();
            vm.Payments.Count.ShouldBeEquivalentTo(2);
            vm.Payments.ForEach(payment => payment.BillingAddress.Should().NotBeNull());
        }

        protected CreateOrderDetailViewModelParam CreateOrderDetailViewModelParam(Overture.ServiceModel.Orders.Order order)
        {
            return new CreateOrderDetailViewModelParam
            {
                CultureInfo = new CultureInfo("en-US"),
                OrderStatuses = new Dictionary<string, string>
                {
                    {"InProgress", "In Progress"},
                    {"Canceled" , "Canceled"}
                },
                CountryCode = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Order = order,
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                ShipmentsNotes = new Dictionary<Guid, List<string>>(),
                OrderChanges = new List<OrderHistoryItem>(),
                ShipmentStatuses = new Dictionary<string, string>
                {
                    {"InProgress", "In Progress"},
                    {"Canceled" , "Canceled"}
                }
            };
        }

        protected Overture.ServiceModel.Orders.Order CreateOrder(string orderStatus, string paymentStatus)
        {
            return new Overture.ServiceModel.Orders.Order
            {
                OrderStatus = orderStatus,
                OrderNumber = GetRandom.String(32),
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Total = 100,
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            Address = new Address(),
                            FulfillmentMethod = new FulfillmentMethod
                            {
                                Cost = GetRandom.Double()
                            },
                            Taxes = new List<Tax>()
                        },
                        new Shipment
                        {
                            Address = new Address(),
                            FulfillmentMethod = new FulfillmentMethod
                            {
                                Cost = GetRandom.Double()
                            },
                            Taxes = new List<Tax>()
                        }
                    },
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            PaymentStatus = paymentStatus,
                            BillingAddress = new Address
                            {
                                AddressName = GetRandom.String(10),
                                City = GetRandom.String(15),
                                CountryCode = GetRandom.String(2),
                                Email = GetRandom.String(10),
                                FirstName = GetRandom.String(5),
                                LastName = GetRandom.String(5),
                                Id = GetRandom.Guid(),
                                IsPreferredBilling = true,
                                IsPreferredShipping = true,
                                Line1 = GetRandom.String(20),
                                PostalCode = GetRandom.String(6),
                                PhoneNumber = GetRandom.String(10),
                                RegionCode = GetRandom.String(3)
                            },
                            PaymentMethod = new PaymentMethod
                            {
                                PaymentProviderName = GetRandom.String(32)
                            }
                        },
                        new Payment
                        {
                            PaymentStatus = paymentStatus,
                            BillingAddress = new Address
                            {
                                AddressName = GetRandom.String(10),
                                City = GetRandom.String(15),
                                CountryCode = GetRandom.String(2),
                                Email = GetRandom.String(10),
                                FirstName = GetRandom.String(5),
                                LastName = GetRandom.String(5),
                                Id = GetRandom.Guid(),
                                IsPreferredBilling = true,
                                IsPreferredShipping = true,
                                Line1 = GetRandom.String(20),
                                PostalCode = GetRandom.String(6),
                                PhoneNumber = GetRandom.String(10),
                                RegionCode = GetRandom.String(3)
                            },
                            PaymentMethod = new PaymentMethod
                            {
                                PaymentProviderName = GetRandom.String(32)
                            }
                        }
                    }
                }
            };

        }

    }
}

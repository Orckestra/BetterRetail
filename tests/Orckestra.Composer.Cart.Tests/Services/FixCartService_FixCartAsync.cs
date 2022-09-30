using System;
using System.Collections.Generic;
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

using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Overture.ServiceModel.Orders;
namespace Orckestra.Composer.Cart.Tests.Services
{
    public class FixCartService_FixCartAsync
    {
        private AutoMocker _container;
        private readonly ProcessedCart _dummyCart = new ProcessedCart
        {
            Shipments = new List<Shipment>
            {
                new Shipment
                {
                    FulfillmentLocationId = GetRandom.Guid()
                }
            }
        };

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var inventoryProviderMock = _container.GetMock<IInventoryLocationProvider>();
            inventoryProviderMock.Setup(
                invProv => invProv.GetFulfillmentLocationAsync(It.IsNotNull<GetFulfillmentLocationParam>()))
                .ReturnsAsync(new FulfillmentLocation
                {
                    Id = GetRandom.Guid()
                });
        }

        [Test]
        public async Task WHEN_Cart_has_no_valid_fulfillmentLocationId_SHOULD_invoke_UpdateShipment()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var sut = _container.CreateInstance<FixCartService>();

            //Act
            await sut.FixCartAsync(new FixCartParam
            {
                Cart = new ProcessedCart
                {
                    CultureName = "en-US",
                    Payments = new List<Payment>
                    {
                        new Payment()
                    },
                    Shipments = new List<Shipment>
                    {
                      new Shipment()
                    }
                },
                ScopeId = GetRandom.String(10)
            });

            //Assert
            _container.Verify<ICartRepository>(r => r.UpdateShipmentAsync(It.IsNotNull<UpdateShipmentParam>()));
        }

        [Test]
        public async Task WHEN_Cart_already_has_fullfillmentLocationId_SHOULD_not_invoke_UpdateShipment()
        {
            //Arrange
            _container.Use(OvertureClientFactory.CreateMockWithValue(_dummyCart));
            var sut = _container.CreateInstance<FixCartService>();

            //Act
            var cart = await sut.FixCartAsync(new FixCartParam
            {
                Cart = new ProcessedCart
                {
                    Payments = new List<Payment>
                    {
                        new Payment()
                    },
                    Shipments = new List<Shipment>
                    {
                      new Shipment
                      {
                          FulfillmentLocationId = Guid.NewGuid()
                      }
                    }
                },
                ScopeId = GetRandom.String(10)
            });

            //Assert
            cart.Should().NotBeNull();
            _container.Verify<ICartRepository>(r => r.UpdateShipmentAsync(It.IsNotNull<UpdateShipmentParam>()), Times.Never());
        }
        [Test]
        public async Task WHEN_cart_has_no_payment_SHOULD_call_AddPaymentRequest()
        {
            //Arrange
            _container.GetMock<ICartRepository>().Setup(r => r.AddPaymentAsync(It.IsNotNull<AddPaymentParam>()))
                .ReturnsAsync(new ProcessedCart
                {
                    CultureName = "en-US",
                    Name = "With payment",
                    Payments = new List<Payment>
                    {
                        new Payment()
                    },
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            FulfillmentLocationId = GetRandom.Guid()
                        }
                    }
                })
                .Verifiable("No payment was added.");

            //_container.Use(ovMock);

            var sut = _container.CreateInstance<FixCartService>();

            //Act
            var cart = await sut.FixCartAsync(new FixCartParam
            {
                Cart = new ProcessedCart
                {
                    CultureName = "en-US",
                    Name = "No payment cart",
                    Payments = new List<Payment>(),
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            FulfillmentLocationId = GetRandom.Guid()
                        }
                    }
                },
                ScopeId = GetRandom.String(10)
            });

            //Assert
            cart.Should().NotBeNull();
            cart.Name.Should().BeEquivalentTo("With payment");
            _container.Verify<ICartRepository>(ov => ov.AddPaymentAsync(It.IsNotNull<AddPaymentParam>()));

        }

        [Test]
        public async Task WHEN_cart_has_invalid_payment_SHOULD_call_AddPaymentRequest()
        {
            //Arrange
            _container.GetMock<ICartRepository>().Setup(r => r.AddPaymentAsync(It.IsNotNull<AddPaymentParam>()))
                .ReturnsAsync(new ProcessedCart
                {
                    CultureName = "en-US",
                    Name = "With payment",
                    Payments = new List<Payment>
                    {
                        new Payment()
                    },
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            FulfillmentLocationId = GetRandom.Guid()
                        }
                    },
                    ScopeId = GetRandom.String(10)
                })
                .Verifiable("No payment was added.");

            var sut = _container.CreateInstance<FixCartService>();

            //Act
            var cart = await sut.FixCartAsync(new FixCartParam
            {
                Cart = new ProcessedCart
                {
                    CultureName = "en-US",
                    Name = "No payment cart",
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            PaymentStatus = PaymentStatus.Voided
                        }
                    },
                    Shipments = new List<Shipment>
                    {
                      new Shipment
                      {
                          FulfillmentLocationId = Guid.NewGuid()
                      }
                    }
                },
                ScopeId = GetRandom.String(10)
            });

            //Assert
            cart.Should().NotBeNull();
            cart.Name.Should().BeEquivalentTo("With payment");
            _container.Verify<ICartRepository>(ov => ov.AddPaymentAsync(It.IsNotNull<AddPaymentParam>()));

        }

        [Test]
        public async Task WHEN_cart_has_payment_SHOULD_not_call_AddPayment()
        {
            //Arrange
            _container.GetMock<ICartRepository>().Setup(r => r.AddPaymentAsync(It.IsNotNull<AddPaymentParam>()))
                .ThrowsAsync(new InvalidOperationException("This method should NOT be called."));

            var sut = _container.CreateInstance<FixCartService>();

            //Act
            var cart = await sut.FixCartAsync(new FixCartParam
            {
                Cart = new ProcessedCart
                {
                    CultureName = "en-US",
                    Name = "Payment cart from AddLineItem",
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            PaymentStatus = PaymentStatus.New
                        }
                    },
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            FulfillmentLocationId = GetRandom.Guid()
                        }
                    }
                },
                ScopeId = GetRandom.String(10)
            });

            //Assert
            cart.Should().NotBeNull();
            cart.Name.Should().BeEquivalentTo("Payment cart from AddLineItem");
        }

    }
}

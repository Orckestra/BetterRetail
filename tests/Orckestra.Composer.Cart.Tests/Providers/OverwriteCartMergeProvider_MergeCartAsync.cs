using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.CartMerge;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class OverwriteCartMergeProvider_MergeCartAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {

            _container = new AutoMocker();
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            var guestCustomerId = Guid.NewGuid();
            var loggedCustomerId = Guid.NewGuid();
            var guestCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>
                        {
                            new LineItem
                            {
                                ProductId = "P1",
                                Quantity = 1
                            }
                        }
                    }
                }
            };
            var loggedCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>
                        {
                            new LineItem
                            {
                                ProductId = "P2",
                                Quantity = 1
                            },
                        }
                    }
                }
            };

            var cartRepositoryMock = new Mock<ICartRepository>();

            cartRepositoryMock
                .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == guestCustomerId)))
                .ReturnsAsync(guestCart);

            cartRepositoryMock
               .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == loggedCustomerId)))
               .ReturnsAsync(loggedCart);
            _container.Use(cartRepositoryMock);

            var provider = _container.CreateInstance<OverwriteCartMergeProvider>();

            //Act
            var param = new CartMergeParam
            {
                GuestCustomerId = guestCustomerId,
                LoggedCustomerId = loggedCustomerId,
                Scope = GetRandom.String(32)
            };
            await provider.MergeCartAsync(param).ConfigureAwait(false);

            //Assert
            cartRepositoryMock.Verify(c => c.UpdateCartAsync(It.Is<UpdateCartParam>(p => VerifyMergedCart(p))), Times.Once);
        }

        private bool VerifyMergedCart(UpdateCartParam param)
        {
            param.Shipments.First().LineItems.Count.ShouldBeEquivalentTo(1);

            var lineItem1 = param.Shipments.First().LineItems.Find(x => x.ProductId == "P1");
            lineItem1.Should().NotBeNull();
            lineItem1.Quantity.ShouldBeEquivalentTo(1);

            return true;
        }

        [Test]
        public async Task WHEN_guest_cart_contains_no_lineitem_SHOULD_do_nothing()
        {
            //Arrange
            var guestCustomerId = Guid.NewGuid();
            var loggedCustomerId = Guid.NewGuid();
            var guestCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>()
                    }
                }
            };
            var loggedCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>()
                    }
                }
            };

            var cartRepositoryMock = new Mock<ICartRepository>();

            cartRepositoryMock
                .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == guestCustomerId)))
                .ReturnsAsync(guestCart);

            cartRepositoryMock
               .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == loggedCustomerId)))
               .ReturnsAsync(loggedCart);
            _container.Use(cartRepositoryMock);

            var provider = _container.CreateInstance<OverwriteCartMergeProvider>();

            //Act
            var param = new CartMergeParam
            {
                GuestCustomerId = guestCustomerId,
                LoggedCustomerId = loggedCustomerId,
                Scope = GetRandom.String(32)
            };

            await provider.MergeCartAsync(param).ConfigureAwait(false);

            //Assert
            cartRepositoryMock.Verify(c => c.UpdateCartAsync(It.IsAny<UpdateCartParam>()), Times.Never);
        }

        [Test]
        public async Task WHEN_customerId_are_the_same_SHOULD_do_nothing()
        {
            var guestId = Guid.NewGuid();
            var loggedId = guestId;
            var guestCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>()
                    }
                }
            };
            var loggedCart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>()
                    }
                }
            };

            var cartRepositoryMock = _container.GetMock<ICartRepository>();
            cartRepositoryMock
                .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == guestId)))
                .ReturnsAsync(guestCart);

            cartRepositoryMock
               .Setup(c => c.GetCartAsync(It.Is<GetCartParam>(x => x.CustomerId == loggedId)))
               .ReturnsAsync(loggedCart);

            var provider = _container.CreateInstance<OverwriteCartMergeProvider>();

            //Act
            var param = new CartMergeParam
            {
                GuestCustomerId = guestId,
                LoggedCustomerId = loggedId,
                Scope = GetRandom.String(32)
            };

            await provider.MergeCartAsync(param).ConfigureAwait(false);

            //Assert
            cartRepositoryMock.Verify(c => c.UpdateCartAsync(It.IsAny<UpdateCartParam>()), Times.Never);
        }
    }
}

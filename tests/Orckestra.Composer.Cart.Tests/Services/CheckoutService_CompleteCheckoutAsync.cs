using System;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using Orckestra.Composer.Cart.ViewModels;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CheckoutServiceCompleteCheckoutAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();

            _container.Use(ViewModelMapperFactory.Create());
            _container.Use(CartViewModelFactoryMock.Create());
    
            var cartRepoMock = _container.GetMock<ICartRepository>();
            cartRepoMock.Setup(repo => repo.CompleteCheckoutAsync(It.IsNotNull<CompleteCheckoutParam>()))
                .Returns((CompleteCheckoutParam p) =>
                {
                    var order = new Overture.ServiceModel.Orders.Order
                    {
                        Cart = new ProcessedCart()
                        {
                            Customer = new CustomerSummary()
                            {
                                Email = GetRandom.Email()
                            },
                            Shipments = new System.Collections.Generic.List<Shipment>()
                            {
                                new Shipment
                                {
                                    Id = GetRandom.Guid(),
                                    LineItems = new System.Collections.Generic.List<LineItem>()
                                    {
                                        new LineItem
                                        {
                                            Id = GetRandom.Guid(),
                                            Sku = GetRandom.String(10),
                                            CatalogId = GetRandom.String(10),                                            
                                            PlacedPrice = GetRandom.Decimal(),
                                            PlacedQuantity = GetRandom.Int(),
                                            Status = GetRandom.String(5),
                                            Total = GetRandom.Decimal(),
                                            KvaValues = new Overture.ServiceModel.PropertyBag(),
                                            KvaDisplayValues = new Overture.ServiceModel.PropertyBag(),
                                            ProductSummary = new CartProductSummary
                                            {
                                                Brand = null,
                                                DisplayName = GetRandom.String(10),
                                                PrimaryParentCategoryId = GetRandom.String(10)
                                            }
                                        }
                                    },
                                    FulfillmentMethod = new FulfillmentMethod
                                    {
                                        FulfillmentMethodType = FulfillmentMethodType.PickUp
                                    }
                                }
                            }
                        },
                        OrderNumber = GetRandom.String(12)
                    };

                    return Task.FromResult(order);
                });
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CheckoutService>();

            // Act
            var result = await service.CompleteCheckoutAsync(new CompleteCheckoutParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
            });

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Param_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<CheckoutService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.CompleteCheckoutAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            var service = _container.CreateInstance<CheckoutService>();
            var param = new CompleteCheckoutParam
            {
                Scope = scope,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
            };

            // Act
            Expression<Func<Task<CompleteCheckoutViewModel>>> expression = () => service.CompleteCheckoutAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CheckoutService>();
            var param = new CompleteCheckoutParam
            {
                Scope = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.CompleteCheckoutAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_Is_GuidEmpty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CheckoutService>();
            var param = new CompleteCheckoutParam
            {
                Scope = GetRandom.String(10),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.CompleteCheckoutAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CustomerId");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            var service = _container.CreateInstance<CheckoutService>();
            var param = new CompleteCheckoutParam
            {
                Scope = GetRandom.String(10),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = cartName,
            };

            // Act
            Expression<Func<Task<CompleteCheckoutViewModel>>> expression = () => service.CompleteCheckoutAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }
    }
}

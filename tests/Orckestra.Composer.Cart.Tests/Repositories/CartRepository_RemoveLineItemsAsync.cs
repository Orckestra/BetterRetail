using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.LineItems;
using static Orckestra.Composer.Utils.ExpressionUtility;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class CartRepository_RemoveLineItemsAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_passing_null_parameter_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemoveLineItemsAsync(null));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("\t\r\n")]
        [TestCase("  ")]
        public void WHEN_passing_invalid_scope_SHOULD_throw_ArgumentException_with_details(string scope)
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.String(7)
                    }
                },
                Scope = scope
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveLineItemsAsync(p));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
            exception.Message.Should().ContainEquivalentOf("scope");
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("\t\r\n")]
        [TestCase("  ")]
        public void WHEN_passing_invalid_cartName_SHOULD_throw_ArgumentException_with_details(string cartName)
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = cartName,
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.String(7)
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveLineItemsAsync(p));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
            exception.Message.Should().ContainEquivalentOf("cartName");
        }

        [Test]
        public void WHEN_null_cultureInfo_SHOULD_throw_ArgumentException_with_details()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.String(7)
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveLineItemsAsync(p));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
            exception.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public void WHEN_empty_customerId_SHOULD_throw_ArgumentException_with_details()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.String(7)
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveLineItemsAsync(p));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
            exception.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_null_LineItems_SHOULD_throw_ArgumentException_with_details()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = null,
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveLineItemsAsync(p));

            //Assert
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
            exception.Message.Should().ContainEquivalentOf("LineItems");
        }
        
        [Test]
        public void WHEN_LineItems_Id_is_empty_guid_SHOULD_throw_ArgumentException_with_details()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.String(7)
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.RemoveLineItemsAsync(p);
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => expression.Compile().Invoke());

            //Assert
            exception.Message.Should().ContainEquivalentOf("Line item with index ");
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t\r\n")]
        public void WHEN_LineItems_Id_is_empty_guid_SHOULD_throw_ArgumentException_with_details(string productId)
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = productId
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            Expression<Func<Task<ProcessedCart>>> expression = () => sut.RemoveLineItemsAsync(p);
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => expression.Compile().Invoke());

            //Assert
            exception.Message.Should().ContainEquivalentOf("Line item with index ");
        }

        [Test]
        public async Task WHEN_all_parameters_ok_SHOULD_invoke_OvertureClient_SendAsync_with_AddOrUpdateRequest()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.Boolean() ? GetRandom.String(4) : null
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var vm = await sut.RemoveLineItemsAsync(p);

            //Assert
            Container.Verify<IOvertureClient>(m => m.SendAsync(It.IsNotNull<AddOrUpdateLineItemsRequest>()));
        }

        [Test]
        public async Task WHEN_all_parameters_ok_SHOULD_set_cache_value_with_result()
        {
            //Arrange
            var p = new RemoveLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = ForTests.TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                LineItems = new List<RemoveLineItemsParam.LineItemDescriptor>
                {
                    new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = GetRandom.Guid(),
                        ProductId = GetRandom.String(7),
                        VariantId = GetRandom.Boolean() ? GetRandom.String(4) : null
                    }
                },
                Scope = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var vm = await sut.RemoveLineItemsAsync(p);

            //Assert
            //3.8 upgrade
            Container.Verify<ICacheProvider>(m => m.SetAsync(It.IsNotNull<CacheKey>(), It.IsAny<ProcessedCart>(), It.IsAny<CacheKey>()));
        }
    }
}

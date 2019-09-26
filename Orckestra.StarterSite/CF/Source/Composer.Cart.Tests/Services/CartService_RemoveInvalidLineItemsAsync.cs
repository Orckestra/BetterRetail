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
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CartService_RemoveInvalidLineItemsAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            Container.GetMock<ICartRepository>()
                .Setup(m => m.GetCartAsync(It.IsNotNull<GetCartParam>()))
                .ReturnsAsync(new ProcessedCart());
        }

        [Test]
        public void WHEN_param_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = Container.CreateInstance<CartService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemoveInvalidLineItemsAsync(null));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task WHEN_param_ok_SHOULD_invoke_CartRepository_GetCart()
        {
            //Arrange
            var p = new RemoveInvalidLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(7),
                BaseUrl = GetRandom.WwwUrl()
            };

            MockLineItemService(GetRandom.Boolean());

            Container.GetMock<ICartRepository>().Setup(repository => repository.RemoveLineItemsAsync(It.IsAny<RemoveLineItemsParam>())).ReturnsAsync(new ProcessedCart()).Verifiable();

            var sut = Container.CreateInstance<CartService>();

            //Act
            await sut.RemoveInvalidLineItemsAsync(p);

            //Assert
            Container.Verify<ICartRepository>(m => m.GetCartAsync(It.IsNotNull<GetCartParam>()));
        }

        [Test]
        public async Task WHEN_param_ok_SHOULD_invoke_LineItemService_GetInvalidLineItems()
        {
            //Arrange
            var p = new RemoveInvalidLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(7),
                BaseUrl = GetRandom.WwwUrl()
            };

            MockLineItemService(GetRandom.Boolean());

            Container.GetMock<ICartRepository>().Setup(repository => repository.RemoveLineItemsAsync(It.IsAny<RemoveLineItemsParam>())).ReturnsAsync(new ProcessedCart()).Verifiable();

            var sut = Container.CreateInstance<CartService>();

            //Act
            await sut.RemoveInvalidLineItemsAsync(p);

            //Assert
            Container.Verify<ILineItemService>(m => m.GetInvalidLineItems(It.IsNotNull<ProcessedCart>()));
        }

        [Test]
        public async Task WHEN_has_no_invalid_lineItems_SHOULD_not_call_CartRepository_RemoveLineItemsAsync()
        {
            //Arrange
            var p = new RemoveInvalidLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(7),
                BaseUrl = GetRandom.WwwUrl()
            };

            MockLineItemService(false);

            Container.GetMock<ICartRepository>().Setup(repository => repository.RemoveLineItemsAsync(It.IsAny<RemoveLineItemsParam>())).ReturnsAsync(new ProcessedCart()).Verifiable();

            var sut = Container.CreateInstance<CartService>();

            //Act
            await sut.RemoveInvalidLineItemsAsync(p);

            //Assert
            Container.Verify<ICartRepository>(m => m.RemoveLineItemsAsync(It.IsAny<RemoveLineItemsParam>()), Times.Never());
        }
        
        [Test]
        public async Task WHEN_has_invalid_lineItems_SHOULD_call_CartRepository_RemoveLineItemsAsync()
        {
            //Arrange
            var p = new RemoveInvalidLineItemsParam
            {
                CartName = GetRandom.String(7),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(7),
                BaseUrl = GetRandom.WwwUrl()
            };

            MockLineItemService(true);

            Container.GetMock<ICartRepository>().Setup(repository => repository.RemoveLineItemsAsync(It.IsAny<RemoveLineItemsParam>())).ReturnsAsync(new ProcessedCart()).Verifiable();

            var sut = Container.CreateInstance<CartService>();

            //Act
            await sut.RemoveInvalidLineItemsAsync(p);

            //Assert
            Container.GetMock<ICartRepository>().Verify();
        }

        private void MockLineItemService(bool hasInvalidLineItems)
        {
            var invalidLineItems = new List<LineItem>();

            if (hasInvalidLineItems)
            {
                invalidLineItems.Add(new LineItem());
                invalidLineItems.Add(new LineItem());
            }

            Container.GetMock<ILineItemService>().Setup(m => m.GetInvalidLineItems(It.IsNotNull<ProcessedCart>()))
                .Returns(invalidLineItems);
        }
    }
}

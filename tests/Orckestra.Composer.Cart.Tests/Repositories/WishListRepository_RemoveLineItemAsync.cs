using System;
using System.Threading.Tasks;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    public class WishListRepository_RemoveLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<WishListRepository>();

            // Act
            var result = await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                LineItemId = GetRandom.Guid()
            });

            // Assert
            result.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("         ")]
        [TestCase("\r\n\t")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new RemoveLineItemParam
            {
                Scope = scope,
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                LineItemId = Guid.NewGuid()
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.RemoveLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new RemoveLineItemParam
            {
                Scope = "Canada",
                CultureInfo = null,
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                LineItemId = Guid.NewGuid()
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.RemoveLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CultureInfo)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("         ")]
        [TestCase("\r\n\t")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new RemoveLineItemParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = cartName,
                LineItemId = Guid.NewGuid()
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.RemoveLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new RemoveLineItemParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.Empty,
                CartName = "WishList",
                LineItemId = Guid.NewGuid()
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.RemoveLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CustomerId)));
        }

        public void WHEN_LineId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new RemoveLineItemParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                LineItemId = Guid.Empty
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.RemoveLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.LineItemId)));
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<WishListRepository>();

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => cartRepository.RemoveLineItemAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }
    }
}

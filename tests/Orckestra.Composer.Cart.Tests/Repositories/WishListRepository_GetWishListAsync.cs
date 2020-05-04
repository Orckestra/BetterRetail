using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class WishListRepository_GetWishListAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
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
            var param = new GetCartParam
            {
                Scope = scope,
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.GetWishListAsync(param);
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
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = null,
                CustomerId = Guid.NewGuid(),
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.GetWishListAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.Empty,
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.GetWishListAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
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
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = cartName
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.GetWishListAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetWishListAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}

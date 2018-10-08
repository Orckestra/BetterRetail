using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;

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

        [TestCase(null, "en-CA", "WishList", "randomCustomerId", "param.Scope")]
        [TestCase("", "en-CA", "WishList", "randomCustomerId", "param.Scope")]
        [TestCase("Canada", null, "WishList", "randomCustomerId", "param.CultureInfo")]
        [TestCase("Canada", "en-CA", "WishList", null, "param.CustomerId")]
        [TestCase("Canada", "en-CA", null, "randomCustomerId", "param.CartName")]
        [TestCase("Canada", "en-CA", "", "randomCustomerId", "param.CartName")]
        public void WHEN_Param_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope, string cultureName,
            string cartName, string customerId, string paramName)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await repository.GetWishListAsync(new GetCartParam
                {
                    Scope = scope,
                    CultureInfo =
                        string.IsNullOrWhiteSpace(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName),
                    CustomerId = string.IsNullOrWhiteSpace(customerId) ? Guid.Empty : GetRandom.Guid(),
                    CartName = cartName
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain(paramName);
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(async () =>
            {
                await repository.GetWishListAsync(null);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}

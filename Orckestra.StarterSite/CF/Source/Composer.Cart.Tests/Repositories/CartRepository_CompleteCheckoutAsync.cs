using System;
using System.Globalization;
using System.Threading.Tasks;
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
    public class CartRepositoryCompleteCheckoutAsync
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            CompleteCheckoutParam p = null;
            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.CompleteCheckoutAsync(p));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [TestCase("    ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_Scope_is_null_or_whitespace_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var p = new CompleteCheckoutParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = scope,
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.CompleteCheckoutAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
            ex.Message.Should().ContainEquivalentOf("Scope");
        }

        [Test]
        public void WHEN_CustomerId_is_default_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var p = new CompleteCheckoutParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(10),
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.CompleteCheckoutAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [TestCase("     ")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_CartName_is_null_or_whitespace_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var p = new CompleteCheckoutParam()
            {
                CartName = cartName,
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(10),
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.CompleteCheckoutAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
            ex.Message.Should().Contain("CartName");
        }

        [Test]
        public void WHEN_CultureInfo_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var p = new CompleteCheckoutParam()
            {
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            };

            var sut = Container.CreateInstance<CartRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.CompleteCheckoutAsync(p));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            Container.Use(OvertureClientFactory.Create());
            var repository = Container.CreateInstance<CartRepository>();

            //Act
            var result = await repository.CompleteCheckoutAsync(new CompleteCheckoutParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            });

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_Dependencies_Return_Null_Values_SHOULD_Succeed()
        {
            //Arrange
            Container.Use(OvertureClientFactory.CreateWithNullValues());
            var repository = Container.CreateInstance<CartRepository>();

            //Act
            var result = await repository.CompleteCheckoutAsync(new CompleteCheckoutParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.CurrentCulture,
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(10),
            });

            // Assert
            result.Should().NotBeNull();
        }
    }
}

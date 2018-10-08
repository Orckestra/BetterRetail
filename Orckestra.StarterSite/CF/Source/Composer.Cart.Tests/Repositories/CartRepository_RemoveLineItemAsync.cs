using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    public class CartRepositoryRemoveLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();
        }

        [Test]
        public async void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

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

        [Test]
        public async void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.CreateWithNullValues());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var result = await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                LineItemId          = GetRandom.Guid()
            });

            // Assert
            result.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope       = scope,
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId  = GetRandom.Guid(),
                    CartName    = GetRandom.String(32),
                    LineItemId = GetRandom.Guid()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.Scope");
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope       = GetRandom.String(32),
                    CultureInfo = null,
                    CustomerId  = GetRandom.Guid(),
                    CartName    = GetRandom.String(32),
                    LineItemId          = GetRandom.Guid()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope       = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId  = Guid.Empty,
                    CartName    = GetRandom.String(32),
                    LineItemId          = GetRandom.Guid()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CustomerId");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope       = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId  = GetRandom.Guid(),
                    CartName    = cartName,
                    LineItemId          = GetRandom.Guid()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CartName");
        }

        [Test]
        public void WHEN_Id_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope       = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId  = GetRandom.Guid(),
                    CartName    = GetRandom.String(32),
                    LineItemId          = Guid.Empty
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.LineItemId");
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(null);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}

using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.ForTests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.ViewModels;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Services
{
    public class CartServiceRetrieveACartAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();

            _container.Use(ViewModelMapperFactory.Create());
            _container.Use(CartRepositoryFactory.Create());
            _container.Use(CartViewModelFactoryMock.Create());
            _container.Use(CountryServiceMock.Create());

            var localizationProviderMock = new Mock<ILocalizationProvider>();
            localizationProviderMock
                .Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns("{0}");

            _container.Use(localizationProviderMock);
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.GetCartViewModelAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();
            _container.Use(CartRepositoryFactory.CreateWithNullValues());

            // Act
            var result = service.GetCartViewModelAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new GetCartParam
            {
                Scope = scope,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.GetCartViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.GetCartViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.GetCartViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = cartName,
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.GetCartViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [Test]
        public void WHEN_ExecuteWorkflow_Is_Null_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.GetCartViewModelAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = null,
                WorkflowToExecute = GetRandom.String(32),
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }


        [Test]
        public void WHEN_WorkflowToExecute_Is_Null_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.GetCartViewModelAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = null,
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.GetCartViewModelAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
        }
    }
}

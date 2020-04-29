using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    public class CartRepositoryRetrieveACartAsync
    {
        private AutoMocker _container;
        private CartRepository _repository;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();
            _repository = _container.CreateInstance<CartRepository>();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<ProcessedCart>>>(),
                    It.IsAny<Func<ProcessedCart, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<ProcessedCart>>, Func<ProcessedCart, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();

            var overtureClient = _container.GetMock<IOvertureClient>();
            var dummyCart = new ProcessedCart();

            overtureClient
                .Setup(client => client.SendAsync(
                    It.IsNotNull<GetCartRequest>()))
                .ReturnsAsync(dummyCart)
                .Verifiable();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Act
            var result = _repository.GetCartAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            // Act
            var result = _repository.GetCartAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            var param = new GetCartParam
            {
                Scope = scope,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => _repository.GetCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => _repository.GetCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => _repository.GetCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            var param = new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = cartName,
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => _repository.GetCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [Test]
        public void WHEN_ExecuteWorkflow_Is_Null_SHOULD_Succeed()
        {
            // Act
            var result = _repository.GetCartAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = null,
                WorkflowToExecute = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [Test]
        public void WHEN_WorkflowToExecute_Is_Null_SHOULD_Succeed()
        {
            // Act
            var result = _repository.GetCartAsync(new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ExecuteWorkflow = GetRandom.Boolean(),
                WorkflowToExecute = null,
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetCartAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}

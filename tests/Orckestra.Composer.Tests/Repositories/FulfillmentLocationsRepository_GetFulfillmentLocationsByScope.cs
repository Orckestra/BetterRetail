using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Tests.Repositories
{
    [TestFixture]
    public class FulfillmentLocationsRepository_GetFulfillmentLocationsByScope
    {
        private AutoMocker _container;
        private FulfillmentLocationsRepository _sut;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var overtureClient = _container.GetMock<IOvertureClient>();
            overtureClient.Setup(ovClient => ovClient.SendAsync(
                    It.IsNotNull<GetFulfillmentLocationsByScopeRequest>()))
                .ReturnsAsync(new List<FulfillmentLocation>())
                .Verifiable();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<List<FulfillmentLocation>>>>(),
                    It.IsAny<Func<List<FulfillmentLocation>, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<List<FulfillmentLocation>>>,
                        Func<List<FulfillmentLocation>, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();

            _sut = _container.CreateInstance<FulfillmentLocationsRepository>();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            GetFulfillmentLocationsByScopeParam p = null;

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetFulfillmentLocationsByScopeAsync(p));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.ShouldBeEquivalentTo("param");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("   ")]
        [TestCase("\r\n")]
        public void WHEN_scope_is_invalid_SHOULD_Throw_ArgumentException(string scope)
        {
            //Arrange
            var p = new GetFulfillmentLocationsByScopeParam
            {
                IncludeChildScopes = GetRandom.Boolean(),
                IncludeSchedules = GetRandom.Boolean(),
                Scope = scope
            };

            //Act
            Expression<Func<Task<List<FulfillmentLocation>>>> expression = () => _sut.GetFulfillmentLocationsByScopeAsync(p);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(p.Scope)));
        }

        [Test]
        public async Task WHEN_param_ok_SHOULD_call_OvertureClient()
        {
            //Arrange
            var p = new GetFulfillmentLocationsByScopeParam
            {
                IncludeChildScopes = GetRandom.Boolean(),
                IncludeSchedules = GetRandom.Boolean(),
                Scope = GetRandom.String(6)
            };

            //Act
            var results = await _sut.GetFulfillmentLocationsByScopeAsync(p);

            //Assert
            _container.Verify<IOvertureClient>(
                ovClient => ovClient.SendAsync(It.IsNotNull<GetFulfillmentLocationsByScopeRequest>()));
        }
        
        [Test]
        public async Task WHEN_param_ok_SHOULD_return_Overture_result()
        {
            //Arrange
            var p = new GetFulfillmentLocationsByScopeParam
            {
                IncludeChildScopes = GetRandom.Boolean(),
                IncludeSchedules = GetRandom.Boolean(),
                Scope = GetRandom.String(6)
            };

            //Act
            var results = await _sut.GetFulfillmentLocationsByScopeAsync(p);

            //Assert
            results.Should().NotBeNull();
        }
        
        [Test]
        public async Task WHEN_param_ok_SHOULD_cache_result()
        {
            //Arrange
            var p = new GetFulfillmentLocationsByScopeParam
            {
                IncludeChildScopes = GetRandom.Boolean(),
                IncludeSchedules = GetRandom.Boolean(),
                Scope = GetRandom.String(6)
            };

            //Act
            var results = await _sut.GetFulfillmentLocationsByScopeAsync(p);

            //3.8 upgrade
            //Assert
            _container.Verify<ICacheProvider>(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<List<FulfillmentLocation>>>>(),
                    It.IsAny<Func<List<FulfillmentLocation>, Task>>(),
                    It.IsAny<CacheKey>()));
        }
    }
}

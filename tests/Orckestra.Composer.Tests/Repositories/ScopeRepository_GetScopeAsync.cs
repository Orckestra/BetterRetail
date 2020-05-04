using System;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Tests.Repositories
{
    [TestFixture]
    public class ScopeRepository_GetScopeAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<Scope>>>(),
                    It.IsAny<Func<Scope, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<Scope>>, Func<Scope, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException_with_param_name()
        {
            //Arrange
            GetScopeParam param = null;
            var sut = _container.CreateInstance<ScopeRepository>();

            //Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetScopeAsync(param));

            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("             ")]
        [TestCase("\r\n\t")]
        public void WHEN_scope_is_invalid_SHOULD_throw_ArgumentException_with_param_name(string scope)
        {
            //Arrange
            var param = new GetScopeParam
            {
                Scope = scope
            };

            var sut = _container.CreateInstance<ScopeRepository>();

            //Act & Assert
            Expression<Func<Task<Scope>>> expression = () => sut.GetScopeAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public async Task WHEN_request_is_ok_SHOULD_invoke_OvertureClient()
        {
            //Arrange
            var param = new GetScopeParam
            {
                Scope = GetRandom.String(7)
            };

            MockOvertureClient(param.Scope);
            var sut = _container.CreateInstance<ScopeRepository>();

            //Act
            var vm = await sut.GetScopeAsync(param);

            //Assert
            _container.GetMock<IOvertureClient>().VerifyAll();
        }

        [Test]
        public void WHEN_request_is_ok_SHOULD_invoke_OvertureClient_with_valid_request()
        {
            //Arrange
            var param = new GetScopeParam
            {
                Scope = GetRandom.String(7)
            };

            MockOvertureClient(param.Scope);
            var sut = _container.CreateInstance<ScopeRepository>();

            //Act & Assert
            Assert.DoesNotThrowAsync(() => sut.GetScopeAsync(param));
        }

        [Test]
        public async Task WHEN_request_is_ok_SHOULD_return_OvertureClient_value()
        {
            //Arrange
            var param = new GetScopeParam
            {
                Scope = GetRandom.String(7)
            };

            var response = new Scope
            {
                Id = Guid.NewGuid().ToString()
            };

            MockOvertureClient(param.Scope, response);
            var sut = _container.CreateInstance<ScopeRepository>();

            //Act
            var res = await sut.GetScopeAsync(param);

            //Assert
            res.Should().Be(response);
        }

        [Test]
        [Ignore("")]//todo: add a nice comment
        //todo: create a functional / integration test
        public async Task WHEN_requested_many_times_SHOULD_invoke_OvertureClient_once()
        {
            //Arrange
            var param = new GetScopeParam
            {
                Scope = GetRandom.String(7)
            };

            MockOvertureClient(param.Scope);
            var sut = _container.CreateInstance<ScopeRepository>();

            //Act
            await sut.GetScopeAsync(param);
            await sut.GetScopeAsync(param);
            await sut.GetScopeAsync(param);
            await sut.GetScopeAsync(param);
            await sut.GetScopeAsync(param);

            //Assert
            _container.GetMock<IOvertureClient>().Verify(client => client.SendAsync(It.IsNotNull<GetScopeRequest>()), Times.AtMostOnce);
        }

        private void MockOvertureClient(string scope, Scope response = null)
        {
            var mock = _container.GetMock<IOvertureClient>();

            mock.Setup(client => client.SendAsync(It.IsNotNull<GetScopeRequest>()))
                .Returns((GetScopeRequest request) =>
                {
                    if (request.ScopeId != scope)
                    {
                        throw new ArgumentException("Scope passed to the OvertureClient does not match the one that was requested");
                    }

                    if (request.CultureName != null)
                    {
                        throw new ArgumentException("Culture Name should be null in order to retrieve all supported cultures.");
                    }

                    if (!request.IncludeCurrency)
                    {
                        throw new ArgumentException("IncludeCurrency should be true.");
                    }

                    if (request.IncludeChildren)
                    {
                        throw new ArgumentException("Children of the scope should not be included.");
                    }

                    return Task.FromResult<Scope>(response);
                })
                .Verifiable($"Did not call Overture with a {nameof(GetScopeRequest)}.");
        }
    }
}

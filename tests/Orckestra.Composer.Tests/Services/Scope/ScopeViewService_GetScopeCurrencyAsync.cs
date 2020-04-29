using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Tests.Services.Scope
{
    [TestFixture]
    public class ScopeViewService_GetScopeCurrencyAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException_with_param_name()
        {
            //Arrange
            GetScopeCurrencyParam param = null;
            var sut = _container.CreateInstance<ScopeViewService>();

            //Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetScopeCurrencyAsync(param));

            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("         ")]
        [TestCase("\r\n\t")]
        public void WHEN_scope_is_invalid_SHOULD_throw_ArgumentException_with_paramName(string scope)
        {
            //Arrange
            GetScopeCurrencyParam param = new GetScopeCurrencyParam
            {
                Scope = scope,
                CultureInfo = CultureInfo.InvariantCulture
            };
            var sut = _container.CreateInstance<ScopeViewService>();

            //Act & Assert
            Expression<Func<Task<CurrencyViewModel>>> expression = () => sut.GetScopeCurrencyAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_cultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = null
            };
            var sut = _container.CreateInstance<ScopeViewService>();

            //Act & Assert
            Expression<Func<Task<CurrencyViewModel>>> expression = () => sut.GetScopeCurrencyAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public async Task WHEN_param_is_ok_SHOULD_invoke_scopeRepository()
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = CultureInfo.InvariantCulture
            };
            var sut = _container.CreateInstance<ScopeViewService>();

            //Act
            var vm = await sut.GetScopeCurrencyAsync(param);

            //Assert
            _container.GetMock<IScopeRepository>().Verify(scopeRepo => scopeRepo.GetScopeAsync(It.IsNotNull<GetScopeParam>()));
        }

        [Test]
        public void WHEN_param_is_ok_SHOULD_invoke_ScopeRepo_with_scope_in_param()
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = CultureInfo.InvariantCulture
            };

            _container.GetMock<IScopeRepository>().Setup(scopeRepo => scopeRepo.GetScopeAsync(It.IsNotNull<GetScopeParam>()))
                .Returns((GetScopeParam req) =>
                {
                    if (req.Scope != param.Scope)
                    {
                        throw new ArgumentException("Scope in param is not passed to the repository.");
                    }

                    return Task.FromResult<Overture.ServiceModel.Scope>(null);
                });

            var sut = _container.CreateInstance<ScopeViewService>();

            //Act & Assert
            Assert.DoesNotThrowAsync(() => sut.GetScopeCurrencyAsync(param));
        }

        [Test]
        public async Task WHEN_param_is_ok_and_scope_has_currency_SHOULD_invoke_ViewModelMapper()
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = CultureInfo.InvariantCulture
            };

            _container.GetMock<IViewModelMapper>()
                .Setup(vmm => vmm.MapTo<CurrencyViewModel>(It.IsAny<object>(), It.IsNotNull<CultureInfo>()))
                .Verifiable();

            MockScopeRepository();

            var sut = _container.CreateInstance<ScopeViewService>();

            //Act
            var vm = await sut.GetScopeCurrencyAsync(param);

            //Assert
            _container.Verify<IViewModelMapper>();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task WHEN_scope_is_null_or_has_no_currency_SHOULD_return_null(bool nullCurrency)
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = CultureInfo.InvariantCulture
            };

            if (nullCurrency)
            {
                _container.GetMock<IScopeRepository>()
                    .Setup(repo => repo.GetScopeAsync(It.IsAny<GetScopeParam>()))
                    .ReturnsAsync(new Overture.ServiceModel.Scope
                    {
                        Currency = null
                    });
            }
            else
            {
                _container.GetMock<IScopeRepository>()
                    .Setup(repo => repo.GetScopeAsync(It.IsAny<GetScopeParam>()))
                    .ReturnsAsync(null);
            }

            var sut = _container.CreateInstance<ScopeViewService>();

            //Act
            var vm = await sut.GetScopeCurrencyAsync(param);

            //Assert
            vm.Should().BeNull();
        }

        [Test]
        public async Task WHEN_scope_has_currency_SHOULD_return_viewModel_from_mapper()
        {
            //Arrange
            var param = new GetScopeCurrencyParam
            {
                Scope = GetRandom.String(7),
                CultureInfo = CultureInfo.InvariantCulture
            };

            var result = new CurrencyViewModel();

            MockScopeRepository();
            _container.GetMock<IViewModelMapper>()
                .Setup(vmm => vmm.MapTo<CurrencyViewModel>(It.IsAny<object>(), It.IsNotNull<CultureInfo>()))
                .Returns(result);

            var sut = _container.CreateInstance<ScopeViewService>();

            //Act
            var vm = await sut.GetScopeCurrencyAsync(param);

            //Assert
            vm.Should().Be(result);
        }

        private void MockScopeRepository()
        {
            var mock = _container.GetMock<IScopeRepository>();

            mock.Setup(scopeRepo => scopeRepo.GetScopeAsync(It.IsNotNull<GetScopeParam>()))
                .ReturnsAsync(new Overture.ServiceModel.Scope
                {
                    Currency = new Currency()
                });
        }
    }
}

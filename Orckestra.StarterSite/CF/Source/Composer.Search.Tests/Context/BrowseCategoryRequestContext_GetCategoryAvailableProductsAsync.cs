using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Search.Tests.Context
{
    [TestFixture]
    public class BrowseCategoryRequestContextGetCategoryAvailableProductsAsync
    {
        private AutoMocker _container = new AutoMocker();
        private Mock<HttpRequestBase> _requestMock;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _requestMock = new Mock<HttpRequestBase>();
            _requestMock.Setup(q => q.Url).Returns(new Uri("https://google.com"));
            _requestMock.Setup(q => q.ApplicationPath).Returns(@"x:\");
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetCategoryAvailableProductsAsync(null));

            //Assert
            exception.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public async Task WHEN_param_is_ok_SHOULD_fill_criteria_context_from_ComposerContext()
        {
            //Arrange
            var scope = GetRandom.String(6);
            var cultureInfo = CultureInfo.InvariantCulture;

            ArrangeComposerContext(scope, cultureInfo);

            var param = CreateEmptyParam();
            GetCategoryBrowsingViewModelParam passedParam = null;

            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();
            var catBrowsingMock = _container.GetMock<ICategoryBrowsingViewService>();
            catBrowsingMock
                .Setup(q => q.GetCategoryBrowsingViewModelAsync(It.IsAny<GetCategoryBrowsingViewModelParam>()))
                .Callback((GetCategoryBrowsingViewModelParam p) => passedParam = p)
                .ReturnsAsync(new CategoryBrowsingViewModel());

            //Act
            await sut.GetCategoryAvailableProductsAsync(param);

            //Assert
            passedParam.CultureInfo.Should().NotBeNull();
            passedParam.CultureInfo.Should().Be(cultureInfo);

            _container.Verify<IComposerRequestContext>();
        }

        [Test]
        public async Task WHEN_param_is_ok_and_is_first_call_SHOULD_model_is_not_null()
        {
            //Arrange
            ArrangeCategoryBrowsingViewService();

            var param = CreateEmptyParam();
            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();

            //Act
            var vm = await sut.GetCategoryAvailableProductsAsync(param);

            //Assert
            vm.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_param_is_ok_and_is_first_call_SHOULD_mocked_service_invoked_once()
        {
            //Arrange
            Mock<ICategoryBrowsingViewService> mock = ArrangeCategoryBrowsingViewService();

            var param = CreateEmptyParam();
            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();

            //Act
            await sut.GetCategoryAvailableProductsAsync(param);

            //Assert
            mock.Verify(service => service.GetCategoryBrowsingViewModelAsync(It.IsAny<GetCategoryBrowsingViewModelParam>()), Times.Once);
        }

        [Test]
        public async Task WHEN_sut_is_called_twice_SHOULD_return_same_vm_instance()
        {
            //Arrange
            ArrangeCategoryBrowsingViewService();

            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();

            //Act
            var firstVm = await sut.GetCategoryAvailableProductsAsync(CreateEmptyParam());
            var secondVm = await sut.GetCategoryAvailableProductsAsync(CreateEmptyParam());

            //Assert
            secondVm.Should().BeSameAs(firstVm, "the browsing request context should return the same instance when called many times");
        }

        [Test]
        public async Task WHEN_sut_is_called_twice_SHOULD_mocked_service_invoked_once()
        {
            //Arrange
            Mock<ICategoryBrowsingViewService> mock = ArrangeCategoryBrowsingViewService();

            var sut = _container.CreateInstance<BrowseCategoryRequestContext>();

            //Act
            await sut.GetCategoryAvailableProductsAsync(CreateEmptyParam());
            await sut.GetCategoryAvailableProductsAsync(CreateEmptyParam());

            //Assert
            mock.Verify(service => service.GetCategoryBrowsingViewModelAsync(It.IsAny<GetCategoryBrowsingViewModelParam>()), Times.Once);
        }

        private void ArrangeComposerContext(string scope, CultureInfo cultureInfo)
        {
            var mock = _container.GetMock<IComposerRequestContext>();

            mock.SetupGet(ci => ci.Scope).Returns(scope).Verifiable();
            mock.SetupGet(ci => ci.CultureInfo).Returns(cultureInfo).Verifiable();

            _container.Use(mock);
        }

        private Mock<ICategoryBrowsingViewService> ArrangeCategoryBrowsingViewService()
        {
            var mock = _container.GetMock<ICategoryBrowsingViewService>();

            mock.Setup(vs => vs.GetCategoryBrowsingViewModelAsync(It.IsAny<GetCategoryBrowsingViewModelParam>()))
                .ReturnsAsync(new CategoryBrowsingViewModel())
                .Verifiable();

            _container.Use(mock);

            return mock;
        }

        private GetBrowseCategoryParam CreateEmptyParam()
        {
            return new GetBrowseCategoryParam
            {
                Request = _requestMock.Object,
            };
        }
    }
}

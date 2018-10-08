using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductBreadcrumbServiceCreateBreadcrumbAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_Parameters_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            var service = _container.CreateInstance<ProductBreadcrumbService>();

            //Act
            var func = new Func<Task<BreadcrumbViewModel>> (() => service.CreateBreadcrumbAsync(null));

            //Assert
            func.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            var service = _container.CreateInstance<ProductBreadcrumbService>();
            var parameters = new GetProductBreadcrumbParam()
            {
                CategoryId = GetRandom.String(10),
                CultureInfo = null,
                HomeUrl = null,
                ProductName = GetRandom.String(10),
                Scope = GetRandom.String(8)
            };

            //Act
            var func = new Func<Task<BreadcrumbViewModel>>(() => service.CreateBreadcrumbAsync(parameters));

            //Assert
            func.ShouldThrow<ArgumentException>();
        }

        [TestCase("", "", "")]
        [TestCase(null, "", "")]
        [TestCase("abc", "abcde", null)]
        public void WHEN_String_Parameters_Are_Invalid_SHOULD_Throw_ArgumentException(string productName, string categoryId, string scope)
        {
            //Arrange
            var service = _container.CreateInstance<ProductBreadcrumbService>();
            var parameters = new GetProductBreadcrumbParam()
            {
                CategoryId = categoryId,
                CultureInfo = CultureInfo.InvariantCulture,
                HomeUrl = GetRandom.String(25),
                ProductName = productName,
                Scope = scope
            };

            //Act
            var func = new Func<Task<BreadcrumbViewModel>>(() => service.CreateBreadcrumbAsync(parameters));

            //Assert
            func.ShouldThrow<ArgumentException>();
        }

        [Test]
        public async void WHEN_Parameters_Are_Valid_SHOULD_Return_ViewModel()
        {
            //Arrange
            var categoryServiceMock = CreateCategoryServiceMock();
            var localizationProvider = _container.GetMock<ILocalizationProvider>();
            var urlProviderMock = _container.GetMock<ICategoryBrowsingUrlProvider>();

            localizationProvider.Setup(
                lp => lp.GetLocalizedString(It.Is<GetLocalizedParam>(param => param.Key == "L_Home")))
                .Returns("Home")
                .Verifiable();

            urlProviderMock.Setup(up => up.BuildCategoryBrowsingUrl(It.IsNotNull<BuildCategoryBrowsingUrlParam>()))
                .Returns(new Func<BuildCategoryBrowsingUrlParam, string>(p => GetRandom.Url()))
                .Verifiable();

            _container.Use(categoryServiceMock);
            _container.Use(localizationProvider);
            _container.Use(urlProviderMock);

            var service = _container.CreateInstance<ProductBreadcrumbService>();
            var productName = GetRandom.String(10);
            var homeUrl = GetRandom.Url();

            var parameters = new GetProductBreadcrumbParam()
            {
                CategoryId = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                HomeUrl = homeUrl,
                ProductName = productName,
                Scope = GetRandom.String(7)
            };

            //Act
            var vm = await service.CreateBreadcrumbAsync(parameters);

            //Assert
            vm.Should().NotBeNull();
            vm.Items.Should().HaveCount(3);
            vm.Items[0].DisplayName.Should().Be("Home");
            vm.Items[0].Url.Should().Be(homeUrl);
            vm.Items[1].DisplayName.Should().Be("B");
            vm.Items[1].Url.Should().NotBeNullOrWhiteSpace();
            vm.Items[2].DisplayName.Should().Be("A");
            vm.Items[2].Url.Should().NotBeNullOrWhiteSpace();
            vm.ActivePageName.Should().Be(productName);

            categoryServiceMock.Verify();
            localizationProvider.Verify();
            urlProviderMock.Verify();
        }

        [Test]
        public async void WHEN_Product_Without_A_Category_SHOULD_Return_Minimal_Breadcrumb()
        {
            // Arrange
            var service = _container.CreateInstance<ProductBreadcrumbService>();
            var homeUrl = GetRandom.Url();
            var productName = GetRandom.String(12);

            var parameters = new GetProductBreadcrumbParam()
            {
                CategoryId = null,
                CultureInfo = CultureInfo.InvariantCulture,
                HomeUrl = homeUrl,
                ProductName = productName,
                Scope = GetRandom.String(7)
            };

            // Act
            var vm = await service.CreateBreadcrumbAsync(parameters);

            // Assert
            vm.Should().NotBeNull();
            vm.Items.Should().HaveCount(1);
            vm.Items[0].Url.Should().Be(homeUrl);
            vm.ActivePageName.Should().Be(productName);
        }

        private Mock<ICategoryViewService> CreateCategoryServiceMock()
        {
            var mock = _container.GetMock<ICategoryViewService>();

            mock.Setup(service => service.GetCategoriesPathAsync(It.IsAny<GetCategoriesPathParam>()))
                .ReturnsAsync(new[]
                {
                    new CategoryViewModel { Id = "A", DisplayName = "A" }, 
                    new CategoryViewModel { Id = "B", DisplayName = "B" }, 
                    new CategoryViewModel { Id = "Root" }
                })
                .Verifiable();

            return mock;
        }
    }
}

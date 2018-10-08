using System;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;

namespace Orckestra.Composer.Search.Tests.Service
{
    [TestFixture]
    public class SearchBreadcrumViewServiceCreateBreadcrumbViewModel
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            var localizationProviderMock = Container.GetMock<ILocalizationProvider>();
            localizationProviderMock.Setup(m => m.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns<GetLocalizedParam>(param => GetRandom.String(10))
                .Verifiable();

            Container.Use(localizationProviderMock);
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            GetSearchBreadcrumbParam p = null;

            var sut = Container.CreateInstance<SearchBreadcrumbViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => sut.CreateBreadcrumbViewModel(p));

            //Assert
            ex.ParamName.Should().BeEquivalentTo("param");
        }

        [Test]
        public void WHEN_cultureInfo_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var p = new GetSearchBreadcrumbParam()
            {
                CultureInfo = null,
                HomeUrl = GetRandom.WwwUrl(),
                Keywords = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<SearchBreadcrumbViewService>();

            //Act
            var ex = Assert.Throws<ArgumentException>(() => sut.CreateBreadcrumbViewModel(p));
        }

        [Test]
        public void WHEN_parameters_ok_SHOULD_return_valid_viewModel()
        {
            //Arrange
            var p = new GetSearchBreadcrumbParam()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                HomeUrl = GetRandom.WwwUrl(),
                Keywords = GetRandom.String(7)
            };

            var sut = Container.CreateInstance<SearchBreadcrumbViewService>();

            //Act
            var vm = sut.CreateBreadcrumbViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Items.Should().NotBeNullOrEmpty();

            var homeItem = vm.Items.First();
            homeItem.Url.Should().Be(p.HomeUrl);
            homeItem.DisplayName.Should().NotBeNullOrWhiteSpace();

            vm.ActivePageName.Should().NotBeNullOrWhiteSpace();
        }
    }
}

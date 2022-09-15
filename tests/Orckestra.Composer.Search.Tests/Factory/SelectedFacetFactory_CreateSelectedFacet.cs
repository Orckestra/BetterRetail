using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Dependency;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Providers.SelectedFacet;
using Orckestra.Composer.Search.Tests.Mock;
using Orckestra.Overture;

namespace Orckestra.Composer.Search.Tests.Factory
{
    [TestFixture]
    public class SelectedFacetFactory_CreateSelectedFacet
    {
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use((ISelectedFacetProviderRegistry)new SelectedFacetProviderRegistry());
            SetupFacets();
        }

        private const string CultureName = "en-CA";
        private AutoMocker _container;

        private SearchFilter MockSearchFilter()
        {
            var filter = new SearchFilter { Name = GetRandom.String(5), Value = "value1|value2" };
            return filter;
        }

        [Test]
        public void WHEN_facet_is_not_configured_in_SearchConfiguration_FacetSetting_SHOULD_return_empty_list()
        {
            //Arrange
            var registry = _container.Get<ISelectedFacetProviderRegistry>();
            var providerName = GetRandom.String(15);
            registry.RegisterProvider(providerName, typeof(FakeSelectedFacetProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(typeof(FakeSelectedFacetProvider)))
                .Returns(new FakeSelectedFacetProvider());

            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var filter = MockSearchFilter();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var result = sut.CreateSelectedFacet(filter, cultureInfo);

            //Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void WHEN_filter_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => sut.CreateSelectedFacet(null, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_filter_name_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var filter = new SearchFilter { Name = string.Empty };
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateSelectedFacet(filter, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_filter_name_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var filter = new SearchFilter { Name = null };
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateSelectedFacet(filter, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_known_facet_and_provider_name_SHOULD_retrieve_instance()
        {
            //Arrange
            var registry = _container.Get<ISelectedFacetProviderRegistry>();
            var facetType = FacetType.Range;
            var providerName = facetType.ToString();
            registry.RegisterProvider(providerName, typeof(FakeSelectedFacetProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeSelectedFacetProvider());

            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var filter = MockSearchFilter();
            var cultureInfo = new CultureInfo(CultureName);
            SetupFacets(new FacetSetting(filter.Name) {FacetType = facetType});

            //Act
            var result = sut.CreateSelectedFacet(filter, cultureInfo);

            //Assert
            result.Should().HaveCount(1);
        }

        [Test]
        public void WHEN_registry_is_empty_SHOULD_throw_InvalidOperationException()
        {
            //Arrange
            var sut = _container.CreateInstance<SelectedFacetFactory>();
            var facetType = FacetType.Range;
            var filter = MockSearchFilter();
            var cultureInfo = new CultureInfo(CultureName);
            SetupFacets(new FacetSetting(filter.Name) {FacetType = facetType});

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => sut.CreateSelectedFacet(filter, cultureInfo));

            //Assert
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_unknow_provider_name_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var registry = _container.Get<ISelectedFacetProviderRegistry>();
            registry.RegisterProvider<FakeSelectedFacetProvider>(GetRandom.String(15));
            var facetType = FacetType.Range;
            var filter = MockSearchFilter();
            var cultureInfo = new CultureInfo(CultureName);
            var sut = _container.CreateInstance<SelectedFacetFactory>();
            SetupFacets(new FacetSetting(filter.Name) {FacetType = facetType});

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.CreateSelectedFacet(filter, cultureInfo));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().BeEquivalentTo("name");
        }

        private void SetupFacets(params FacetSetting[] settings)
        {
            _container.GetMock<IFacetConfigurationContext>()
                .Setup(x => x.GetFacetSettings())
                .Returns(new List<FacetSetting>(settings));
        }
    }
}
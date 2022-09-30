using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Dependency;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Search.Tests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Search;
using FacetType = Orckestra.Composer.Search.Facets.FacetType;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Tests.Factory
{
    [TestFixture]
    public class FacetFactory_CreateFacet
    {
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use((IFacetProviderRegistry) new FacetProviderRegistry());
            SetupFacets();
        }

        private const string CultureName = "en-CA";
        private AutoMocker _container;

        [Test]
        public void WHEN_facet_is_not_configured_in_SearchConfiguration_FacetSetting_SHOULD_return_Null()
        {
            //Arrange
            var registry = _container.Get<IFacetProviderRegistry>();
            var providerName = GetRandom.String(15);
            registry.RegisterProvider(providerName, typeof (FakeFacetProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeFacetProvider());

            var sut = _container.CreateInstance<FacetFactory>();
            var facet = new Facet {FieldName = GetRandom.String(5)};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var result = sut.CreateFacet(facet, selectedFacets, cultureInfo);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void WHEN_facet_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetFactory>();
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception =
                Assert.Throws<ArgumentNullException>(() => sut.CreateFacet(null, selectedFacets, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("facet");
        }

        [Test]
        public void WHEN_facet_name_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetFactory>();
            var facet = new Facet {FieldName = string.Empty};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateFacet(facet, selectedFacets, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("facet");
        }

        [Test]
        public void WHEN_facet_name_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetFactory>();
            var facet = new Facet {FieldName = null};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateFacet(facet, selectedFacets, cultureInfo));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("facet");
        }

        [Test]
        public void WHEN_known_facet_and_provider_name_SHOULD_retrieve_instance()
        {
            //Arrange
            var registry = _container.Get<IFacetProviderRegistry>();
            var facetType = FacetType.Range;
            var providerName = facetType.ToString();
            registry.RegisterProvider(providerName, typeof (FakeFacetProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeFacetProvider());

            var sut = _container.CreateInstance<FacetFactory>();
            var facet = new Facet {FieldName = GetRandom.String(5)};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);
            SetupFacets(new FacetSetting(facet.FieldName) {FacetType = facetType});

            //Act
            var result = sut.CreateFacet(facet, selectedFacets, cultureInfo);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_registry_is_empty_SHOULD_throw_InvalidOperationException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetFactory>();
            var facetType = FacetType.Range;
            var facet = new Facet {FieldName = GetRandom.String(5)};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);
            SetupFacets(new FacetSetting(facet.FieldName) {FacetType = facetType});

            //Act
            var exception =
                Assert.Throws<InvalidOperationException>(() => sut.CreateFacet(facet, selectedFacets, cultureInfo));

            //Assert
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_unknow_provider_name_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var registry = _container.Get<IFacetProviderRegistry>();
            registry.RegisterProvider<FakeFacetProvider>(GetRandom.String(15));
            var facetType = FacetType.Range;
            var facet = new Facet {FieldName = GetRandom.String(5)};
            var selectedFacets = new List<SearchFilter>();
            var cultureInfo = new CultureInfo(CultureName);
            SetupFacets(new FacetSetting(facet.FieldName) {FacetType = facetType});

            var sut = _container.CreateInstance<FacetFactory>();

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.CreateFacet(facet, selectedFacets, cultureInfo));

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
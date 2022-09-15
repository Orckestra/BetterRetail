using System;
using System.Collections.Generic;
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
using Orckestra.Composer.Search.Providers.FacetPredicate;
using Orckestra.Composer.Search.Tests.Mock;
using Orckestra.Overture;

namespace Orckestra.Composer.Search.Tests.Factory
{
    [TestFixture]
    public class FacetPredicateFactory_CreateFacetPredicate
    {
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use((IFacetPredicateProviderRegistry) new FacetPredicateProviderRegistry());
            SetupFacets();
        }

        private AutoMocker _container;

        private SearchFilter MockSearchFilter()
        {
            var filter = new SearchFilter {Name = GetRandom.String(5), Value = "value1|value2"};
            return filter;
        }

        [Test]
        public void WHEN_facet_is_not_configured_in_SearchConfiguration_FacetSetting_SHOULD_return_Null()
        {
            //Arrange
            var registry = _container.Get<IFacetPredicateProviderRegistry>();
            var providerName = GetRandom.String(15);
            registry.RegisterProvider(providerName, typeof (FakeFacetPredicateProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeFacetPredicateProvider());
            var filter = MockSearchFilter();

            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var predicate = sut.CreateFacetPredicate(filter);

            //Assert
            predicate.Should().BeNull();
        }

        [Test]
        public void WHEN_filter_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => sut.CreateFacetPredicate(null));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_filter_name_is_empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateFacetPredicate(new SearchFilter {Name = string.Empty}));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_filter_name_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var exception =
                Assert.Throws<ArgumentException>(() => sut.CreateFacetPredicate(new SearchFilter {Name = null}));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("filter");
        }

        [Test]
        public void WHEN_known_facet_and_provider_name_SHOULD_retrieve_instance()
        {
            //Arrange
            var registry = _container.Get<IFacetPredicateProviderRegistry>();
            var facetType = FacetType.Range;
            var providerName = facetType.ToString();
            registry.RegisterProvider(providerName, typeof (FakeFacetPredicateProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeFacetPredicateProvider());
            var filter = MockSearchFilter();
            SetupFacets(new FacetSetting(filter.Name) {FacetType = facetType});

            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var predicate = sut.CreateFacetPredicate(filter);

            //Assert
            predicate.Should().NotBeNull();
        }

        [Test]
        public void WHEN_registry_is_empty_SHOULD_throw_InvalidOperationException()
        {
            //Arrange
            var sut = _container.CreateInstance<FacetPredicateFactory>();
            var filter = MockSearchFilter();
            SetupFacets(new FacetSetting(filter.Name) {FacetType = FacetType.Range});

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => sut.CreateFacetPredicate(filter));

            //Assert
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_unknow_provider_name_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var registry = _container.Get<IFacetPredicateProviderRegistry>();
            registry.RegisterProvider<FakeFacetPredicateProvider>(GetRandom.String(15));
            var filter = MockSearchFilter();
            SetupFacets(new FacetSetting(filter.Name) {FacetType = FacetType.Range});

            var sut = _container.CreateInstance<FacetPredicateFactory>();

            //Act
            var exception = Assert.Throws<ArgumentException>(() => sut.CreateFacetPredicate(filter));

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
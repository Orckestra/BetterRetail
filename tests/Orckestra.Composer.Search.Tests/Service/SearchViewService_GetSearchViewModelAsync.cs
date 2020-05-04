using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;
using Facet = Orckestra.Overture.ServiceModel.Search.Facet;
using FacetType = Orckestra.Composer.Search.Facets.FacetType;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Tests.Service
{
    [TestFixture]
    public class SearchViewService_GetSearchViewModelAsync
    {
        private const string CultureName = "en-CA";
        private const string Scope = "global";
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            SetUpSearchgUrlProvider();
            SetUpComposerContext();
            SetUpDamProvider();
            SetUpLocalizationProvider();
            SetUpProductUrlProvider();
            SetUpSearchRepository();
            SetUpViewModelMapper();
            SetUpSelectedFacetFactory();
        }

        private void SetUpSelectedFacetFactory()
        {
            var mock = new Mock<ISelectedFacetFactory>();
            mock.Setup(factory => factory.CreateSelectedFacet(It.IsAny<SearchFilter>(), It.IsAny<CultureInfo>()))
                 .Returns((SearchFilter searchFilter, CultureInfo cultureInfo) => new List<SelectedFacet> { new SelectedFacet { FieldName = searchFilter.Name } });
            _container.Use(mock);
        }

        private void SetUpSearchRepository()
        {
            var results = new ProductSearchResult
            {
                Facets = new List<Facet>(),
                Documents = new List<ProductDocument>()
            };

            var mock = new Mock<ISearchRepository>();
            mock.Setup(repository => repository.SearchProductAsync(It.IsAny<SearchCriteria>())).ReturnsAsync(results);

            _container.Use(mock);
        }

        private void SetUpViewModelMapper()
        {
            var mock = new Mock<IViewModelMapper>();

            _container.Use(mock);
        }

        private void SetUpDamProvider()
        {
            var mock = new Mock<IDamProvider>();

            _container.Use(mock);
        }

        private void SetUpLocalizationProvider()
        {
            var mock = new Mock<ILocalizationProvider>();

            _container.Use(mock);
        }

        private void SetUpProductUrlProvider()
        {
            var mock = new Mock<IProductUrlProvider>();

            _container.Use(mock);
        }

        private void SetUpComposerContext()
        {
            var mock = new Mock<IComposerContext>();
            mock.Setup(context => context.CultureInfo).Returns(new CultureInfo(CultureName));
            mock.Setup(context => context.Scope).Returns("global");

            _container.Use(mock);
        }

        private void SetUpSearchgUrlProvider()
        {
            var mock = new Mock<ISearchUrlProvider>();

            _container.Use(mock);
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_argument_null_exception()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.GetSearchViewModelAsync(null));
        }

        [Test]
        public async Task WHEN_keywords_is_null_SHOULD_return_no_product()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();
            
            // Act 
            var model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = null,
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope
            });

            //Assert
            model.ProductSearchResults.TotalCount.Should().Be(0);
        }

        [Test]
        public async Task WHEN_keywords_is_empty_SHOULD_return_no_product()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act
            var model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = string.Empty,
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope
            });

            //Assert
            model.ProductSearchResults.TotalCount.Should().Be(0);
        }

        [Test]
        public void WHEN_culture_info_is_null_SHOULD_throw_argument_exception()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetSearchViewModelAsync(
                new SearchCriteria
                {
                    Keywords = "any",
                    CultureInfo = null,
                    Scope = "global"
                }
            ));
        }

        [Test]
        public void WHEN_scope_is_null_SHOULD_throw_argument_exception()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetSearchViewModelAsync(
                new SearchCriteria
                {
                    Keywords = "any",
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = null
                }
            ));
        }

        [Test]
        public void WHEN_scope_is_empty_SHOULD_throw_argument_exception()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetSearchViewModelAsync(
                new SearchCriteria
                {
                    Keywords = "any",
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = string.Empty
                }
            ));
        }

        [Test]
        public async Task WHEN_keywords_is_not_null_or_whitespace_SHOULD_returned_model_contain_keywords()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act
            SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = "any",
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope
            });

            // Assert
            model.Keywords.Should().Be("any");
        }

        [Test]
        public async Task WHEN_no_selected_facet_SHOULD_returned_model_contain_no_facet()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act
            SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = "any",
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                SelectedFacets = { }
            });

            // Assert
            model.SelectedFacets.Facets.Count.Should().Be(0);
        }

        [Test]
        public async Task WHEN_one_selected_facet_SHOULD_facets_count_should_be_1()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();
            var facetName = GetRandom.String(5);
            SetupFacets(new FacetSetting(facetName));

            // Act
            SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = "any",
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                SelectedFacets =
                {
                    new Composer.Parameters.SearchFilter{Name = facetName}
                }
            });

            // Assert
            model.SelectedFacets.Facets.Count.Should().Be(1);
        }

        [Test]
        public async Task WHEN_one_selected_facet_SHOULD_facets_are_not_all_removable()
        {
            // Arrange
            SearchViewService service = _container.CreateInstance<SearchViewService>();

            // Act
            SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
            {
                Keywords = "any",
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                SelectedFacets =
                {
                    new Composer.Parameters.SearchFilter()
                }
            });

            // Assert
            model.SelectedFacets.IsAllRemovable.Should().BeFalse();
        }

        //[Test]
        //public async Task WHEN_two_selected_facets_SHOULD_facets_are_all_removable()
        //{
        //    // Arrange
        //    SearchViewService service = _container.CreateInstance<SearchViewService>();
        //    var facetName1 = GetRandom.String(5);
        //    var facetName2 = GetRandom.String(5);
        //    SearchConfiguration.FacetSettings.Add(new FacetSetting(facetName1));
        //    SearchConfiguration.FacetSettings.Add(new FacetSetting(facetName2));

        //    // Act
        //    SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
        //    {
        //        Keywords = "any",
        //        CultureInfo = new CultureInfo(CultureName),
        //        Scope = Scope,
        //        SelectedFacets =
        //        {
        //            new Composer.Parameters.SearchFilter{Name = facetName1}, 
        //            new Composer.Parameters.SearchFilter{Name = facetName2}
        //        }
        //    });

        //    // Assert
        //    model.SelectedFacets.IsAllRemovable.Should().BeTrue();
        //}

        //[Test]
        //public async Task WHEN_two_selected_facets_including_system_Facet_SHOULD_facets_are_not_all_removable()
        //{
        //    // Arrange
        //    SearchViewService service = _container.CreateInstance<SearchViewService>();

        //    // Act
        //    SearchViewModel model = await service.GetSearchViewModelAsync(new SearchCriteria
        //    {
        //        Keywords = "any",
        //        CultureInfo = new CultureInfo(CultureName),
        //        Scope = Scope,
        //        SelectedFacets =
        //        {
        //            new Composer.Parameters.SearchFilter(),
        //            new Composer.Parameters.SearchFilter { IsSystem = true }
        //        }
        //    });

        //    // Assert
        //    model.SelectedFacets.IsAllRemovable.Should().BeFalse();
        //}

        private void SetupFacets(params FacetSetting[] settings)
        {
            _container.GetMock<IFacetConfigurationContext>()
                .Setup(x => x.GetFacetSettings())
                .Returns(new List<FacetSetting>(settings));
        }
    }
}

using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Providers;
using Orckestra.Composer.SearchQuery.Repositories;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Search;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Tests.Service
{
    [TestFixture]
    public class SearchQueryViewService_GetSearchQueryViewModelAsync
    {
        private const string CultureName = "en-CA";
        private const string Scope = "global";
        private const string QueryName = "QueryName";
        private const string QuerySelectedFacetName = "CategoryLevel1";
        private const string QuerySelectedFacetValue= "Women";
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            SetUpSearchUrlProvider();
            SetUpSearchQueryUrlProvider();
            SetUpComposerContext();
            SetUpDamProvider();
            SetUpLocalizationProvider();
            SetUpProductUrlProvider();
            SetUpSearchQueryRepository();
            SetUpViewModelMapper();
            SetUpInventoryRepository();
            SetUpProductSettingsRepository();
            SetUpProductSearchViewModelFactory();
            SetUpProductSettingsViewService();
            SetUpPriceProvider();
            SetUpCategoryRepository();
            SetUpSelectedFacetFactory();
            SetFacetFactory();
        }


        private void SetUpSearchQueryRepository()
        {
            var result = new SearchResult()
            {
                Documents = new List<Document>(),
                Facets = new List<Facet>()
                {
                    new Facet()  {
                        FacetType = FacetType.Field,
                        FieldName = QuerySelectedFacetName,
                        Count = 1,
                        Values = new List<FacetValue>()
                        {
                            new FacetValue()
                            {
                                Value = QuerySelectedFacetValue,
                                DisplayName = QuerySelectedFacetValue,
                                Count = 1,
                            },
                            new FacetValue()
                            {
                                Value = GetRandom.String(5),
                                DisplayName = GetRandom.String(5),
                                Count = 5
                            }
                        }
                    }
                }
            };

            var results = new SearchQueryResult
            {
                SelectedFacets = new List<Overture.ServiceModel.SearchQueries.SelectedFacet>() {
                new SelectedFacet()
                {
                    FacetName = QuerySelectedFacetName,
                    Values = new List<string>() {QuerySelectedFacetValue}
                }
                },
                Result = result
            };

            var mock = new Mock<ISearchQueryRepository>();
            var mockSetup = mock.Setup(repository => repository.SearchQueryProductAsync(It.IsAny<SearchQueryProductParams>()));
            mockSetup.Returns(Task.FromResult(results));
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
            mock.Setup(_ => _.GetProductMainImagesAsync(It.IsAny<GetProductMainImagesParam>())).Returns(Task.FromResult(new List<ProductMainImage>()));
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

        private void SetUpInventoryRepository()
        {
            var mock = new Mock<Orckestra.Composer.SearchQuery.Repositories.IInventoryRepository>();
            mock.Setup(_ => _.GetInventoryLocationStatusesBySkus(It.IsAny<GetInventoryLocationStatuseParam>())).Returns(Task.FromResult(new List<Overture.ServiceModel.Products.Inventory.InventoryItemAvailability>()));
            _container.Use(mock);
        }

        private void SetUpCategoryRepository()
        {
            var mock = new Mock<ICategoryRepository>();
            var categories = new List<Category>();
            var tree = new Tree<Category, string>(categories, category => category.Id, category => category.PrimaryParentCategoryId, StringComparer.InvariantCultureIgnoreCase);
            mock.Setup(_ => _.GetCategoriesTreeAsync(It.IsAny<GetCategoriesParam>())).Returns(Task.FromResult(tree));
            _container.Use(mock);
        }

        private void SetUpProductSettingsRepository()
        {
            var mock = new Mock<IProductSettingsRepository>();
            mock.Setup(_ => _.GetProductSettings(It.IsAny<string>())).Returns(Task.FromResult(new Overture.ServiceModel.Products.ProductSettings()));
            _container.Use(mock);
        }

        private void SetUpProductSearchViewModelFactory()
        {
            var mock = new Mock<IProductSearchViewModelFactory>();

            mock.Setup(_ => _.EnrichAppendProductSearchViewModels(
                It.IsAny<IList<(ProductSearchViewModel, ProductDocument)>>(), It.IsAny<SearchCriteria>()))
                .Returns((IList<(ProductSearchViewModel, ProductDocument)> param1, SearchCriteria param2) =>
                {
                    IList<ProductSearchViewModel> results = param1.Select((resultItem) => resultItem.Item1).ToList();
                    return Task.FromResult(results);
                });
                
               
            _container.Use(mock);
        }

        private void SetUpProductSettingsViewService()
        {
            var mock = new Mock<IProductSettingsViewService>();
            _container.Use(mock);
        }

        private void SetUpPriceProvider()
        {
            var mock = new Mock<IPriceProvider>();
            _container.Use(mock);
        }

        private void SetUpSearchQueryUrlProvider()
        {
            var mock = new Mock<ISearchQueryUrlProvider>();
            _container.Use(mock);
        }

        private void SetUpSearchUrlProvider()
        {
            var mock = new Mock<ISearchUrlProvider>();

            _container.Use(mock);
        }

        private void SetUpSelectedFacetFactory()
        {
            var mock = new Mock<ISelectedFacetFactory>();
            mock.Setup(factory => factory.CreateSelectedFacet(It.IsAny<SearchFilter>(), It.IsAny<CultureInfo>()))
                 .Returns((SearchFilter searchFilter, CultureInfo cultureInfo) => new List<Orckestra.Composer.Search.Facets.SelectedFacet> { new Orckestra.Composer.Search.Facets.SelectedFacet { FieldName = searchFilter.Name } });
            _container.Use(mock);
        }

        private void SetFacetFactory()
        {
            var mock = new Mock<IFacetFactory>();
            mock.Setup(_ => _.CreateFacet(It.IsAny<Facet>(), It.IsAny<IReadOnlyList<SearchFilter>>(), It.IsAny<CultureInfo>()))
                .Returns((Facet facetResult, IReadOnlyList<SearchFilter> searchFilters, CultureInfo cultureInfo) =>
                {
                    var facet = new Facets.Facet
                    {
                        Title = facetResult.Title,
                        FieldName = facetResult.FieldName,
                        Quantity = 1,
                        SortWeight = 1,
                        IsDisplayed = true
                    };

                    var facetValues = facetResult.Values.Select(resultFacetValue => new Facets.FacetValue
                    {
                        Title = "title",
                        Value = resultFacetValue.Value,
                        Quantity = resultFacetValue.Count,
                        IsSelected = false,
                        MinimumValue = resultFacetValue.MinimumValue,
                        MaximumValue = resultFacetValue.MaximumValue
                    }).ToList();

                    facet.FacetValues = facetValues;
                    return facet;
                });
            _container.Use(mock);
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_argument_null_exception()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.GetSearchQueryViewModelAsync(null));
        }

        [Test]
        public void WHEN_Criteria_is_null_throw_argument_null_exception()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();

            // Act 
            var param = new GetSearchQueryViewModelParams
            {
                CultureInfo = new CultureInfo(CultureName),
                Criteria = null
            };

            //Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetSearchQueryViewModelAsync(param));

        }

        [Test]
        public async Task WHEN_Query_has_facet_SHOULD_returned_model_contain_selected_facet()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();

            // Act
            var param = new GetSearchQueryViewModelParams
            {
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                QueryName = QueryName,
                QueryType = SearchQueryType.Merchandising,
                Criteria = new SearchCriteria
                {
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = Scope,
                    SelectedFacets = { }
                }
            };
            var model = await service.GetSearchQueryViewModelAsync(param);

            // Assert
            model.FacetSettings.SelectedFacets.Facets.Count.Should().Be(1);
            model.FacetSettings.SelectedFacets.Facets.FirstOrDefault().FieldName.Should().Equals(QuerySelectedFacetName);
        }

        [Test]
        public async Task WHEN_Query_has_facet_SHOULD_returned_selected_notremovable_facet()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();

            // Act
            var param = new GetSearchQueryViewModelParams
            {
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                QueryName = QueryName,
                QueryType = SearchQueryType.Merchandising,
                Criteria = new SearchCriteria
                {
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = Scope,
                    SelectedFacets = { }
                }
            };
            var model = await service.GetSearchQueryViewModelAsync(param);

            // Assert
            model.FacetSettings.SelectedFacets.Facets.FirstOrDefault().IsRemovable.Should().BeFalse();
        }

        [Test]
        public async Task WHEN_Query_has_facet_and_one_selected_facet_by_user_SHOULD_selected_facets_count_should_be_2()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();
            var facetName = GetRandom.String(5);

            var param = new GetSearchQueryViewModelParams
            {
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                QueryName = QueryName,
                QueryType = SearchQueryType.Merchandising,
                Criteria = new SearchCriteria
                {
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = Scope,
                    SelectedFacets = {
                        new SearchFilter{Name = facetName}
                    }
                }
            };

            // Act
            var model = await service.GetSearchQueryViewModelAsync(param);

            // Assert
            model.FacetSettings.SelectedFacets.Facets.Count.Should().Be(2);
        }

        [Test]
        public async Task WHEN_Query_has_facet_SHOULD_facet_value_should_be_not_removable()
        {
            // Arrange
            SearchQueryViewService service = _container.CreateInstance<SearchQueryViewService>();
            var facetName = GetRandom.String(5);

            var param = new GetSearchQueryViewModelParams
            {
                CultureInfo = new CultureInfo(CultureName),
                Scope = Scope,
                QueryName = QueryName,
                QueryType = SearchQueryType.Merchandising,
                Criteria = new SearchCriteria
                {
                    CultureInfo = new CultureInfo(CultureName),
                    Scope = Scope,
                    SelectedFacets = {
                        new SearchFilter{Name = facetName}
                    }
                }
            };

            // Act
            var model = await service.GetSearchQueryViewModelAsync(param);

            // Assert
            model.ProductSearchResults.Facets.FirstOrDefault(f => f.FieldName == QuerySelectedFacetName)
                .FacetValues.FirstOrDefault(v => v.Value == QuerySelectedFacetValue)
                .IsRemovable.Should().BeFalse();
        }
    }
}

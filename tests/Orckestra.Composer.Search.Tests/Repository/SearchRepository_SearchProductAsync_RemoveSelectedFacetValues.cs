using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using FacetType = Orckestra.Composer.Search.Facets.FacetType;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Tests.Repository
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SearchRepository_SearchProductAsync_RemoveSelectedFacetValues
    {
        private IProductRequestFactory ProductRequestFactory { get; set; }
        private IFacetPredicateFactory FacetPredicateFactory { get; set; }
        private ISearchRepository _sut;
        private Mock<IOvertureClient> OvertureClientMock { get; set; }
        private Mock<IFacetConfigurationContext> FacetConfigurationContext { get; set; }

        private const string SomeSingleFacetFieldName = "SomeSingleFacet";
        private const string SingleFacetValue1 = "SingleFacetValue1";
        private const string SingleFacetValue2 = "SingleFacetValue2";

        private const string SomeMultiFacetFieldName = "SomeMultiFacet";
        private const string MultiFacetValue1 = "MultiFacetValue1";
        private const string MultiFacetValue2 = "MultiFacetValue2";


        [SetUp]
        public void SetUp()
        {
            // Arrange
            ProductRequestFactory = new AutoMocker().CreateInstance<ProductRequestFactory>(); ;
            FacetPredicateFactory = MockFacetPredicateFactory().Object;
            FacetConfigurationContext = new Mock<IFacetConfigurationContext>();

            OvertureClientMock = MockOvertureClient();
            _sut = new SearchRepository(OvertureClientMock.Object, ProductRequestFactory, FacetPredicateFactory, FacetConfigurationContext.Object);

            FacetConfigurationContext
                .Setup(x => x.GetFacetSettings())
                .Returns(new List<FacetSetting>
                {
                    new FacetSetting(SomeSingleFacetFieldName)
                    {
                        FacetType              = Facets.FacetType.SingleSelect,
                        SortWeight             = -1.0,
                        MaxCollapsedValueCount = 5
                    },
                    new FacetSetting(SomeMultiFacetFieldName)
                    {
                        FacetType              = FacetType.MultiSelect,
                        SortWeight             = 0.0,
                        MaxCollapsedValueCount = 5,
                        MaxExpendedValueCount  = 20
                    },
                });
        }

        [Test]
        public async Task WHEN_overture_returns_facets_in_search_results_SHOULD_remove_selected_facet_for_non_multiselect_facet_types_from_result()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = "test search keywords",
                Scope = "Global",
                IncludeFacets = true
            };

            param.SelectedFacets.Add(new SearchFilter()
            {
                Name = SomeSingleFacetFieldName,
                Value = SingleFacetValue1
            });

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            var facetToVerify = result.Facets.Find(facet => facet.FieldName == SomeSingleFacetFieldName);

            facetToVerify.Should().BeNull();
        }

        [Test]
        public async Task WHEN_overture_returns_facets_in_search_results_SHOULD_keep_selected_facets_for_multiselect_facet_types_from_result()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = "test search keywords",
                Scope = "Global",
                IncludeFacets = true
            };

            param.SelectedFacets.Add(new SearchFilter()
            {
                Name = SomeMultiFacetFieldName,
                Value = MultiFacetValue1
            });

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            var facetToVerify = result.Facets.Find(facet => facet.FieldName == SomeMultiFacetFieldName);

            facetToVerify.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_overture_returns_facets_in_search_results_SHOULD_keep_all_facet_values_if_there_are_no_selected_facets()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = "test search keywords",
                Scope = "Global",
                IncludeFacets = true
            };

            param.SelectedFacets.Add(new SearchFilter()
            {
                Name = SomeMultiFacetFieldName,
                Value = MultiFacetValue1
            });

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            var singleFacetToVerify = result.Facets.Find(facet => facet.FieldName == SomeSingleFacetFieldName);

            singleFacetToVerify.Should().NotBeNull();


            var multiFacetToVerify = result.Facets.Find(facet => facet.FieldName == SomeMultiFacetFieldName);

            multiFacetToVerify.Should().NotBeNull();
        }


        private Mock<IFacetPredicateFactory> MockFacetPredicateFactory()
        {
            var mockPaymentProviderFactory = new Mock<IFacetPredicateFactory>();

            mockPaymentProviderFactory.Setup(factory => factory.CreateFacetPredicate(It.IsNotNull<SearchFilter>()))
                .Returns(new FacetPredicate());

            return mockPaymentProviderFactory;
        }

        private static Mock<IOvertureClient> MockOvertureClient()
        {
            var overtureClient = new Mock<IOvertureClient>(MockBehavior.Strict);

            overtureClient
                .Setup(c => c.SendAsync(It.IsNotNull<SearchAvailableProductsRequest>()))
                .ReturnsAsync(new ProductSearchResult
                {
                    Facets = new List<Facet>
                    {
                        new Facet
                        {
                            FieldName = SomeSingleFacetFieldName,
                            Values = new List<FacetValue>
                            {
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = SingleFacetValue1
                                },
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = SingleFacetValue2
                                }
                            },
                            Count = GetRandom.PositiveInt()+2
                        },
                        new Facet
                        {
                            FieldName = SomeMultiFacetFieldName,
                            Values = new List<FacetValue>
                            {
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = "MultiFacetValue1"
                                },
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = "MultiFacetValue2"
                                }
                            },
                            Count = GetRandom.PositiveInt() + 2
                        }
                    },
                    TotalCount = GetRandom.PositiveInt()
                });

            return overtureClient;
        }
    }
}

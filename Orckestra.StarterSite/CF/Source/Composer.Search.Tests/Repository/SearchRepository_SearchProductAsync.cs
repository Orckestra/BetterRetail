using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Tests.Repository
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SearchRepository_SearchProductAsync
    {
        private IProductRequestFactory ProductRequestFactory { get; set; }
        private IFacetPredicateFactory FacetPredicateFactory { get; set; }
        private ISearchRepository _sut;
        private Mock<IOvertureClient> OvertureClientMock { get; set; }

        private IList<FacetSetting> _oldFacetGroupSettingsList;


        [SetUp]
        public void SetUp()
        {
            // Arrange
            ProductRequestFactory = new AutoMocker().CreateInstance<ProductRequestFactory>();
            FacetPredicateFactory = MockFacetPredicateFactory().Object;

            OvertureClientMock = MockOvertureClient();
            _sut = new SearchRepository(OvertureClientMock.Object, ProductRequestFactory, FacetPredicateFactory);

            // Keep a record of the original facet group settings list.
            _oldFacetGroupSettingsList = SearchConfiguration.FacetSettings;
            SearchConfiguration.FacetSettings = new[]
            {
                new FacetSetting("ExpectedName")
                {
                    SortWeight = 99.1,
                    MaxCollapsedValueCount = 5,
                    MaxExpendedValueCount = 90
                },
                new FacetSetting("WithDependencyName")
                {
                    SortWeight = -293.93,
                    DependsOn = new[]
                    {
                        "ExpectedName"
                    }
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original facet group settings.
            SearchConfiguration.FacetSettings = _oldFacetGroupSettingsList;
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SearchProductAsync(null));
        }

        [Test]
        public void WHEN_culture_is_null_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = null,
                Keywords = Guid.NewGuid().ToString(),
                Scope = Guid.NewGuid().ToString(),
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SearchProductAsync(param));
        }


        [TestCase("           ")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("\r\n\t")]
        public void WHEN_keyword_is_empty_SHOULD_throw_ArgumentException(string keyword)
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = null,
                Keywords = keyword,
                Scope = Guid.NewGuid().ToString(),
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SearchProductAsync(param));
        }

        [TestCase("           ")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("\r\n\t")]
        public void WHEN_scope_is_empty_SHOULD_throw_ArgumentException(string scope)
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = null,
                Keywords = Guid.NewGuid().ToString(),
                Scope = scope,
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SearchProductAsync(param));
        }

        [Test]
        public async Task WHEN_culture_and_keywords_provided_SHOULD_issue_request_properly_configured()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = GetRandom.Phrase(10),
                Scope = GetRandom.String(6),
            };

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            OvertureClientMock.Verify();
        }

        [Test]
        public async Task WHEN_overture_returns_unexpected_facet_SHOULD_remove_facet_from_result()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = GetRandom.Phrase(10),
                Scope = GetRandom.String(6),
                IncludeFacets = true
            };

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            result.Facets.Should().NotContain(facet => facet.FieldName == "UnexpectedName");
        }

        [Test]
        public async Task WHEN_request_SHOULD_have_dependency_in_facets()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = GetRandom.Phrase(10),
                Scope = GetRandom.String(6),
                SelectedFacets = { new SearchFilter
                {
                    Name = "ExpectedName",
                    Value = GetRandom.String(32),
                    IsSystem = GetRandom.Boolean()
                }},
                IncludeFacets = true
            };

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            result.Facets.Should().Contain(facet => facet.FieldName == "WithDependencyName");
        }

        [Test]
        public async Task WHEN_request_has_singlevalue_selectedfacet_SHOULD_remove_facet_from_result()
        {
            // Arrange
            var param = new SearchCriteria()
            {
                CultureInfo = new CultureInfo("en-US"),
                Keywords = "test search keywords",
                Scope = "Global",
                SelectedFacets = { new SearchFilter
                {
                    Name = "ExpectedName",
                    Value = GetRandom.String(32),
                    IsSystem = GetRandom.Boolean()
                }},
                IncludeFacets = true
            };

            // Act
            var result = await _sut.SearchProductAsync(param);

            // Assert
            result.Facets.Should().NotContain(facet => facet.FieldName == "ExpectedName");
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
                            FieldName = "ExpectedName",
                            Values = new List<FacetValue>
                            {
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                },
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                }
                            },
                            Count = GetRandom.PositiveInt()+2
                        },
                        new Facet
                        {
                            FieldName = "UnexpectedName",
                            Values = new List<FacetValue>
                            {
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                },
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                }
                            },
                            Count = GetRandom.PositiveInt()+2
                        },
                        new Facet
                        {
                            FieldName = "WithDependencyName",
                            Values = new List<FacetValue>
                            {
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                },
                                new FacetValue
                                {
                                    Count = GetRandom.PositiveInt(),
                                    Value = GetRandom.String(32)
                                }
                            },
                            Count = GetRandom.PositiveInt()+2
                        },


                    },
                    TotalCount = GetRandom.PositiveInt()
                });

            return overtureClient;
        }
    }
}

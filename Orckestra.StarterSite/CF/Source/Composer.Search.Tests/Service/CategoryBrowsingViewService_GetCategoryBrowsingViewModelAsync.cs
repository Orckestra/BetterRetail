using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Search;
using ServiceStack.Text;
using Facet = Orckestra.Overture.ServiceModel.Search.Facet;
using FacetType = Orckestra.Composer.Search.Facets.FacetType;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Tests.Service
{
    [TestFixture]
    public class CategoryBrowsingViewService_GetCategoryBrowsingViewModelAsync
    {
        private const string CultureName = "en-CA";
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            SetUpFacetConfiguration();
            SetUpCategoryBrowsingUrlProvider();
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

        private void SetUpFacetConfiguration()
        {
            SetupFacets(
                new FacetSetting("CategoryLevel1_Facet")
                {
                    FacetType = FacetType.SingleSelect,
                    SortWeight = -1.0,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("CategoryLevel2_Facet")
                {
                    FacetType = FacetType.SingleSelect,
                    SortWeight = -1.0,
                    MaxCollapsedValueCount = 5,
                    DependsOn = new[]
                    {
                        "CategoryLevel1_Facet"
                    }
                },
                new FacetSetting("CategoryLevel3_Facet")
                {
                    FacetType = FacetType.SingleSelect,
                    SortWeight = -1.0,
                    MaxCollapsedValueCount = 5,
                    DependsOn = new[]
                    {
                        "CategoryLevel2_Facet"
                    }
                });
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

        private void SetupCategoryUrlProvider()
        {
            _container.GetMock<ICategoryBrowsingUrlProvider>()
                .Setup(m => m.BuildCategoryBrowsingUrl(It.IsNotNull<BuildCategoryBrowsingUrlParam>()))
                .Returns((BuildCategoryBrowsingUrlParam p) => GetRandom.WwwUrl() + "/" + p.CategoryId);

            _container.GetMock<ICategoryBrowsingUrlProvider>()
               .Setup(m => m.BuildSearchQueryString(It.IsNotNull<BuildSearchUrlParam>()))
               .Returns(new NameValueCollection());
        }

        private void SetUpComposerContext()
        {
            var mock = new Mock<IComposerContext>();
            mock.Setup(context => context.CultureInfo).Returns(new CultureInfo(CultureName));
            mock.Setup(context => context.Scope).Returns("global");

            _container.Use(mock);
        }

        private void SetUpCategoryBrowsingUrlProvider()
        {
            var mock = new Mock<ICategoryBrowsingUrlProvider>();

            _container.Use(mock);
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_argument_null_exception()
        {
            // Arrange
            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.GetCategoryBrowsingViewModelAsync(null));
        }

        [Test]
        public void WHEN_category_id_is_null_SHOULD_throw_argument_exception()
        {
            // Arrange
            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetCategoryBrowsingViewModelAsync(
                new GetCategoryBrowsingViewModelParam
                {
                    CategoryId = null
                }
            ));
        }

        [Test]
        public void WHEN_selected_facets_is_null_SHOULD_throw_argument_exception()
        {
            // Arrange
            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetCategoryBrowsingViewModelAsync(
                new GetCategoryBrowsingViewModelParam
                {
                    CategoryId = GetRandom.String(1),
                    SelectedFacets = null
                }
            ));
        }

        [Test]
        public void WHEN_category_id_not_found_SHOULD_throw_invalid_operation_exception()
        {
            // Arrange
            SetupCategoryRepository();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCategoryBrowsingViewModelAsync(
                new GetCategoryBrowsingViewModelParam
                {
                    CategoryId = GetRandom.String(1),
                    SelectedFacets = new List<SearchFilter>()
                }
            ));
        }

        [Test]
        public async Task WHEN_category_id_found_SHOULD_return_view_model()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.Should().NotBeNull();
            model.CategoryId.Should().Be(categories[0].Id);
        }

        [Test]
        public async Task WHEN_category_id_found_SHOULD_have_landing_page_url()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.LandingPageUrls.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_category_has_no_child_SHOULD_view_model_contain_no_child_category()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.ChildCategories.Count.Should().Be(0);
        }

        [Test]
        public async Task WHEN_category_has_one_child_SHOULD_view_model_contain_one_child_category()
        {
            // Arrange
            var parentId = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = parentId,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = parentId
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = parentId,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.ChildCategories.Count.Should().Be(1);
        }

        [Test]
        public async Task WHEN_category_has_many_children_SHOULD_view_model_contain_many_child_categories()
        {
            // Arrange
            var parentId = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = parentId,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = parentId
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_1" }}),
                    PrimaryParentCategoryId = parentId
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_2" }}),
                    PrimaryParentCategoryId = parentId
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = parentId,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.ChildCategories.Count.Should().Be(3);
        }

        [Test]
        public async Task WHEN_category_has_children_SHOULD_view_model_contain_child_categories_with_correct_title()
        {
            // Arrange
            var parentId = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = parentId,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = parentId
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_1" }}),
                    PrimaryParentCategoryId = parentId
                },
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_2" }}),
                    PrimaryParentCategoryId = parentId
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = parentId,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.ChildCategories[0].Title.Should().Be(categories[1].DisplayName.GetLocalizedValue(CultureName));
            model.ChildCategories[1].Title.Should().Be(categories[2].DisplayName.GetLocalizedValue(CultureName));
            model.ChildCategories[2].Title.Should().Be(categories[3].DisplayName.GetLocalizedValue(CultureName));
        }

        [Test]
        public async Task WHEN_category_is_found_SHOULD_facet_type_is_single_select()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(1),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.First().FacetType.Should().Be(FacetType.SingleSelect);
        }

        [Test]
        public async Task WHEN_category_with_no_ancestor_is_found_SHOULD_selected_facets_contain_1_facet()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(10),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.Count.Should().Be(1);
        }

        [Test]
        public async Task WHEN_category_with_many_ancestors_is_found_SHOULD_selected_facets_contain_self_and_all_ancestors()
        {
            // Arrange
            var level1Id = GetRandom.String(10);
            var level2Id = GetRandom.String(10);
            var level3Id = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = level1Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = level2Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = level1Id
                },
                new Category
                {
                    Id = level3Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level3_0" }}),
                    PrimaryParentCategoryId = level2Id
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = level3Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.Count.Should().Be(3);
        }

        [Test]
        public async Task WHEN_category_is_found_SHOULD_facet_is_not_removable()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(1),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.First().IsRemovable.Should().BeFalse();
            model.SelectedFacets.IsAllRemovable.Should().BeFalse();
        }

        [Test]
        public async Task WHEN_category_with_ancestors_is_found_SHOULD_facets_are_not_all_removable()
        {
            // Arrange
            var level1Id = GetRandom.String(10);
            var level2Id = GetRandom.String(10);
            var level3Id = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = level1Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = level2Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = level1Id
                },
                new Category
                {
                    Id = level3Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level3_0" }}),
                    PrimaryParentCategoryId = level2Id
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = level3Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets[0].IsRemovable.Should().BeFalse();
            model.SelectedFacets.Facets[1].IsRemovable.Should().BeFalse();
            model.SelectedFacets.Facets[2].IsRemovable.Should().BeFalse();

            model.SelectedFacets.IsAllRemovable.Should().BeFalse();
        }

        [Test]
        public async Task WHEN_category_is_found_SHOULD_selected_facets_contain_self_and_all_ancestors_whose_field_name_starts_with_CategoryLevel()
        {
            // Arrange
            var level1Id = GetRandom.String(10);
            var level2Id = GetRandom.String(10);
            var level3Id = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = level1Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = level2Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = level1Id
                },
                new Category
                {
                    Id = level3Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level3_0" }}),
                    PrimaryParentCategoryId = level2Id
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = level3Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.Should().OnlyContain(facet => facet.FieldName.StartsWith("CategoryLevel"));
        }

        [Test]
        public async Task WHEN_category_is_found_SHOULD_selected_facets_contain_self_and_all_ancestors_whose_field_name_ends_with_Facet()
        {
            // Arrange
            var level1Id = GetRandom.String(10);
            var level2Id = GetRandom.String(10);
            var level3Id = GetRandom.String(10);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = level1Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = level2Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = level1Id
                },
                new Category
                {
                    Id = level3Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level3_0" }}),
                    PrimaryParentCategoryId = level2Id
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = level3Id,
                SelectedFacets = new List<SearchFilter>()
            });

            // Assert
            model.SelectedFacets.Facets.Should().OnlyContain(facet => facet.FieldName.EndsWith("_Facet"));
        }

        [Test]
        public async Task WHEN_param_contains_system_facet_SHOULD_facet_is_not_removable()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = GetRandom.String(1),
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            var facetName = GetRandom.String(5);
            SetupFacets(new FacetSetting(facetName));

            // Act
            var expectedFacet = new SearchFilter
            {
                Name = facetName,
                IsSystem = true
            };

            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categories[0].Id,
                SelectedFacets = new List<SearchFilter> { expectedFacet }
            });

            // Assert
            model.SelectedFacets.Facets.First(facet => facet.FieldName == expectedFacet.Name).IsRemovable.Should().BeFalse();
        }

        [Test]
        public async Task WHEN_selected_category_is_children_SHOULD_return_filled_landingPageUrls()
        {
            // Arrange
            var level1Id = "level1";
            var level2Id = "level2";
            var level3Id = "level3";

            var categories = new List<Category>
            {
                new Category
                {
                    Id = level1Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
                },
                new Category
                {
                    Id = level2Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level2_0" }}),
                    PrimaryParentCategoryId = level1Id
                },
                new Category
                {
                    Id = level3Id,
                    DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level3_0" }}),
                    PrimaryParentCategoryId = level2Id
                }
            };

            SetupCategoryRepository(categories);
            SetupCategoryUrlProvider();

            CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();

            // Act
            CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = level3Id,
                SelectedFacets = new List<SearchFilter>()
            });

            //Assert
            model.Should().NotBeNull();
            model.LandingPageUrls.Should().NotBeNullOrEmpty();
            model.LandingPageUrls.Should().HaveCount(2);
            model.LandingPageUrls[0].Should().EndWith(level1Id);
            model.LandingPageUrls[1].Should().EndWith(level2Id);

        }
        //[Test]
        //public async Task WHEN_param_contains_not_system_facet_SHOULD_facet_is_removable()
        //{
        //    // Arrange
        //    var categories = new List<Category>
        //    {
        //        new Category
        //        {
        //            Id = GetRandom.String(1),
        //            DisplayName = new LocalizedString(new Dictionary<string, string> { { CultureName, "level1_0" }})
        //        }
        //    };

        //    SetupCategoryRepository(categories);

        //    CategoryBrowsingViewService service = _container.CreateInstance<CategoryBrowsingViewService>();
        //    string facetName = GetRandom.String(5);

        //    SearchConfiguration.FacetSettings.Add(new FacetSetting(facetName));

        //    // Act
        //    var expectedFacet = new SearchFilter
        //    {
        //        Name = facetName,
        //        IsSystem = false
        //    };

        //    CategoryBrowsingViewModel model = await service.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
        //    {
        //        CategoryId = categories[0].Id,
        //        SelectedFacets = new List<SearchFilter> { expectedFacet }
        //    });

        //    // Assert
        //    model.SelectedFacets.Facets.First(facet => facet.FieldName == expectedFacet.Name).IsRemovable.Should().BeTrue();
        //}

        private void SetupCategoryRepository()
        {
            SetupCategoryRepository(new List<Category>()); // empty category tree
        }

        private void SetupCategoryRepository(List<Category> categories)
        {
            var categoriesTree = new Tree<Category, string>(source: categories,
                                                            keySelector: category => category.Id,
                                                            parentKeySelector: category => category.PrimaryParentCategoryId,
                                                            comparer: StringComparer.InvariantCultureIgnoreCase);

            var mock = new Mock<ICategoryRepository>();
            mock.Setup(repository => repository.GetCategoriesTreeAsync(It.IsAny<GetCategoriesParam>())).ReturnsAsync(categoriesTree);

            _container.Use(mock);
        }

        private void SetupFacets(params FacetSetting[] settings)
        {
            _container.GetMock<IFacetConfigurationContext>()
                .Setup(x => x.GetFacetSettings())
                .Returns(new List<FacetSetting>(settings));
        }

    }
}

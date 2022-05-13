using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Search;
using Facet = Orckestra.Composer.Search.Facets.Facet;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;
using Suggestion = Orckestra.Composer.Search.ViewModels.Suggestion;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Search.Services
{
    public abstract class BaseSearchViewService<TParam> where TParam : class, ISearchParam
    {
        private const string VariantPropertyBagKey = "VariantId";

        public virtual SearchType SearchType => SearchType.Searching;

        protected IDamProvider DamProvider { get; }
        protected IFacetFactory FacetFactory { get; }
        protected ILocalizationProvider LocalizationProvider { get; }
        protected ISearchRepository SearchRepository { get; }
        protected ISearchUrlProvider SearchUrlProvider { get; }
        protected ISelectedFacetFactory SelectedFacetFactory { get; }
        protected IComposerContext ComposerContext { get; }
        protected IProductSearchViewModelFactory ProductSearchViewModelFactory { get; private set; }
        protected ICategoryRepository CategoryRepository { get; set; }

        protected BaseSearchViewService(ISearchRepository searchRepository,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            ISearchUrlProvider searchUrlProvider,
            IFacetFactory facetFactory,
            ISelectedFacetFactory selectedFacetFactory,
            IComposerContext composerContext,
            IProductSearchViewModelFactory productSearchViewModelFactory,
            ICategoryRepository categoryRepository)
        {
            SearchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            SelectedFacetFactory = selectedFacetFactory ?? throw new ArgumentNullException(nameof(selectedFacetFactory));
            FacetFactory = facetFactory ?? throw new ArgumentNullException(nameof(facetFactory));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ProductSearchViewModelFactory = productSearchViewModelFactory ?? throw new ArgumentNullException(nameof(productSearchViewModelFactory));
            CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        protected virtual IList<Facet> BuildFacets(SearchCriteria criteria, ProductSearchResult searchResult)
        {
            if (searchResult.Facets == null) { return new List<Facet>(); }

            var cultureInfo = criteria.CultureInfo;
            var selectedFacets = criteria.SelectedFacets;

            var facetList =
                searchResult.Facets
                    .Select(facetResult => FacetFactory.CreateFacet(facetResult, selectedFacets, cultureInfo))
                    .Where(facet => facet != null && facet.Quantity > 0)
                    .OrderBy(facet => facet.SortWeight)
                    .ThenBy(facet => facet.FieldName)
                    .ToList();

            return facetList;
        }

        protected virtual IList<PromotedFacetValue> BuildPromotedFacetValues(IEnumerable<Facet> facets)
        {
            return facets
                .SelectMany(facet => facet.FacetValues
                    .Where(value => value.IsPromoted)
                    .Select(value => new
                    {
                        PromotionWeight = value.PromotionSortWeight,
                        FacetValue = new PromotedFacetValue(facet.FieldName, facet.FacetType, value.Value)
                        {
                            Title = value.Title,
                            Quantity = value.Quantity,
                            IsSelected = value.IsSelected
                        }
                    })
                )
                .OrderBy(facetValue => facetValue.PromotionWeight)
                .Select(facetValue => facetValue.FacetValue)
                .ToList();
        }

        private SearchPaginationViewModel BuildPaginationForSearchResults(
            ProductSearchResult searchResult,
            TParam searchParam, int maxPages)
        {
            var totalCount = searchResult.TotalCount;
            var itemsPerPage = SearchConfiguration.MaxItemsPerPage;
            var totalPages = (int)Math.Ceiling((double)totalCount / itemsPerPage);

            var param = new CreateSearchPaginationParam<TParam>
            {
                SearchParameters = searchParam,
                CurrentPageIndex = searchParam.Criteria.Page,
                MaximumPages = SearchConfiguration.ShowAllPages ? totalPages : maxPages,
                TotalNumberOfPages = totalPages,
                CorrectedSearchTerms = searchResult.CorrectedSearchTerms
            };

            var pages = GetPages(param);
            var pager = new SearchPaginationViewModel
            {
                PreviousPage = GetPreviousPage(param),
                NextPage = GetNextPage(param),
                CurrentPage = pages.FirstOrDefault(p => p.IsCurrentPage),
                Pages = pages,
                TotalNumberOfPages = totalPages
            };

            return pager;
        }

        /// <summary>
        ///     Creates the product search results view model.
        /// </summary>
        /// <createSearchViewModelParam name="createSearchViewModelParam">The parameter.</createSearchViewModelParam>
        /// <returns></returns>
        protected virtual async Task<ProductSearchResultsViewModel> CreateProductSearchResultsViewModelAsync(CreateProductSearchResultsViewModelParam<TParam> param)
        {
            //TODO: Implement by calling the ViewModelMapper instead.
            var searchResultViewModel = new ProductSearchResultsViewModel
            {
                SearchResults = new List<ProductSearchViewModel>(),
                CategoryFacetCounts = BuildCategoryFacetCounts(param),
                Keywords = param.SearchParam.Criteria.Keywords,
                TotalCount = param.SearchResult.TotalCount,
                CorrectedSearchTerms = param.SearchResult.CorrectedSearchTerms,
                Suggestions = new List<Suggestion>()
            };

            if (param.SearchResult.Suggestions != null)
            {
                foreach (var suggestion in param.SearchResult.Suggestions)
                {
                    var cloneParam = param.SearchParam.Criteria.Clone();
                    cloneParam.Keywords = suggestion.Title;

                    searchResultViewModel.Suggestions.Add(new Suggestion
                    {
                        Title = suggestion.Title,
                        Url = SearchUrlProvider.BuildSearchUrl(new BuildSearchUrlParam
                        {
                            SearchCriteria = cloneParam
                        })
                    });
                }
            }

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ImageUrls);
            var searchResultsList = new List<(ProductSearchViewModel, ProductDocument)>();

            // Populate search results
            foreach (var resultItem in param.SearchResult.Documents)
            {
                searchResultsList.Add((ProductSearchViewModelFactory.GetProductSearchViewModel(resultItem, param.SearchParam.Criteria, imgDictionary), resultItem));
            }
            
            searchResultViewModel.SearchResults = await ProductSearchViewModelFactory.EnrichAppendProductSearchViewModels(searchResultsList, param.SearchParam.Criteria).ConfigureAwait(false);

            var facets = BuildFacets(param.SearchParam.Criteria, param.SearchResult);
            searchResultViewModel.Facets = facets;
            searchResultViewModel.Pagination = BuildPaginationForSearchResults(param.SearchResult, param.SearchParam, SearchConfiguration.MaximumPages);
            searchResultViewModel.PromotedFacetValues = BuildPromotedFacetValues(facets);

            // TODO: Fix this
            new SearchSortByResolver<TParam>(LocalizationProvider, GetSearchSortByList(SearchType), GenerateUrl)
                .Resolve(searchResultViewModel, param.SearchParam);

            searchResultViewModel.BaseUrl = param.SearchParam.Criteria.BaseUrl;

            return searchResultViewModel;
        }

        protected virtual CategoryFacetCounts BuildCategoryFacetCounts(CreateProductSearchResultsViewModelParam<TParam> param)
        {
            if (param.CategoryFacetCountsResult == null)
            {
                return new CategoryFacetCounts()
                {
                    TotalCount = param.SearchResult.TotalCount
                };
            }

            return new CategoryFacetCounts
            {
                TotalCount = param.CategoryFacetCountsResult.TotalCount,
                Facets = param.CategoryFacetCountsResult.Facets.Where(f => !f.FieldName.EndsWith("_Facet")).Select(f =>
                     new Facet
                     {
                         FieldName = f.FieldName,
                         FacetValues = f.Values.Select(fv => new Facets.FacetValue()
                         {
                             Value = fv.Value,
                             Quantity = fv.Count
                         }).ToList()
                     }).ToList()
            };
        }

        protected virtual async Task<CategoryFacetValuesTree> BuildCategoryFacetValuesTree(IList<Facet> facets,
            SelectedFacets selectedFacets,
            CategoryFacetCounts categoryCounts)
        {
            var categories = await CategoryRepository.GetCategoriesTreeAsync(new GetCategoriesParam
            {
                Scope = ComposerContext.Scope
            }).ConfigureAwait(false);

            return FacetFactory.BuildCategoryFacetValuesTree(facets, selectedFacets, categories, categoryCounts, ComposerContext.CultureInfo);
        }

        private IList<SearchSortBy> GetSearchSortByList(SearchType searchType)
        {
            return SearchConfiguration.SearchSortBy.Where(d => !d.SearchType.HasValue || d.SearchType.Value == searchType).ToList();
        }

        protected virtual SelectedFacets FlattenFilterList(IList<SearchFilter> filters, CultureInfo cultureInfo)
        {
            if (filters == null) { throw new ArgumentNullException(nameof(filters)); }

            var facets = new List<SelectedFacet>();

            foreach (var filter in filters)
            {
                var selectedFacets = SelectedFacetFactory.CreateSelectedFacet(filter, cultureInfo);
                facets.AddRange(selectedFacets);
            }

            return new SelectedFacets
            {
                Facets = facets,
                IsAllRemovable = facets.Count(filter => filter.IsRemovable) > 1
            };
        }

        protected abstract string GenerateUrl(CreateSearchPaginationParam<TParam> param);

        protected virtual SearchPageViewModel GetNextPage(CreateSearchPaginationParam<TParam> param)
        {
            var searchCriteria = param.SearchParameters.Criteria;
            var nextPage = new SearchPageViewModel
            {
                DisplayName = LocalizationProvider
                    .GetLocalizedString(new GetLocalizedParam
                    {
                        Category = "List-Search",
                        Key = "B_Next",
                        CultureInfo = searchCriteria.CultureInfo
                    })
            };

            if (param.CurrentPageIndex < param.TotalNumberOfPages)
            {
                searchCriteria.Page = param.CurrentPageIndex + 1;
                nextPage.Url = GenerateUrl(param);
            }
            else
            {
                nextPage.IsCurrentPage = true;
            }

            return nextPage;
        }

        protected virtual IEnumerable<SearchPageViewModel> GetPages(CreateSearchPaginationParam<TParam> param)
        {
            var pages = new List<SearchPageViewModel>();
            var endPage = 0;
            var startPage = 0;

            if (param.TotalNumberOfPages < param.MaximumPages)
            {
                startPage = 1;
                endPage = param.TotalNumberOfPages;
            }
            else if (param.MaximumPages <= param.TotalNumberOfPages)
            {
                var maxPagesSplit = (int)Math.Floor((double)param.MaximumPages / 2);
                var potentialStartPage = param.CurrentPageIndex - maxPagesSplit;

                if (potentialStartPage < 1)
                {
                    startPage = 1;
                    endPage = param.MaximumPages;
                }
                else
                {
                    //Can you explain what it does
                    var potentialEndPage = param.CurrentPageIndex + maxPagesSplit - (param.MaximumPages % 2 == 0 ? 1 : 0);

                    if (potentialEndPage > param.TotalNumberOfPages)
                    {
                        startPage = param.TotalNumberOfPages - param.MaximumPages + 1;
                        endPage = param.TotalNumberOfPages;
                    }
                    else
                    {
                        startPage = potentialStartPage;
                        endPage = potentialEndPage;
                    }
                }
            }
            else if (param.MaximumPages > param.TotalNumberOfPages)
            {
                startPage = 1;
                endPage = param.TotalNumberOfPages;
            }

            for (var index = startPage; index <= endPage; index++)
            {
                var displayName = index.ToString(CultureInfo.InvariantCulture);
                param.SearchParameters.Criteria.Page = index;
                var searchUrl = GenerateUrl(param);
                var searchPage = new SearchPageViewModel
                {
                    DisplayName = displayName,
                    Url = searchUrl,
                    IsCurrentPage = index == param.CurrentPageIndex,
                    UrlPath = searchUrl.Replace(param.SearchParameters.Criteria.BaseUrl, string.Empty)
                };

                pages.Add(searchPage);
            }

            return pages;
        }

        protected virtual SearchPageViewModel GetPreviousPage(CreateSearchPaginationParam<TParam> param)
        {
            var searchCriteria = param.SearchParameters.Criteria;
            var previousPage = new SearchPageViewModel
            {
                DisplayName = LocalizationProvider
                .GetLocalizedString(new GetLocalizedParam
                {
                    Category = "List-Search",
                    Key = "B_Previous",
                    CultureInfo = searchCriteria.CultureInfo
                })
            };

            if (param.CurrentPageIndex > 1)
            {
                searchCriteria.Page = param.CurrentPageIndex - 1;
                previousPage.Url = GenerateUrl(param);
            }
            else
            {
                previousPage.IsCurrentPage = true;
            }

            return previousPage;
        }
      

        /// <summary>
        ///     Searches the available products based on the given search criteria.
        /// </summary>
        /// <param name="param">The criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        protected virtual async Task<ProductSearchResultsViewModel> SearchAsync(TParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Criteria == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Criteria)), nameof(param)); }
            if (param.Criteria.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Criteria.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Criteria.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Criteria.Scope)), nameof(param)); }

            var searchResult = await SearchRepository.SearchProductAsync(param.Criteria).ConfigureAwait(false);

            var cloneParam = (TParam)param.Clone();

            if (searchResult == null) { return null; }

            var imageUrls = await DamProvider.GetProductMainImagesAsync(GetImagesParam(searchResult.Documents)).ConfigureAwait(false);

            var createSearchViewModelParam = new CreateProductSearchResultsViewModelParam<TParam>
            {
                SearchParam = cloneParam,
                ImageUrls = imageUrls,
                SearchResult = searchResult
            };

            if (param.Criteria.IncludeFacets &&
                param.Criteria.SelectedFacets != null && 
                param.Criteria.SelectedFacets.Any(s => s.Name?.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix) ?? false))
            {
                createSearchViewModelParam.CategoryFacetCountsResult = await SearchRepository.GetCategoryFacetCountsAsync(param.Criteria).ConfigureAwait(false);
            }

            return await CreateProductSearchResultsViewModelAsync(createSearchViewModelParam).ConfigureAwait(false);
        }
        private static GetProductMainImagesParam GetImagesParam(List<ProductDocument> documnets)
        {
            return new GetProductMainImagesParam
            {
                ImageSize = SearchConfiguration.DefaultImageSize,
                ProductImageRequests = documnets
                    .Select(document => new ProductImageRequest
                    {
                        ProductId = document.ProductId,
                        Variant = document.PropertyBag.ContainsKey(VariantPropertyBagKey)
                            ? new VariantKey { Id = document.PropertyBag[VariantPropertyBagKey].ToString() }
                            : VariantKey.Empty,
                        ProductDefinitionName = document.PropertyBag.ContainsKey("DefinitionName")
                            ? document.PropertyBag["DefinitionName"].ToString()
                            : string.Empty,
                        PropertyBag = document.PropertyBag
                    }).ToList()
            };
        }
    }
}
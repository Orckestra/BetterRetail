using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Search;
using Facet = Orckestra.Composer.Search.Facets.Facet;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;
using Suggestion = Orckestra.Composer.Search.ViewModels.Suggestion;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Services
{
    public abstract class BaseSearchViewService<TParam> where TParam : class, ISearchParam
    {
        private const string VariantPropertyBagKey = "VariantId";

        private ProductSettingsViewModel _productSettings;

        public virtual SearchType SearchType => SearchType.Searching;

        protected IDamProvider DamProvider { get; }
        protected IFacetFactory FacetFactory { get; }
        protected ILocalizationProvider LocalizationProvider { get; }
        protected IProductUrlProvider ProductUrlProvider { get; }
        protected ISearchRepository SearchRepository { get; }
        protected ISearchUrlProvider SearchUrlProvider { get; }
        protected ISelectedFacetFactory SelectedFacetFactory { get; }
        protected IViewModelMapper ViewModelMapper { get; }
        protected IPriceProvider PriceProvider { get; }
        protected IComposerContext ComposerContext { get; }
        protected IProductSettingsViewService ProductSettings { get; }
        protected IScopeViewService ScopeViewService { get; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        protected BaseSearchViewService(
            ISearchRepository searchRepository,
            IViewModelMapper viewModelMapper,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            ISearchUrlProvider searchUrlProvider,
            IFacetFactory facetFactory,
            ISelectedFacetFactory selectedFacetFactory,
            IPriceProvider priceProvider,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IScopeViewService scopeViewService,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            SearchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            SelectedFacetFactory = selectedFacetFactory ?? throw new ArgumentNullException(nameof(selectedFacetFactory));
            FacetFactory = facetFactory ?? throw new ArgumentNullException(nameof(facetFactory));
            PriceProvider = priceProvider ?? throw new ArgumentNullException(nameof(priceProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ProductSettings = productSettings ?? throw new ArgumentNullException(nameof(productSettings));
            ScopeViewService = scopeViewService ?? throw new ArgumentNullException(nameof(scopeViewService));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
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


            // Populate search results
            foreach (var resultItem in param.SearchResult.Documents)
            {
                var productSearchVm = await CreateProductSearchViewModelAsync(resultItem, param, imgDictionary).ConfigureAwait(false);
                searchResultViewModel.SearchResults.Add(productSearchVm);
            }

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

        private IList<SearchSortBy> GetSearchSortByList(SearchType searchType)
        {
            return SearchConfiguration.SearchSortBy.Where(d => !d.SearchType.HasValue || d.SearchType.Value == searchType).ToList();
        }

        protected virtual async Task<ProductSearchViewModel> CreateProductSearchViewModelAsync(ProductDocument productDocument, CreateProductSearchResultsViewModelParam<TParam> createSearchViewModelParam, IDictionary<Tuple<string, string>, ProductMainImage> imgDictionary)
        {
            var cultureInfo = createSearchViewModelParam.SearchParam.Criteria.CultureInfo;

            string variantId = null;
            if (productDocument.PropertyBag.ContainsKey(VariantPropertyBagKey))
            {
                variantId = productDocument.PropertyBag[VariantPropertyBagKey] as string;
            }

            var productSearchVm = ViewModelMapper.MapTo<ProductSearchViewModel>(productDocument, cultureInfo);
            productSearchVm.BrandId = ExtractLookupId("Brand_Facet", productDocument.PropertyBag);

            MapProductSearchViewModelInfos(productSearchVm, productDocument, cultureInfo);
            MapProductSearchViewModelUrl(productSearchVm, variantId, cultureInfo, createSearchViewModelParam.SearchParam.Criteria.BaseUrl);
            MapProductSearchViewModelImage(productSearchVm, imgDictionary);
            productSearchVm.IsAvailableToSell = await GetProductSearchViewModelAvailableForSell(productSearchVm, productDocument).ConfigureAwait(false);
            productSearchVm.Pricing = await PriceProvider.GetPriceAsync(productSearchVm.HasVariants, productDocument).ConfigureAwait(false);
            productSearchVm.IsEligibleForRecurring = RecurringOrdersSettings.Enabled && productDocument.PropertyBag.IsEligibleForRecurring();

            productSearchVm.Context["IsEligibleForRecurring "] = productSearchVm.IsEligibleForRecurring;

            return productSearchVm;
        }

        protected virtual string ExtractLookupId(string fieldName, PropertyBag propertyBag)
        {
            if (propertyBag == null) { return null; }
            var fieldValue = propertyBag.ContainsKey(fieldName) ? propertyBag[fieldName] as string : null;
            if (string.IsNullOrWhiteSpace(fieldValue)) { return null; }

            var extractedValues = fieldValue.Split(new[] { "::" }, StringSplitOptions.None);
            return extractedValues.Length < 3
                ? null
                : extractedValues[2];
        }

        protected virtual async Task<bool> GetProductSearchViewModelAvailableForSell(
            ProductSearchViewModel productSearchViewModel,
            ProductDocument productDocument)
        {
            if (productSearchViewModel.HasVariants) { return true; }

            _productSettings = await ProductSettings.GetProductSettings(ComposerContext.Scope, ComposerContext.CultureInfo).ConfigureAwait(false);

            if (!_productSettings.IsInventoryEnabled) { return true; }

            var availableStatusesForSell = ComposerConfiguration.AvailableStatusForSell;

            return (from inventoryItemAvailability
                        in productDocument.InventoryLocationStatuses
                    from inventoryItemStatuse
                        in inventoryItemAvailability.Statuses
                    select GetInventoryItemStatus(inventoryItemStatuse.Status))
                    .Any(inventoryItemStatus => availableStatusesForSell
                        .Any(availableStatusForSell => availableStatusForSell == inventoryItemStatus));
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

        protected static InventoryStatusEnum GetInventoryItemStatus(InventoryStatus inventoryStatus)
        {
            switch (inventoryStatus)
            {
                case InventoryStatus.InStock:
                    return InventoryStatusEnum.InStock;
                case InventoryStatus.OutOfStock:
                    return InventoryStatusEnum.OutOfStock;
                case InventoryStatus.PreOrder:
                    return InventoryStatusEnum.PreOrder;
                case InventoryStatus.BackOrder:
                    return InventoryStatusEnum.BackOrder;
                default:
                    return InventoryStatusEnum.Unspecified;
            }
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

        protected static bool HasVariants(ProductDocument resultItem)
        {
            if (resultItem?.PropertyBag == null)
            {
                return false;
            }

            if (!resultItem.PropertyBag.TryGetValue("GroupCount", out object variantCountObject) || variantCountObject == null)
            {
                return false;
            }

            var variantCountString = variantCountObject.ToString();

            int.TryParse(variantCountString, out int result);

            return result > 0;
        }

        protected virtual void MapProductSearchViewModelImage(
            ProductSearchViewModel productSearchVm,
            IDictionary<Tuple<string, string>,
            ProductMainImage> imgDictionary)
        {
            string productVariantId = null;
            if (!string.IsNullOrWhiteSpace(productSearchVm.VariantId))
            {
                productVariantId = productSearchVm.VariantId;
            }

            var imageKey = Tuple.Create(productSearchVm.ProductId, productVariantId);
            var imageExists = imgDictionary.TryGetValue(imageKey, out ProductMainImage mainImage);

            if (imageExists)
            {
                productSearchVm.ImageUrl = mainImage.ImageUrl;
                productSearchVm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }
        }

        protected virtual void MapProductSearchViewModelInfos(
            ProductSearchViewModel productSearchVm,
            ProductDocument productDocument,
            CultureInfo cultureInfo)
        {
            productSearchVm.DisplayName = TrimProductDisplayName(productSearchVm.FullDisplayName);

            //TODO use ProductDocument property when overture will have add it.
            if (productSearchVm.Bag.ContainsKey("DefinitionName"))
            {
                productSearchVm.DefinitionName = productSearchVm.Bag["DefinitionName"].ToString();
            }

            productSearchVm.HasVariants = HasVariants(productDocument);
        }

        protected virtual void MapProductSearchViewModelUrl(
            ProductSearchViewModel productSearchVm,
            string productVariantId,
            CultureInfo cultureInfo, string baseUrl)
        {
            productSearchVm.Url = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = cultureInfo,
                ProductId = productSearchVm.ProductId,
                ProductName = productSearchVm.FullDisplayName,
                VariantId = productVariantId,
                BaseUrl = baseUrl,
                SKU = productSearchVm.Sku
            });
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

        private static string TrimProductDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) { return string.Empty; }

            var trimmedDisplayName = displayName.Substring(
                0, Math.Min(displayName.Length, DisplayConfiguration.ProductNameMaxLength));

            return trimmedDisplayName;
        }
    }
}
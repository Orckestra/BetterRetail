using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Search.ViewModels.Metadata;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.WebAPIFilters;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.Search.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class SearchController : ApiController
    {
        private const int MAXIMUM_AUTOCOMPLETE_RESULT = 8;
        private const int MAXIMUM_SEARCH_TERMS_SUGGESTIONS = 5;
        private const int MAXIMUM_CATEGORIES_SUGGESTIONS = 4;
        private const int MAXIMUM_BRAND_SUGGESTIONS = 3;

        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected ISearchManagementRepository SearchManagementRepository { get; private set; }

        public SearchController(
            IComposerContext composerContext, 
            ISearchViewService searchViewService, 
            IInventoryLocationProvider inventoryLocationProvider, 
            ISearchUrlProvider searchUrlProvider, 
            ISearchManagementRepository searchManagementRepository)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(composerContext));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(composerContext));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(composerContext));
            SearchManagementRepository = searchManagementRepository ?? throw new ArgumentNullException(nameof(composerContext));
        }

     
        [ActionName("autocomplete")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> AutoComplete(AutoCompleteSearchViewModel request, int limit = MAXIMUM_AUTOCOMPLETE_RESULT)
        {
            var originalSearchTerms = request.Query.Trim();
            var searchTerms = MultiWordSynonymHelper.ExceptionMapOutput(originalSearchTerms, ComposerContext.CultureInfo.Name); ;

            var searchCriteria = new SearchCriteria
            {
                Keywords = searchTerms,
                NumberOfItemsPerPage = limit,
                IncludeFacets = false,
                StartingIndex = 0,
                SortBy = "score",
                SortDirection = "desc",
                Page = 1,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                InventoryLocationIds = await InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().ConfigureAwait(false),
            };

            var vm = new AutoCompleteViewModel();
            var searchResultsViewModel = await SearchViewService.GetSearchViewModelAsync(searchCriteria).ConfigureAwait(false);

            if (searchResultsViewModel != null && searchResultsViewModel.ProductSearchResults.SearchResults != null &&
                searchResultsViewModel.ProductSearchResults.TotalCount == 0)
            {
                searchCriteria.Keywords = originalSearchTerms;
                searchResultsViewModel = await SearchViewService.GetSearchViewModelAsync(searchCriteria).ConfigureAwait(false);
            }

            if (searchResultsViewModel != null && searchResultsViewModel.ProductSearchResults.SearchResults != null && searchResultsViewModel.ProductSearchResults.TotalCount > 0)
            {
                vm.Products = new List<ProductSearchViewModel>();
                vm.Products.AddRange(searchResultsViewModel.ProductSearchResults.SearchResults.Take(limit));
                vm.Products.ForEach(p => p.AsExtensionModel<IProductSearchViewModelMetadata>().SearchTerm = searchTerms);
            }

            return Ok(vm);
        }

        private static string RootCategoryId = "Root";
        private static Regex CategoryFieldName = new Regex(@"CategoryLevel(\d+)_Facet");

        [ActionName("suggestCategories")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SuggestCategories(AutoCompleteSearchViewModel request, int limit = MAXIMUM_CATEGORIES_SUGGESTIONS)
        {

            string language = ComposerContext.CultureInfo.Name;
            string searchTerm = request.Query.Trim().ToLower();

            List<Category> categories = await SearchViewService.GetAllCategories();
            List<Facet> categoryCounts = await SearchViewService.GetCategoryProductCounts(language);

            List<CategorySuggestionViewModel> categorySuggestionList = new List<CategorySuggestionViewModel>();

            categories.ForEach((category) =>
            {
                // Find the parents of the category
                List<Category> parents = new List<Category>();
                Category currentNode = category;
                while (!string.IsNullOrWhiteSpace(currentNode.PrimaryParentCategoryId) && currentNode.PrimaryParentCategoryId != RootCategoryId)
                {
                    Category parent = categories.Single((cat) => cat.Id == currentNode.PrimaryParentCategoryId);
                    parents.Add(parent);
                    currentNode = parent;
                }
                parents.Reverse();

                string displayName;
                bool success = category.DisplayName.TryGetValue(language, out displayName);
                if (success)
                {
                    FacetValue categoryCount = categoryCounts
                        .Where((facet) => int.TryParse(CategoryFieldName.Match(facet.FieldName).Groups[1].Value, out int n) && parents.Count == n - 1)
                        .Single()
                        .Values
                        .Where((facetValue) => facetValue.Value == category.DisplayName[language]).SingleOrDefault();
                    if (categoryCount != null)
                    {
                        categorySuggestionList.Add(new CategorySuggestionViewModel
                        {
                            DisplayName = displayName,
                            Parents = parents.Select((parent) => parent.DisplayName[language]).ToList(),
                            Quantity = categoryCount.Count
                        });
                    }
                }
            });

            categorySuggestionList = categorySuggestionList.OrderByDescending((category) => category.Quantity).ToList();

            List<CategorySuggestionViewModel> finalSuggestions = categorySuggestionList
                .Where((suggestion) => suggestion.DisplayName.ToLower().Contains(searchTerm))
                .Take(limit)
                .ToList();

            CategorySuggestionsViewModel vm = new CategorySuggestionsViewModel();
            vm.Suggestions = finalSuggestions.Count > 0 ? finalSuggestions : null;

            return Ok(vm);
        }
      
        [ActionName("suggestBrands")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SuggestBrands(AutoCompleteSearchViewModel request, int limit = MAXIMUM_BRAND_SUGGESTIONS)
        {
            string searchTerm = request.Query.Trim().ToLower();

            //Load the brands
            List<BrandSuggestionViewModel> brandList = new List<BrandSuggestionViewModel>();
            List<Facet> facets = await SearchViewService.GetBrandProductCounts(ComposerContext.CultureInfo.Name).ConfigureAwait(false);
            facets.Single().Values.ForEach((facetValue) =>
            {
                brandList.Add(new BrandSuggestionViewModel
                {
                    DisplayName = facetValue.DisplayName
                });
            });

            //TODO - Since locally we don't have a lot of brands, for now let's take them all
            List<BrandSuggestionViewModel> brandSuggestions = brandList.Where((suggestion) => suggestion.DisplayName.ToLower().Contains(searchTerm)).Take(limit).ToList();

            BrandSuggestionsViewModel vm = new BrandSuggestionsViewModel();
            vm.Suggestions = brandSuggestions.Count > 0 ? brandSuggestions : null;

            return Ok(vm);
        }
       
        [ActionName("suggestTerms")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SuggestTerms(AutoCompleteSearchViewModel request)
        {
            string searchTerm = request.Query.Trim().ToLower();

            List<string> suggestedTerms = await SearchManagementRepository.GetSearchSuggestedTerms(ComposerContext.Scope, ComposerContext.CultureInfo, searchTerm).ConfigureAwait(false);

            SearchTermsSuggestionsViewModel vm = new SearchTermsSuggestionsViewModel();
            if (suggestedTerms != null && suggestedTerms.Count > 0)
            {
                List<SearchTermsSuggestionViewModel> sugg = new List<SearchTermsSuggestionViewModel>();
                suggestedTerms.ForEach(term =>
                {
                    sugg.Add(new SearchTermsSuggestionViewModel { DisplayName = term });
                });
                vm.Suggestions = sugg;
            }

            return Ok(vm);
        }
       
    }

    public class AutoCompleteSearchViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Query { get; set; }
    }
}
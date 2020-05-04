using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Search;

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
        protected ISearchTermsTransformationProvider SearchTermsTransformationProvider { get; private set; }
        protected IAutocompleteProvider AutocompleteProvider { get; private set; }

        public SearchController(
            IComposerContext composerContext, 
            ISearchViewService searchViewService, 
            IInventoryLocationProvider inventoryLocationProvider, 
            ISearchTermsTransformationProvider searchTermsTransformationProvider,
            IAutocompleteProvider autocompleteProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            SearchTermsTransformationProvider = searchTermsTransformationProvider ?? throw new ArgumentNullException(nameof(searchTermsTransformationProvider));
            AutocompleteProvider = autocompleteProvider ?? throw new ArgumentNullException(nameof(autocompleteProvider));
        }

     
        [ActionName("autocomplete")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> AutoComplete(AutoCompleteSearchViewModel request, int limit = MAXIMUM_AUTOCOMPLETE_RESULT)
        {
            var originalSearchTerms = request.Query.Trim();
            var searchTerms = SearchTermsTransformationProvider.TransformSearchTerm(originalSearchTerms, ComposerContext.CultureInfo.Name); ;

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

            var searchResultsViewModel = await SearchViewService.GetSearchViewModelAsync(searchCriteria).ConfigureAwait(false);
            if (searchResultsViewModel.ProductSearchResults?.TotalCount == 0 && originalSearchTerms != searchTerms)
            {
                searchCriteria.Keywords = originalSearchTerms;
                searchResultsViewModel = await SearchViewService.GetSearchViewModelAsync(searchCriteria).ConfigureAwait(false);
            }

            var vm = new AutoCompleteViewModel() { Suggestions = new List<ProductSearchViewModel>() };
            if (searchResultsViewModel.ProductSearchResults?.SearchResults?.Count > 0)
            {
                vm.Suggestions = searchResultsViewModel.ProductSearchResults.SearchResults.Take(limit)
                    .Select(p => { p.SearchTerm = searchTerms; return p; })
                    .ToList();
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

            foreach (var category in categories)
            {
                if (!category.DisplayName.TryGetValue(language, out string displayName)) continue;

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

                FacetValue categoryCount = categoryCounts
                        .Where((facet) => int.TryParse(CategoryFieldName.Match(facet.FieldName).Groups[1].Value, out int n) && parents.Count == n - 1)
                        .FirstOrDefault()?
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
            };

            List<CategorySuggestionViewModel> finalSuggestions = categorySuggestionList
                .Where((suggestion) => suggestion.DisplayName.ToLower().Contains(searchTerm))
                .OrderByDescending((category) => category.Quantity)
                .Take(limit)
                .ToList();

            CategorySuggestionsViewModel vm = new CategorySuggestionsViewModel
            {
                Suggestions = finalSuggestions
            };

            return Ok(vm);
        }
      
        [ActionName("suggestBrands")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SuggestBrands(AutoCompleteSearchViewModel request, int limit = MAXIMUM_BRAND_SUGGESTIONS)
        {
            string searchTerm = request.Query.Trim().ToLower();

            List<Facet> facets = await SearchViewService.GetBrandProductCounts(ComposerContext.CultureInfo.Name).ConfigureAwait(false);
            List<BrandSuggestionViewModel> brandList = facets.Single().Values.Select(facetValue => new BrandSuggestionViewModel
            {
                DisplayName = facetValue.DisplayName
            }).ToList();

            BrandSuggestionsViewModel vm = new BrandSuggestionsViewModel()
            {
                Suggestions = brandList
                    .Where((suggestion) => suggestion.DisplayName.ToLower().Contains(searchTerm))
                    .OrderBy(x => x.DisplayName)
                    .Take(limit).ToList()
            };
            return Ok(vm);
        }
       
        [ActionName("suggestTerms")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SuggestTerms(AutoCompleteSearchViewModel request, int limit = MAXIMUM_SEARCH_TERMS_SUGGESTIONS)
        {
            string searchTerm = request.Query.Trim().ToLower();

            List<string> suggestedTerms = await AutocompleteProvider.GetSearchSuggestedTerms(ComposerContext.CultureInfo, searchTerm).ConfigureAwait(false);

            SearchTermsSuggestionsViewModel vm = new SearchTermsSuggestionsViewModel()
            {
                Suggestions = suggestedTerms
                    .OrderBy(term => term)
                    .Select(term => new SearchTermsSuggestionViewModel { DisplayName = term })
                    .Take(limit).ToList()
            };
            return Ok(vm);
        }
    }
}
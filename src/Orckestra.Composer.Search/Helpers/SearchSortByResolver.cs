using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;

// TODO: This needs to be refactored. I took it as is for the moment and wrapped it
// in the SearchSortByResolver, but this really needs to be cleaned up.
// Need a real search url provider
//
// Revisited 1: moved hardcoded fields and behavior into SearchConfiguration.
//
// Uncle Bob.

namespace Orckestra.Composer.Search.Helpers
{
    /// <summary>
    /// Utility for building Display Name/URL pairs for the sort by drop down used in search.
    /// </summary>
    public class SearchSortByResolver<TParam>
        where TParam : class, ISearchParam
    {
        private readonly string _relevance = "Relevance";
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IList<SearchSortBy> _searchSortBy;
        private readonly Func<CreateSearchPaginationParam<TParam>, string> _generateUrl;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="localizationProvider"></param>
        /// <param name="searchSortBy"></param>
        /// <param name="generateUrl"></param>
        public SearchSortByResolver(ILocalizationProvider localizationProvider, IList<SearchSortBy> searchSortBy, Func<CreateSearchPaginationParam<TParam>, string> generateUrl)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            _generateUrl = generateUrl ?? throw new ArgumentNullException(nameof(generateUrl));
            _searchSortBy = searchSortBy;

        }

        /// <summary>
        /// Resolves the specified search results view model.
        /// </summary>
        /// <param name="searchResultsViewModel">The search results view model.</param>
        /// <param name="searchCriteria">The search criteria.</param>
        public void Resolve(ProductSearchResultsViewModel searchResultsViewModel, TParam searchCriteria)
        {
            ResolveProductAvailableSortBy(searchCriteria, searchResultsViewModel);
        }

        /// <summary>
        /// Provides the UI a list of sort by information (display & complete url).  It also provide the current selected sort by.
        /// </summary>
        /// <param name="param">The query criteria.</param>
        /// <param name="searchResultsViewModel">The search results view model.</param>
        private void ResolveProductAvailableSortBy(TParam param, ProductSearchResultsViewModel searchResultsViewModel)
        {
            searchResultsViewModel.SelectedSortBy = GetSelectedSortBy(param.Criteria);
            searchResultsViewModel.AvailableSortBys = GetAvailableSortBys(param);
        }

        /// <summary>
        /// Get Configuration for the Queried Search
        /// </summary>
        /// <returns></returns>
        private SelectedSortBy GetSelectedSortBy(SearchCriteria criteria)
        {
            var selectedSortByConfig = _searchSortBy
                .FirstOrDefault(c => c.Field == criteria.SortBy && c.Direction == criteria.SortDirection);

            selectedSortByConfig = selectedSortByConfig ?? _searchSortBy.First();

            var selectedSortBy = new SelectedSortBy
            {
                DisplayName = _localizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category    = selectedSortByConfig.LocalizationCategory,
                    Key         = selectedSortByConfig.LocalizationKey,
                    CultureInfo = criteria.CultureInfo
                }),

                Value = selectedSortByConfig.Field
            };

            return selectedSortBy;
        }

        /// <summary>
        /// List all possible Sorts based on the configuration
        /// </summary>
        /// <returns></returns>
        private IList<SortBy> GetAvailableSortBys(TParam param)
        {
            var sortBys = new List<SortBy>();

            var criteriaToBuildSortByUrls = (TParam)param.Clone();
            criteriaToBuildSortByUrls.Criteria.Page = 1;

            foreach (var sortByConfig in _searchSortBy)
            {
                criteriaToBuildSortByUrls.Criteria.SortBy = sortByConfig.Field;
                criteriaToBuildSortByUrls.Criteria.SortDirection = sortByConfig.Direction;

                sortBys.Add(new SortBy
                {
                    DisplayName = _localizationProvider.GetLocalizedString(new GetLocalizedParam
                    {
                        Category    = sortByConfig.LocalizationCategory,
                        Key         = sortByConfig.LocalizationKey,
                        CultureInfo = criteriaToBuildSortByUrls.Criteria.CultureInfo,
                    }),

                    Url = _generateUrl(new CreateSearchPaginationParam<TParam>()
                    {
                        SearchParameters = criteriaToBuildSortByUrls
                    }),
                    SortingType = string.IsNullOrWhiteSpace(sortByConfig.Field) ? _relevance : $"{sortByConfig.Field}{sortByConfig.Direction}"
                });
            }

            return sortBys;
        }
    }
}
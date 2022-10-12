using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Search.Providers.FacetPredicate;
using Orckestra.Composer.Search.Providers.SelectedFacet;

namespace Orckestra.Composer.Search
{
    public static class SearchConfiguration
    {
        private static ProviderRegistry<ISelectedFacetProvider> _selectedFacetProviderRegistry = new SelectedFacetProviderRegistry();
        private static ProviderRegistry<IFacetPredicateProvider> _facetPredicateProviderRegistry = new FacetPredicateProviderRegistry();
        private static ProviderRegistry<IFacetProvider> _facetProviderRegistry = new FacetProviderRegistry();

        /// <summary>
        ///     Localization Category to use when formatting the facet values
        ///     <see cref="IFacetLocalizationProvider.GetFormattedFacetValueTitle" />
        /// </summary>
        public static string FormatFacetLocalizationCategory = "FacetFormatting";

        /// <summary>
        ///     Pattern used to build the Localization Key when formatting facet values
        ///     <see cref="IFacetLocalizationProvider.GetFormattedFacetValueTitle" />
        /// </summary>
        public static string FormatFacetLocalizationKeyPattern = "{FieldName}_{Value}";

        /// <summary>
        ///     Pattern used to build the Localization Key when formatting facet title
        ///     <see cref="IFacetLocalizationProvider.GetFormattedFacetTitle" />
        /// </summary>
        public static string FormatFacetTitleLocalizationKeyPattern = "{FieldName}_Title";

        /// <summary>
        ///     Pattern used to build the Localization Key when formatting facet values
        ///     <see cref="IFacetLocalizationProvider.GetFormattedRangeFacetValues" />
        /// </summary>
        public static string FormatRangeFacetMaxValueLocalizationKeyPattern = "{FieldName}_MaxValuePattern";

        /// <summary>
        ///     Pattern used to build the Localization Key when formatting facet values
        ///     <see cref="IFacetLocalizationProvider.GetFormattedRangeFacetValues" />
        /// </summary>
        public static string FormatRangeFacetMinMaxValueLocalizationKeyPattern = "{FieldName}_MinMaxValuePattern";

        /// <summary>
        ///     Pattern used to build the Localization Key when formatting facet values
        ///     <see cref="IFacetLocalizationProvider.GetFormattedRangeFacetValues" />
        /// </summary>
        public static string FormatRangeFacetMinValueLocalizationKeyPattern = "{FieldName}_MinValuePattern";

        /// <summary>
        /// Sortable field as displayed by the field selector
        /// "Field" will display the query string "sortBy"
        /// </summary>
        public static IList<SearchSortBy> SearchSortBy = new List<SearchSortBy>
        {
            new SearchSortBy
            {
                Field = "score",
                Direction = "desc",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_Relevance_Option",
                SearchType = SearchType.Searching
            },
            new SearchSortBy
            {
                Field = string.Empty,
                Direction = "",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_Featured_Option",
                SearchType = SearchType.Browsing
            },
            new SearchSortBy
            {
                Field = "CurrentPrice",
                Direction = "asc",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_PriceLowHigh_Option"
            },
            new SearchSortBy
            {
                Field = "CurrentPrice",
                Direction = "desc",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_PriceHighLow_Option"
            },
            new SearchSortBy
            {
                Field = "DisplayName_Sort",
                Direction = "asc",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_NameAZ_Option"
            },
            new SearchSortBy
            {
                Field = "DisplayName_Sort",
                Direction = "desc",
                LocalizationCategory = "List-Search",
                LocalizationKey = "F_NameZA_Option"
            }
        };

        /// <summary>
        ///     Gets or sets the default size of the image.
        /// </summary>
        /// <value>
        ///     The default size of the image.
        /// </value>
        public static string DefaultImageSize { get; set; } = "L";

        public static ProviderRegistry<IFacetPredicateProvider> FacetPredicateProviderRegistry
        {
            get { return _facetPredicateProviderRegistry; }
            set { _facetPredicateProviderRegistry = value; }
        }

        public static ProviderRegistry<IFacetProvider> FacetProviderRegistry
        {
            get { return _facetProviderRegistry; }
            set { _facetProviderRegistry = value; }
        }

        /// <summary>
        ///     Gets or sets the range separator for a range facet value.
        /// </summary>
        /// <value>
        ///     The range separator for a range facet value.
        /// </value>
        public static char FacetRangeValueSplitter { get; set; } = '|';

        /// <summary>
        ///     Gets or sets the filter name parameter prefix.
        /// </summary>
        /// <value>
        ///     The filter name parameter prefix.
        /// </value>
        public static string FilterNameParameterPrefix { get; set; } = "fn";

        /// <summary>
        ///     Gets or sets the filter value parameter prefix.
        /// </summary>
        /// <value>
        ///     The filter value parameter prefix.
        /// </value>
        public static string FilterValueParameterPrefix { get; set; } = "fv";

        /// <summary>
        ///     Gets or sets the maximum items per page.
        /// </summary>
        /// <value>
        ///     The maximum items per page.
        /// </value>
        public static int MaxItemsPerPage { get; set; } = 12;
        /// <summary>
        /// Maximum pages' urls for pagination
        /// </summary>
        public static int MaximumPages { get; set; } = 5;

        /// <summary>
        /// If true, then MaximumPages will be ignored, and all pages' urls will be rendered
        /// </summary>
        public static bool ShowAllPages { get; set; }

        /// <summary>
        ///     Gets or sets the multi-facet value splitter.
        /// </summary>
        /// <value>
        ///     The multi facet value splitter.
        /// </value>
        public static char MultiFacetValueSplitter { get; set; } = '|';

        public static ProviderRegistry<ISelectedFacetProvider> SelectedFacetProviderRegistry
        {
            get { return _selectedFacetProviderRegistry; }
            set { _selectedFacetProviderRegistry = value; }
        }

        /// <summary>
        ///     Gets or sets the size of the thumbnail image.
        /// </summary>
        /// <value>
        ///     The size of the thumbnail image.
        /// </value>
        public static string ThumbnailImageSize { get; set; } = "S";

        public static bool BlockStarSearchs { get; set; }

        /// <summary>
        /// Whether search term should be automatically corrected
        /// </summary>
        public static bool AutoCorrectSearchTerms { get; set; } = true;

        /// <summary>
        /// The prefix for the the category facet field name to indentify which facets are for categories
        /// </summary>
        public static string CategoryFacetFiledNamePrefix { get; set; } = "CategoryLevel";
    }
}
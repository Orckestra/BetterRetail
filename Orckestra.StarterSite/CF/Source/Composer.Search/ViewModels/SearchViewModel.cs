using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public sealed class SearchViewModel : BaseViewModel
    {
        public string Keywords { get; set; }

        public int Page { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public SelectedFacets SelectedFacets { get; set; }

        public ProductSearchResultsViewModel ProductSearchResults { get; set; }
    }
}

using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.SearchQuery.ViewModels
{
    public class SearchQueryViewModel: BaseViewModel
    {
        public SelectedFacets SelectedFacets { get; set; }

        public ProductSearchResultsViewModel ProductSearchResults { get; set; }
    }
}

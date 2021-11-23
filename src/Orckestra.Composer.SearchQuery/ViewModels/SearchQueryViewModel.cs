using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.ViewModels
{
    public class SearchQueryViewModel: BaseViewModel
    {
        public SearchQueryType QueryType { get; set; }
        public string QueryName { get; set; }
        public FacetSettingsViewModel FacetSettings { get; set; }
        public ProductSearchResultsViewModel ProductSearchResults { get; set; }
    }
}

using Orckestra.Composer.Search.ViewModels;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.ViewModels
{
    public class SearchQueryViewModel: BaseSearchViewModel
    {
        public SearchQueryType QueryType { get; set; }
        public string QueryName { get; set; }
    }
}

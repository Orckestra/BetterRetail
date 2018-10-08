using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class SearchQueryProductParams
    {
        public SearchQueryType QueryType { get; set; }
        public string QueryName { get; set; }
        public string ScopeId { get; set; }
        public string CultureName { get; set; }
        public SearchCriteria Criteria { get; internal set; }
    }
}

using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class GetSearchQueriesParam
    {
        public string Scope { get; set; }

        public SearchQueryType QueryType { get; set; }
    }
}

using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.Requests
{
    public class GetQueryFacetsRequest
    {
        public string QueryString { get; set; }

        public string QueryName { get; set; }

        public SearchQueryType QueryType { get; set; }
    }
}

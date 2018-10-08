using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class SearchQueryAsyncParam
    {
        public string Name { get; set; }

        public string Scope { get; set; }

        public SearchQueryType QueryType { get; set; }
    }
}

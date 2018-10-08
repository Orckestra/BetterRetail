using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class GetSearchQueryViewModelParams
    {
        public string QueryName { get; set; }
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public List<string> InventoryLocationIds { get; set; }
        public SearchCriteria Criteria { get; set; }
        public SearchQueryType QueryType { get; set; }
    }
}

using Orckestra.Composer.Providers;
using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.C1CMS.Queries.Controllers
{
    public class ProductSetController: SearchQueryController
    {
        public ProductSetController(IComposerContext composerContext, 
            ISearchUrlProvider searchUrlProvider, 
            ISearchQueryContext searchQueryContext, 
            IInventoryLocationProvider inventoryLocationProvider) : base(composerContext, searchUrlProvider, searchQueryContext, inventoryLocationProvider)
        {
            QueryType = SearchQueryType.ProductSet;
        }
    }
}

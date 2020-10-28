using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.C1CMS.Queries.Controllers
{
    public class ProductSetController : SearchQueryController
    {
        public ProductSetController(IComposerContext composerContext,
            ISearchQueryContext searchQueryContext) : base(composerContext, searchQueryContext)
        {
            QueryType = SearchQueryType.ProductSet;
        }
    }
}

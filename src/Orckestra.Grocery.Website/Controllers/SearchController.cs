using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Search.Context;

namespace Orckestra.Composer.Grocery.Website.Controllers
{
    public class SearchController : SearchBaseController
    {
        public SearchController(ISearchRequestContext searchRequestContext)
             : base(searchRequestContext)
        {
        }
    }
}
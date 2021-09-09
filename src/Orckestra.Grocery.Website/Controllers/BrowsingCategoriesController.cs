using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Grocery.Website.Controllers
{
    public class BrowsingCategoriesController : BrowsingCategoriesBaseController
    {
        public BrowsingCategoriesController(
            IComposerContext composerContext, 
            IBrowseCategoryRequestContext requestContext, 
            IPageService pageService, 
            ICategoryMetaContext categoryMetaContext) 
            : base(composerContext, requestContext, pageService, categoryMetaContext)
        {
        }
    }
}
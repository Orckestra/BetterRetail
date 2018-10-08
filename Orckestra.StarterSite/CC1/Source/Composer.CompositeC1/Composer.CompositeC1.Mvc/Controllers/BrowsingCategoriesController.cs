using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class BrowsingCategoriesController : BrowsingCategoriesBaseController
    {
        public BrowsingCategoriesController(
            IComposerContext composerContext,
            ISearchUrlProvider searchUrlProvider,
            IBrowseCategoryRequestContext requestContext,
            ICategoryViewService categoryViewService,
            ILanguageSwitchService languageSwitchService,
            IPageService pageService,
            IInventoryLocationProvider inventoryLocationProvider) 
            : base(composerContext, searchUrlProvider, requestContext, categoryViewService, languageSwitchService, pageService, inventoryLocationProvider)
        {
        }
    }
}
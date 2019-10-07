using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class BrowsingCategoriesController : BrowsingCategoriesBaseController
    {
        public BrowsingCategoriesController(
            IComposerContext composerContext, 
            IBrowseCategoryRequestContext requestContext, 
            ILanguageSwitchService languageSwitchService, 
            IPageService pageService, 
            ICategoryMetaContext categoryMetaContext) 
            : base(composerContext, requestContext, languageSwitchService, pageService, categoryMetaContext)
        {
        }
    }
}
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class HeaderController : HeaderBaseController
    {
        public HeaderController(IPageService pageService, 
            IComposerContext composerContext, 
            ILanguageSwitchService languageSwitchService, 
            IBreadcrumbViewService breadcrumbViewService) 

            : base(
            pageService, 
            composerContext, 
            languageSwitchService, 
            breadcrumbViewService)
        {
        }
    }
}
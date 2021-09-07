using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class HeaderController : HeaderBaseController
    {
        public HeaderController(IPageService pageService, 
            IComposerContext composerContext, 
            ILanguageSwitchService languageSwitchService) 

            : base(
            pageService, 
            composerContext, 
            languageSwitchService)
        {
        }
    }
}
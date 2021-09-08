using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class HeaderController : HeaderBaseController
    {
        public HeaderController(IPageService pageService, 
            IComposerContext composerContext) 

            : base(
            pageService, 
            composerContext)
        {
        }
    }
}
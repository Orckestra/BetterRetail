using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class FooterController : FooterBaseController
    {
        public FooterController(IComposerContext composerContext, ILocalizationProvider localizationProvider)
            : base(composerContext, localizationProvider)
        {
        }
    }
}
using System;
using System.Web.Mvc;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class FooterBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public FooterBaseController(IComposerContext composerContext, ILocalizationProvider localizationProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public ActionResult SocialLinks()
        {
            var getLocalizedFollowUsParam = new GetLocalizedParam
            {
                Category = "General",
                Key = "L_FollowUs",
                CultureInfo = ComposerContext.CultureInfo
            };

            var localizedFollowUsLabel = LocalizationProvider.GetLocalizedString(getLocalizedFollowUsParam);
            return View("FollowUs", model: string.IsNullOrWhiteSpace(localizedFollowUsLabel) ? string.Empty : localizedFollowUsLabel);
        }
    }
}
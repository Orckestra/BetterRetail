using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class FooterBaseController : Controller
    {
        protected IHomeViewService HomeViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public FooterBaseController(
            IComposerContext composerContext,
            IHomeViewService homeViewService,
            ILocalizationProvider localizationProvider)
        {
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }
            if (homeViewService == null) { throw new ArgumentNullException(nameof(homeViewService)); }
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }

            ComposerContext = composerContext;
            HomeViewService = homeViewService;
            LocalizationProvider = localizationProvider;
        }

        public ActionResult OptionalLinks()
        {
            var optionalLinksViewModel = HomeViewService.GetFooterOptionalLinksViewModel(ComposerContext.CultureInfo).Result;

            return View("FooterOptionalLinks", optionalLinksViewModel);
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

        public ActionResult Copyright()
        {
            var copyrightValue = HomeViewService.GetCopyright(ComposerContext.CultureInfo).Result;

            return Content(copyrightValue);
        }

        public ActionResult MainFooter()
        {
            var param = new GetFooterParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
            };

            var footerViewModel = HomeViewService.GetFooterViewModel(param).Result;

            return View("FooterContainer", footerViewModel);
        }
    }
}
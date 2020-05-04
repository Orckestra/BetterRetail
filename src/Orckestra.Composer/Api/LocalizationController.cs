using System;
using System.Web.Mvc;
using Orckestra.Composer.MvcFilters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Api
{
    /// <summary>
    /// This controller is actually an MvcController used to simulate a WebApi controller
    /// to benefit from the LocalizationCache attributes.
    /// 
    /// We hide this abuse by registring the MvcController on the same path then any other WebApi
    /// </summary>
    [ValidateLanguage]
    public class LocalizationController : Controller
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IComposerContext _composerContext;

        public LocalizationController(ILocalizationProvider localizationProvider, IComposerContext composerContext)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            _composerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        [HttpGet]
        public ActionResult GetTree(string language)
        {
            var tree = _localizationProvider.GetLocalizationTreeAsync(_composerContext.CultureInfo).Result;
            var cache = ComposerConfiguration.LocalizationCacheOptions;

            //Only this work in Sitecore. OutputCache attribute doesn't work.
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(cache.Duration));

            //Security Note: It is safe to AllowGet only as long as the dto is not an array.
            return Json(tree, JsonRequestBehavior.AllowGet); 
        }
    }
}
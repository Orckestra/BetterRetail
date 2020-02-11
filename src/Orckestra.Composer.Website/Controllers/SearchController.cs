using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class SearchController : SearchBaseController
    {
        public SearchController(
            IComposerContext composerContext, 
            IPageService pageService, 
            ISearchRequestContext searchRequestContext, 
            ILanguageSwitchService languageSwitchService, 
            ISearchBreadcrumbViewService searchBreadcrumbViewService,
            ISearchUrlProvider searchUrlProvider)
            : base(
            composerContext, 
            pageService, 
            searchRequestContext, 
            languageSwitchService, 
            searchBreadcrumbViewService,
            searchUrlProvider)
        {
        }
    }
}
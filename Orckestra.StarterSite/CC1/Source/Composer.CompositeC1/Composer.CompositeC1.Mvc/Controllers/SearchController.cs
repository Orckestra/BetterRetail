using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class SearchController : SearchBaseController
    {
        public SearchController(
            IComposerContext composerContext, 
            IPageService pageService, 
            ISearchRequestContext searchRequestContext, 
            ILanguageSwitchService languageSwitchService, 
            ISearchBreadcrumbViewService searchBreadcrumbViewService,
            ISearchUrlProvider searchUrlProvider,
            ISiteConfiguration siteConfiguration) 
            
            : base(
            composerContext, 
            pageService, 
            searchRequestContext, 
            languageSwitchService, 
            searchBreadcrumbViewService,
            searchUrlProvider,
            siteConfiguration)
        {
        }
    }
}
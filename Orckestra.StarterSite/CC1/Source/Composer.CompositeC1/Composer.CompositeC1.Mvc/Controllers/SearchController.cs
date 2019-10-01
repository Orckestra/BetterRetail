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
            ISearchUrlProvider urlProvider, 
            ISearchBreadcrumbViewService searchBreadcrumbViewService,
            IInventoryLocationProvider inventoryLocationProvider,
            ISearchUrlProvider searchUrlProvider,
            ISiteConfiguration siteConfiguration) 
            
            : base(
            composerContext, 
            pageService, 
            searchRequestContext, 
            languageSwitchService, 
            urlProvider, 
            searchBreadcrumbViewService,
            inventoryLocationProvider,
            searchUrlProvider,
            siteConfiguration)
        {
        }
    }
}
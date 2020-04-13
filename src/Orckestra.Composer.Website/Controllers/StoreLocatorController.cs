using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class StoreLocatorController : StoreLocatorBaseController
    {
        public StoreLocatorController(IComposerContext composerContext,
            IStoreViewService storeViewService,
            IStoreDirectoryViewService storeDirectoryViewService,
            IStoreUrlProvider storeUrlProvider,
            IBreadcrumbViewService breadcrumbViewService,
            ILanguageSwitchService languageSwitchViewService) :
            base(composerContext, storeViewService, storeDirectoryViewService, storeUrlProvider, breadcrumbViewService, languageSwitchViewService)
        {
        }
    }
}
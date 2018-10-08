using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class StoreLocatorController : StoreLocatorBaseController
    {
        public StoreLocatorController(IComposerContext composerContext,
            IStoreViewService storeViewService,
            IStoreLocatorViewService storeLocatorViewService,
            IStoreDirectoryViewService storeDirectoryViewService,
            IStoreUrlProvider storeUrlProvider,
            IBreadcrumbViewService breadcrumbViewService,
            ILanguageSwitchService languageSwitchViewService) :
            base(composerContext, storeViewService, storeLocatorViewService, storeDirectoryViewService, storeUrlProvider, breadcrumbViewService, languageSwitchViewService)
        {
        }
    }
}
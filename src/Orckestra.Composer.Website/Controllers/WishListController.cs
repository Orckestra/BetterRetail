using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;

namespace Orckestra.Composer.Website.Controllers
{
    public class WishListController : WishListBaseController
    {
        public WishListController(IComposerContext composerContext,
           ICustomerViewService customerViewService,
           IBreadcrumbViewService breadcrumbViewService,
           ILocalizationProvider localizationProvider,
           IWishListUrlProvider wishListUrlProvider,
           IWishListViewService wishListViewService,
           IWebsiteContext websiteContext) :
            base(composerContext, customerViewService, breadcrumbViewService, localizationProvider, wishListUrlProvider, wishListViewService, websiteContext) { }
    }
}
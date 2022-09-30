using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Utils;
using System;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class WishListBaseController: Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ICustomerViewService CustomerViewService { get; private set; }
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IWishListUrlProvider WishListUrlProvider { get; private set; }
        protected IWishListViewService WishLisViewService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }

        protected WishListBaseController(
           IComposerContext composerContext,
           ICustomerViewService customerViewService,
           IBreadcrumbViewService breadcrumbViewService,
           ILocalizationProvider localizationProvider,
           IWishListUrlProvider wishListUrlProvider,
           IWishListViewService wishListViewService
            )
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CustomerViewService = customerViewService ?? throw new ArgumentNullException(nameof(customerViewService));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            WishListUrlProvider = wishListUrlProvider ?? throw new ArgumentNullException(nameof(wishListUrlProvider));
            WishLisViewService = wishListViewService ?? throw new ArgumentNullException(nameof(wishListViewService));
        }

        public ActionResult WishListInHeader()
        {
            var vm = new WishListViewModel
            {
                Url = WishListUrlProvider.GetWishListUrl(new GetWishListUrlParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
                })
            };

            vm.Context.Add("IsAuthenticated", ComposerContext.IsAuthenticated);

            return View("WishListInHeader", vm);
        }
    }
}
using Composite.Core.Xml;
using Composite.Data;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.MyAccount.Parameters;
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

        protected WishListBaseController(
           IComposerContext composerContext,
           ICustomerViewService customerViewService,
           IBreadcrumbViewService breadcrumbViewService,
           ILocalizationProvider localizationProvider,
           IWishListUrlProvider wishListUrlProvider,
           IWishListViewService wishListViewService
            )
        {
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }
            if (customerViewService == null) { throw new ArgumentNullException(nameof(customerViewService)); }
            if (breadcrumbViewService == null) { throw new ArgumentNullException(nameof(breadcrumbViewService)); }
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (wishListUrlProvider == null) { throw new ArgumentNullException(nameof(wishListUrlProvider)); }
            if (wishListViewService == null) { throw new ArgumentNullException(nameof(wishListViewService)); }

            ComposerContext = composerContext;
            CustomerViewService = customerViewService;
            BreadcrumbViewService = breadcrumbViewService;
            LocalizationProvider = localizationProvider;
            WishListUrlProvider = wishListUrlProvider;
            WishLisViewService = wishListViewService;
        }

        public ActionResult WishListInHeader()
        {
            var vm = new WishListViewModel
            {
                Url = WishListUrlProvider.GetWishListUrl(new GetWishListUrlParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                    WebsiteId = SitemapNavigator.CurrentHomePageId
                })
            };

            vm.Context.Add("IsAuthenticated", ComposerContext.IsAuthenticated);

            return View("WishListInHeader", vm);
        }

        public ActionResult SharedWishList(string id, XhtmlDocument emptyWishListContent)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var param = SharedWishListTokenizer.DecryptToken(id);

            if (param == null)
            {
                return HttpNotFound();
            }

            var customerStatus = CustomerViewService.GetAccountStatusViewModelAsync(new GetAccountStatusViewModelParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    Scope = param.Scope,
                    CustomerId = param.CustomerId
                }).Result;

            if (customerStatus == null || customerStatus.Status == AccountStatusEnum.Inactive)
            {
                return HttpNotFound();
            }

            var vm = WishLisViewService.GetWishListViewModelAsync(new GetCartParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                CartName = CartConfiguration.WishlistCartName,
                Scope = param.Scope,
                CustomerId = param.CustomerId
            }).Result;

            if (vm != null && vm.TotalQuantity == 0 && emptyWishListContent != null)
            {
                return View("SharedWishListContainer", new {TotalQuantity = 0, EmptyContent = emptyWishListContent.Body});
            }

            return View("SharedWishListContainer", vm);
        }

        public ActionResult SharedWishListTitle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("SharedWishListHeaderBlade");
            }

            var param = SharedWishListTokenizer.DecryptToken(id);

            if (param == null)
            {
                return HttpNotFound();
            }

            var vm = CustomerViewService.GetAccountHeaderViewModelAsync(new GetAccountHeaderViewModelParam
            {
                Scope = param.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = param.CustomerId
            }).Result;

            if (vm == null)
            {
                return HttpNotFound();
            }

            return View("SharedWishListHeaderContainer", vm);
        }

    }
}

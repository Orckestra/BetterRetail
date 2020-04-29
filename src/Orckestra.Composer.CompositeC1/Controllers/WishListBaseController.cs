using System;
using System.Web.Mvc;
using Composite.Core.Xml;
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
           IWishListViewService wishListViewService,
           IWebsiteContext websiteContext
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

        public ActionResult SharedWishList(string id, XhtmlDocument emptyWishListContent)
        {
            if (string.IsNullOrEmpty(id)) { return HttpNotFound(); }

            var param = SharedWishListTokenizer.DecryptToken(id);

            if (param == null) { return HttpNotFound(); }

            var customerStatus = CustomerViewService.GetAccountStatusViewModelAsync(new GetAccountStatusViewModelParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    Scope = param.Scope,
                    CustomerId = param.CustomerId
                }).Result;

            if (customerStatus == null || customerStatus.Status == AccountStatusEnum.Inactive) { return HttpNotFound(); }

            var vm = WishLisViewService.GetWishListViewModelAsync(new GetCartParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                CartName = CartConfiguration.WishlistCartName,
                Scope = param.Scope,
                CustomerId = param.CustomerId
            }).Result;

            return vm != null && vm.TotalQuantity == 0 && emptyWishListContent != null
                ? View("SharedWishListContainer", new {TotalQuantity = 0, EmptyContent = emptyWishListContent.Body})
                : View("SharedWishListContainer", vm);
        }

        public ActionResult SharedWishListTitle(string id)
        {
            if (string.IsNullOrEmpty(id)) { return View("SharedWishListHeaderBlade"); }

            var param = SharedWishListTokenizer.DecryptToken(id);

            if (param == null) { return HttpNotFound(); }

            var vm = CustomerViewService.GetAccountHeaderViewModelAsync(new GetAccountHeaderViewModelParam
            {
                Scope = param.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = param.CustomerId
            }).Result;

            return vm == null ? HttpNotFound() : (ActionResult)View("SharedWishListHeaderContainer", vm);
        }
    }
}
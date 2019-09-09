using System;
using System.Collections.Generic;
using Composite.Data;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.MyAccount;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class MyAccountViewService : IMyAccountViewService
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected IWishListUrlProvider WishListUrlProvider { get; private set; }
        protected IPageService PageService { get; private set; }

        public MyAccountViewService(
            IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IWishListUrlProvider wishListUrlProvider,
            IPageService pageService)
        {
            if (composerContext == null) throw new ArgumentNullException("composerContext");
            if (myAccountUrlProvider == null) throw new ArgumentNullException("myAccountUrlProvider");
            if (orderUrlProvider == null) throw new ArgumentNullException("orderUrlProvider");
            if (pageService == null) throw new ArgumentNullException("pageService");
            if (wishListUrlProvider == null) throw new ArgumentNullException("wishListUrlProvider");

            ComposerContext = composerContext;
            MyAccountUrlProvider = myAccountUrlProvider;
            OrderUrlProvider = orderUrlProvider;
            WishListUrlProvider = wishListUrlProvider;
            PageService = pageService;
        }

        public virtual MenuViewModel CreateMenu(string currentUrl)
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                WebsiteId = SitemapNavigator.CurrentHomePageId
            };
            var myAccountUrl = MyAccountUrlProvider.GetMyAccountUrl(urlParam);
            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(urlParam);
            var myOrderHistoryUrl = OrderUrlProvider.GetOrderHistoryUrl(new GetOrderUrlParameter { CultureInfo = ComposerContext.CultureInfo });
            var myWishListUrl = WishListUrlProvider.GetWishListUrl(new GetWishListUrlParam {CultureInfo = ComposerContext.CultureInfo});

            var currentPageId = new Guid(currentUrl);
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration();

            var menu = new MenuViewModel { MenuItems = new List<MenuItemViewModel>() };

            var myAccountPage = PageService.GetPage(pagesConfiguration.MyAccountPageId, ComposerContext.CultureInfo);
            var myAddressPage = PageService.GetPage(pagesConfiguration.AddressListPageId, ComposerContext.CultureInfo);
            var myOrderHistoryPage = PageService.GetPage(pagesConfiguration.OrderHistoryPageId, ComposerContext.CultureInfo);
            var myWishListage = PageService.GetPage(pagesConfiguration.MyWishListPageId, ComposerContext.CultureInfo);

            var myAccountMenuItem = new MenuItemViewModel
            {
                Name = myAccountPage.MenuTitle,
                Url = myAccountUrl,
                IsActive = currentPageId == pagesConfiguration.MyAccountPageId ||
                           currentPageId == pagesConfiguration.ChangePasswordPageId
            };

            var myAdressesMenuItem = new MenuItemViewModel
            {
                Name = myAddressPage.MenuTitle,
                Url = addressListUrl,
                IsActive = currentPageId == pagesConfiguration.AddressListPageId ||
                           currentPageId == pagesConfiguration.AddAddressPageId ||
                           currentPageId == pagesConfiguration.UpdateAddressPageId
            };

             var myWishListMenuItem = new MenuItemViewModel
             {
                 Name = myWishListage.MenuTitle,
                 Url = myWishListUrl,
                 IsActive = currentPageId == pagesConfiguration.MyWishListPageId
             };

            var myOrderHistoryMenuItem = new MenuItemViewModel
            {
                Name = myOrderHistoryPage.MenuTitle,
                Url = myOrderHistoryUrl,
                IsActive = currentPageId == pagesConfiguration.OrderHistoryPageId || currentPageId == pagesConfiguration.OrderDetailsPageId
            };

            menu.MenuItems.AddRange(new List<MenuItemViewModel> { myAccountMenuItem, myWishListMenuItem, myAdressesMenuItem, myOrderHistoryMenuItem });

            return menu;
        }
    }
}

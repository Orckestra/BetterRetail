using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.MyAccount;

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
            var myAccountUrl = MyAccountUrlProvider.GetMyAccountUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });
            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });
            var walletListUrl = MyAccountUrlProvider.GetWalletListUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });
            var myOrderHistoryUrl = OrderUrlProvider.GetOrderHistoryUrl(new GetOrderUrlParameter { CultureInfo = ComposerContext.CultureInfo });
            var myWishListUrl = WishListUrlProvider.GetWishListUrl(new GetWishListUrlParam {CultureInfo = ComposerContext.CultureInfo});

            var currentPageId = new Guid(currentUrl);

            var menu = new MenuViewModel { MenuItems = new List<MenuItemViewModel>() };

            var myAccountPage = PageService.GetPage(PagesConfiguration.MyAccountPageId, ComposerContext.CultureInfo);
            var myAddressPage = PageService.GetPage(PagesConfiguration.AddressListPageId, ComposerContext.CultureInfo);
            var myWalletPage = PageService.GetPage(PagesConfiguration.WalletListPageId, ComposerContext.CultureInfo);
            var myOrderHistoryPage = PageService.GetPage(PagesConfiguration.OrderHistoryPageId, ComposerContext.CultureInfo);
            var myWishListage = PageService.GetPage(PagesConfiguration.MyWishListPageId, ComposerContext.CultureInfo);

            var myAccountMenuItem = new MenuItemViewModel
            {
                Name = myAccountPage.MenuTitle,
                Url = myAccountUrl,
                IsActive = currentPageId == PagesConfiguration.MyAccountPageId ||
                           currentPageId == PagesConfiguration.ChangePasswordPageId
            };

            var myAdressesMenuItem = new MenuItemViewModel
            {
                Name = myAddressPage.MenuTitle,
                Url = addressListUrl,
                IsActive = currentPageId == PagesConfiguration.AddressListPageId ||
                           currentPageId == PagesConfiguration.AddAddressPageId ||
                           currentPageId == PagesConfiguration.UpdateAddressPageId
            };

            var myWalletMenuItem = new MenuItemViewModel
            {
                Name = myWalletPage.MenuTitle,
                Url = walletListUrl,
                IsActive = currentPageId == PagesConfiguration.WalletListPageId
            };

            var myWishListMenuItem = new MenuItemViewModel
             {
                 Name = myWishListage.MenuTitle,
                 Url = myWishListUrl,
                 IsActive = currentPageId == PagesConfiguration.MyWishListPageId
             };

            var myOrderHistoryMenuItem = new MenuItemViewModel
            {
                Name = myOrderHistoryPage.MenuTitle,
                Url = myOrderHistoryUrl,
                IsActive = currentPageId == PagesConfiguration.OrderHistoryPageId || currentPageId ==  PagesConfiguration.OrderDetailsPageId
            };

            menu.MenuItems.AddRange(new List<MenuItemViewModel> { myAccountMenuItem, myWishListMenuItem, myAdressesMenuItem, myWalletMenuItem, myOrderHistoryMenuItem });

            return menu;
        }
    }
}

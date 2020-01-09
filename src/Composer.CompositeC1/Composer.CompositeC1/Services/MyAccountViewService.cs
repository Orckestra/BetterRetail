using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.MyAccount;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Generic;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class MyAccountViewService : IMyAccountViewService
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected IWishListUrlProvider WishListUrlProvider { get; private set; }
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected IPageService PageService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }


        public MyAccountViewService(
            IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IWishListUrlProvider wishListUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IPageService pageService,
            IWebsiteContext websiteContext,
            IRecurringOrdersSettings recurringOrdersSettings,
            ISiteConfiguration siteConfiguration)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
            WishListUrlProvider = wishListUrlProvider ?? throw new ArgumentNullException(nameof(wishListUrlProvider));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public virtual MenuViewModel CreateMenu(string currentUrl)
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            bool recurringOrderConfigEnabled = RecurringOrdersSettings.Enabled;
            var myAccountUrl = MyAccountUrlProvider.GetMyAccountUrl(urlParam);
            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(urlParam);
            var myOrderHistoryUrl = OrderUrlProvider.GetOrderHistoryUrl(new GetOrderUrlParameter { CultureInfo = ComposerContext.CultureInfo });
            var myWishListUrl = WishListUrlProvider.GetWishListUrl(new GetWishListUrlParam {CultureInfo = ComposerContext.CultureInfo});
            var myRecurringScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleUrl(new GetRecurringScheduleUrlParam { CultureInfo = ComposerContext.CultureInfo });

            var currentPageId = new Guid(currentUrl);

            var menu = new MenuViewModel { MenuItems = new List<MenuItemViewModel>() };
            var pageConfiguration = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, WebsiteContext.WebsiteId);
            var myAccountPage = PageService.GetPage(pageConfiguration.MyAccountPageId, ComposerContext.CultureInfo);
            var myAddressPage = PageService.GetPage(pageConfiguration.AddressListPageId, ComposerContext.CultureInfo);
            var myOrderHistoryPage = PageService.GetPage(pageConfiguration.OrderHistoryPageId, ComposerContext.CultureInfo);
            var myWishListage = PageService.GetPage(pageConfiguration.MyWishListPageId, ComposerContext.CultureInfo);
            var myRecurringSchedulePage = PageService.GetPage(RecurringOrdersSettings.RecurringSchedulePageId, ComposerContext.CultureInfo);

            var myAccountMenuItem = new MenuItemViewModel
            {
                Name = myAccountPage.MenuTitle,
                Url = myAccountUrl,
                IsActive = currentPageId == pageConfiguration.MyAccountPageId ||
                           currentPageId == pageConfiguration.ChangePasswordPageId
            };

            var myAdressesMenuItem = new MenuItemViewModel
            {
                Name = myAddressPage.MenuTitle,
                Url = addressListUrl,
                IsActive = currentPageId == pageConfiguration.AddressListPageId ||
                           currentPageId == pageConfiguration.AddAddressPageId ||
                           currentPageId == pageConfiguration.UpdateAddressPageId
            };

             var myWishListMenuItem = new MenuItemViewModel
             {
                 Name = myWishListage.MenuTitle,
                 Url = myWishListUrl,
                 IsActive = currentPageId == pageConfiguration.MyWishListPageId
             };

            var myOrderHistoryMenuItem = new MenuItemViewModel
            {
                Name = myOrderHistoryPage.MenuTitle,
                Url = myOrderHistoryUrl,
                IsActive = currentPageId == pageConfiguration.OrderHistoryPageId ||
                            currentPageId == pageConfiguration.OrderDetailsPageId ||
                            currentPageId == RecurringOrdersSettings.RecurringCartDetailsPageId
            };

            var myRecurringScheduleMenuItem = new MenuItemViewModel
            {
                Name = myRecurringSchedulePage.MenuTitle,
                Url = myRecurringScheduleUrl,
                IsActive = currentPageId == RecurringOrdersSettings.RecurringSchedulePageId || currentPageId == RecurringOrdersSettings.RecurringScheduleDetailsPageId
            };

            menu.MenuItems.AddRange(new List<MenuItemViewModel> { myAccountMenuItem, myWishListMenuItem, myAdressesMenuItem, myOrderHistoryMenuItem});

            if (recurringOrderConfigEnabled)
            {
                menu.MenuItems.Add(myRecurringScheduleMenuItem);
            }

            return menu;
        }
    }
}

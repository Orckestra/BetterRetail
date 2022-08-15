using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Settings;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Recipes.Settings;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.MyAccount;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryMyAccountViewService : MyAccountViewService
    {
        private IRecipeSettings RecipeSettings { get; }
        private IMyUsualsSettings MyUsualsSettings { get; }

        public GroceryMyAccountViewService(IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IWishListUrlProvider wishListUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IPageService pageService,
            IWebsiteContext websiteContext,
            IRecurringOrdersSettings recurringOrdersSettings,
            ISiteConfiguration siteConfiguration,
            IRecipeSettings recipeSettings,
            IMyUsualsSettings myUsualsSettings)
            : base(composerContext,
                myAccountUrlProvider,
                orderUrlProvider,
                wishListUrlProvider,
                recurringScheduleUrlProvider,
                pageService,
                websiteContext,
                recurringOrdersSettings,
                siteConfiguration)
        {
            RecipeSettings = recipeSettings ?? throw new ArgumentNullException(nameof(recipeSettings));
            MyUsualsSettings = myUsualsSettings ?? throw new ArgumentNullException(nameof(myUsualsSettings));
        }

        public override MenuViewModel CreateMenu(string currentUrl)
        {
            var menu = base.CreateMenu(currentUrl);
            menu.MenuItems.Add(GetMenuItem(currentUrl, MyUsualsSettings.MyUsualsPageId));
            menu.MenuItems.Add(GetMenuItem(currentUrl, RecipeSettings.RecipeFavoritesPageId));
            return menu;
        }

        private MenuItemViewModel GetMenuItem(string currentUrl, Guid pageId)
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };
            var currentPageId = new Guid(currentUrl);
            var url = GetUrl(urlParam, pageId);
            var page = PageService.GetPage(pageId, ComposerContext.CultureInfo);

            var menuItem = new MenuItemViewModel
            {
                Name = page.MenuTitle,
                Url = url,
                IsActive = currentPageId == pageId
            };
            return menuItem;
        }

        private string GetUrl(BaseUrlParameter param, Guid pageId)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (pageId == null) { throw new ArgumentNullException(nameof(pageId)); }

            var url = PageService.GetPageUrl(pageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

    }
}

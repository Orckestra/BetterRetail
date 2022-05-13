using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Recipes.Settings;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.MyAccount;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryMyAccountViewService: MyAccountViewService
    {
        private IRecipeSettings RecipeSettings { get; }
        public GroceryMyAccountViewService(IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IWishListUrlProvider wishListUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IPageService pageService,
            IWebsiteContext websiteContext,
            IRecurringOrdersSettings recurringOrdersSettings,
            ISiteConfiguration siteConfiguration,
            IRecipeSettings recipeSettings)
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
        }

        public override MenuViewModel CreateMenu(string currentUrl)
        {
            var menu = base.CreateMenu(currentUrl);
            menu.MenuItems.Add(GetFavoriteRecipesMenuItem(currentUrl));
            return menu;
        }

        protected virtual MenuItemViewModel GetFavoriteRecipesMenuItem(string currentUrl)
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };
            var currentPageId = new Guid(currentUrl);
            var recipeFavoritesUrl = GetRecipeFavoriteUrl(urlParam);
            var recipeFavoritesPage = PageService.GetPage(RecipeSettings.RecipeFavoritesPageId, ComposerContext.CultureInfo);

            var recipeFavoritesMenuItem = new MenuItemViewModel
            {
                Name = recipeFavoritesPage.MenuTitle,
                Url = recipeFavoritesUrl,
                IsActive = currentPageId == RecipeSettings.RecipeFavoritesPageId
            };
            return recipeFavoritesMenuItem;
        }

        private string GetRecipeFavoriteUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(RecipeSettings.RecipeFavoritesPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}

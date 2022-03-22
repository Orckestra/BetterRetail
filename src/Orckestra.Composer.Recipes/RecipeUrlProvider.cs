using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.ContentSearch.DataTypes;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.Recipes
{
    public class RecipeUrlProvider : IRecipeUrlProvider
    {
        protected IPageService PageService { get; }
        protected IWebsiteContext WebsiteContext { get; }
        protected ISiteConfiguration SiteConfiguration { get; }

        protected string RecipesTabPath { get; }

        public RecipeUrlProvider(ISiteConfiguration siteConfiguration, IPageService pageService, IWebsiteContext websiteContext)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            RecipesTabPath = DataFacade.GetData<IContentTab>().FirstOrDefault(i => i.DataTypes.Contains("IRecipe"))?.Title;
        }

        /// <summary>
        /// Builds the search URL.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The base search URL is null or empty. Unable to build the search URL.</exception>
        public virtual string BuildRecipeMealTypeUrl(Guid mealType, CultureInfo cultureInfo)
        {
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.SearchPageId, cultureInfo);
            if (url == null) return null;

            var finalUrl = UrlFormatter.AppendQueryString($"{url}/{RecipesTabPath}", BuildMealTypeQueryString(mealType));
            return finalUrl;
        }

        private NameValueCollection BuildMealTypeQueryString(Guid mealType)
        {
            var queryString = new NameValueCollection();
            queryString.Add("keywords", "*");
            queryString.Add($"f_IRecipe.MealType_{mealType.ToString()}", "on");
            return queryString;
        }
    }
}
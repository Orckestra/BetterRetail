using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Core.Threading;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class CategoryBrowsingUrlProvider : BaseSearchUrlProvider, ICategoryBrowsingUrlProvider
    {
        public ICacheProvider CacheProvider { get; set; }
        public IPageService PageService { get; set; }
        public IWebsiteContext WebsiteContext { get; set; }


        public CategoryBrowsingUrlProvider(ICacheProvider cacheProvider, IPageService pageService, IWebsiteContext websiteContext)
        {
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
        }

        public string BuildCategoryBrowsingUrl(BuildCategoryBrowsingUrlParam param)
        {
            var websiteId = WebsiteContext.WebsiteId;
            var scope = DataScopeManager.CurrentDataScope;

            var key = $"{websiteId}{param.CategoryId}{param.IsAllProductsPage}{scope}";

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.CategoryUrls, key, param.CultureInfo);

            var categoryBaseUrl = CacheProvider.GetOrAdd(cacheKey, () =>
            {
                // Because of ConfigureAwait(false), we lost context here.
                // Therefore we need to re-initialize C1 context because getting the Url.
                using (ThreadDataManager.EnsureInitialize())
                {
                    var map = GetCategoryMap(scope, param.CultureInfo);

                    if (!map.TryGetValue(param.CategoryId, out var categoryPages))
                    {
                        return null;
                    }

                    foreach (var categoryPage in categoryPages.Where(_ => _.IsAllProductsPage == param.IsAllProductsPage))
                    {
                        if (HasAncestor(categoryPage.PageId, websiteId))
                        {
                            return PageService.GetPageUrl(categoryPage.PageId, param.CultureInfo);
                        }
                    }
                }

                return null;
            });

            // Category page is not found
            if (categoryBaseUrl == null) { return null; }

            var finalUrl = UrlFormatter.AppendQueryString(categoryBaseUrl, BuildSearchQueryString(new BuildSearchUrlParam
            {
                SearchCriteria = param.Criteria
            }));

            return finalUrl;
        }

        private bool HasAncestor(Guid categoryPagePageId, Guid websiteId)
        {
            var currentPage = categoryPagePageId;
            while (currentPage != Guid.Empty)
            {
                currentPage = PageManager.GetParentId(currentPage);
                if (currentPage == websiteId) return true;
            }

            return false;
        }

        private Dictionary<string, List<CategoryPage>> GetCategoryMap(DataScopeIdentifier scope, CultureInfo cultureInfo)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.CategoryUrls, $"map_{scope.Name}", cultureInfo);

            return CacheProvider.GetOrAdd(cacheKey, () =>
            {
                using (new DataConnection(scope.ToPublicationScope(), cultureInfo))
                {
                    return DataFacade.GetData<CategoryPage>()
                        .GroupBy(_ => _.CategoryId)
                        .ToDictionary(_ => _.Key, _ => _.ToList());
                }
            });
        }
    }
}
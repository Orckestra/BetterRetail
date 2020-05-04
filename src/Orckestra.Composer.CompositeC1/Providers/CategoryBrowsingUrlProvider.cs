using System;
using System.Globalization;
using Composite.Core.Threading;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class CategoryBrowsingUrlProvider : BaseSearchUrlProvider, ICategoryBrowsingUrlProvider
    {
        public ICacheProvider CacheProvider { get; set; }
        public IPageService PageService { get; set; }

        public CategoryBrowsingUrlProvider(ICacheProvider cacheProvider, IPageService pageService)
        {
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
        }

        public string BuildCategoryBrowsingUrl(BuildCategoryBrowsingUrlParam param)
        {
            var itemId = GuidUtility.Create(GetNamespace(param.IsAllProductsPage), param.CategoryId);
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.CategoryUrls, itemId.ToString(), param.CultureInfo);

           var categoryBaseUrl = CacheProvider.GetOrAdd(cacheKey, () => GetPageUrl(itemId, param.CultureInfo));

            // Category page is not found
            if (categoryBaseUrl == null) { return null; }

            var finalUrl = UrlFormatter.AppendQueryString(categoryBaseUrl, BuildSearchQueryString(new BuildSearchUrlParam
            {
                SearchCriteria = param.Criteria
            }));

            return finalUrl;
        }

        private Guid GetNamespace(bool isAllProductsPage)
        {
            return isAllProductsPage
                ? CategoriesNamespaces.AllProductsNamespaceId
                : CategoriesNamespaces.CategoryNamespaceId;
        }

        private string GetPageUrl(Guid pageId, CultureInfo cultureInfo)
        {
            // Because of ConfigureAwait(false), we lost context here.
            // Therefore we need to re-initialize C1 context because getting the Url.
            using (ThreadDataManager.EnsureInitialize())
            {
                return PageService.GetPageUrl(pageId, cultureInfo);
            }
        }
    }
}
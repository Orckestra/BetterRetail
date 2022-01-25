using Composite.Core.Threading;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture.Caching;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.C1CMS.Queries.Providers
{
    public class SearchQueryUrlProvider : BaseSearchUrlProvider, ISearchQueryUrlProvider
    {
        public ICacheProvider CacheProvider { get; set; }
        public IPageService PageService { get; set; }

        public string BuildSearchQueryUrl(BuildSearchQueryUrlParam param)
        {

            var cacheKey = new CacheKey("SearchQuery", param.PageId.ToString(), param.CultureInfo);

            var categoryBaseUrl = CacheProvider.GetOrAddAsync(cacheKey, () => Task.FromResult(GetPageUrl(param.PageId, param.CultureInfo))).Result;

            // Category page is not found
            if (categoryBaseUrl == null)
            {
                return null;
            }

            var finalUrl = UrlFormatter.AppendQueryString(categoryBaseUrl, BuildSearchQueryString(new BuildSearchUrlParam
            {
                SearchCriteria = param.Criteria
            }));
            return finalUrl;
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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using Composite.Core;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class SearchUrlProvider : BaseSearchUrlProvider, ISearchUrlProvider
    {
        protected IPageService PageService { get; }
        protected IWebsiteContext WebsiteContext { get;}
        protected ISiteConfiguration SiteConfiguration { get; }

        public SearchUrlProvider(ISiteConfiguration siteConfiguration, IPageService pageService, IWebsiteContext websiteContext)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        /// <summary>
        /// Builds the search URL.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The base search URL is null or empty. Unable to build the search URL.</exception>
        public virtual string BuildSearchUrl(BuildSearchUrlParam param)
        {
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.SearchCriteria.CultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.SearchPageId, param.SearchCriteria.CultureInfo);
            if (url == null) return null;

            var finalUrl = UrlFormatter.AppendQueryString(url, BuildSearchQueryString(param));
            return finalUrl;
        }

        /// <summary>
        /// Builds the selected facets.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        public virtual IEnumerable<SearchFilter> BuildSelectedFacets(NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0) return null;

            SortedDictionary<string, SearchFilter> result = new SortedDictionary<string, SearchFilter>();

            foreach (string element in collection)
            {
                var match = Regex.Match(
                    element,
                    $"{SearchConfiguration.FilterNameParameterPrefix}([0-9]+)",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (!match.Success) continue;

                var fnValue = collection[SearchConfiguration.FilterNameParameterPrefix + match.Groups[1].Value];

                if (string.IsNullOrEmpty(fnValue))
                {
                    Log.LogWarning(nameof(SearchUrlProvider), $"{element} facet key value is null or empty.");
                    continue;
                }

                var fvValue = collection[SearchConfiguration.FilterValueParameterPrefix + match.Groups[1].Value];

                if (string.IsNullOrEmpty(fvValue)) continue;

                result.Add(fnValue, new SearchFilter
                {
                    Name = HttpUtility.UrlDecode(fnValue),
                    Value = HttpUtility.UrlDecode(fvValue)
                });
            }

            return result.Values;
        }
    }
}
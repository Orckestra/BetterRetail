using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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
        public virtual IEnumerable<SearchFilter> BuildSelectedFacets(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                return null;
            }

            // TODO use NameValue collection directly.
            var queryStringTokens = queryString.ToString().Split('&');

            var filterNameTokens = queryStringTokens
                .Where(x => x.StartsWith(SearchConfiguration.FilterNameParameterPrefix, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x)
                .Select(x => x.Split('='))
                .ToList();

            var filterValueTokens = queryStringTokens
                .Where(x => x.StartsWith(SearchConfiguration.FilterValueParameterPrefix, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x)
                .Select(x => x.Split('='))
                .ToList();

            var filters = new List<SearchFilter>();

            foreach (var filterNameToken in filterNameTokens)
            {
                var filterPosition = Regex.Replace(filterNameToken[0], "[A-Za-z=]", string.Empty);
                var filterValueToken = filterValueTokens
                    .FirstOrDefault(x => x[0].Equals(SearchConfiguration.FilterValueParameterPrefix + filterPosition, StringComparison.InvariantCultureIgnoreCase));

                if (filterValueToken == null)
                {
                    continue;
                }

                filters.Add(new SearchFilter
                {
                    Name = HttpUtility.UrlDecode(filterNameToken[1]),
                    // TODO: Split here and support list of values here instead of doing it when building facet predicates.
                    Value = HttpUtility.UrlDecode(filterValueToken[1])
                });
            }

            return filters;
        }
    }
}
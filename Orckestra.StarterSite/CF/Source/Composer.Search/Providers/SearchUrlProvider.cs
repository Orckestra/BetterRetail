using System;
using System.Web.Routing;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Search.Providers
{
	public class SearchUrlProvider : BaseSearchUrlProvider, ISearchUrlProvider
    {
        protected ILocalizationProvider LocalizationProvider { get; private set; }

		public SearchUrlProvider(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider;
        }

        public void RegisterRoutes(RouteCollection routeCollection)
        {
            // implementation of ISearchUrlProvider RegisterRoutes method.
        }

        /// <summary>
        /// Builds the search URL.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The base search URL is null or empty. Unable to build the search URL.</exception>
        public string BuildSearchUrl(BuildSearchUrlParam param)
        {
            var relativeUrl = string.Format("/{0}/{1}",
                param.SearchCriteria.CultureInfo.Name,
                LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category    = "Mvc-Cms",
                    Key         = "SearchInUrl", 
                    CultureInfo = param.SearchCriteria.CultureInfo
                })
            );

            var url = new Uri(relativeUrl, UriKind.Relative);

			var finalUrl = UrlFormatter.AppendQueryString(url.ToString(), BuildSearchQueryString(param));

            return finalUrl;
        }


		
    }
}
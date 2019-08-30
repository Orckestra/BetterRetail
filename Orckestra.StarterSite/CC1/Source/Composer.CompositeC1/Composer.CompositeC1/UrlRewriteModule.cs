using Composite.Core.Logging;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Orckestra.Composer.CompositeC1
{
    public class UrlRewriteModule : IHttpModule
    {
        private const string ProductUrlPathIndicatorRegex = "^p-.*";
        private const string StoreUrlPathIndicatorRegex = "^s-.*";

        private readonly IPageService _pageService;
        private PagesConfiguration PagesConfiguration;

        public UrlRewriteModule()
        {
            _pageService = new PageService();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs args)
        {
            var context = ((HttpApplication) sender).Context;
            var url = context.Request.Url;

            var urlPathSegments = url.Segments.Skip(1).Select(s => s.TrimEnd('/')).ToList(); //always skip the first segment, since it's always the first dash

            var newUrl = string.Empty;

            try
            {
                /** 
                 * Logic below identifies the type of page that is being accessed, based on the path segments that are identified, and then applies a redirect
                 * to the URL which is mapped to a page controller.
                 * For instance, a product page is identified because one of the path segments starts with "p-" and has at least 1 subsequent path (for the product id),
                 * and is then redirected to the product page with the product/variant id sent as query strings.
                 * A store page is identified with one of the segments starting with "s-" and has 1 subsequent path (for the store number).
                 * 
                 * Note that this method of redirection creates a hard dependency the cutlure being explicitly passed in the URL. To account for this, we ASSUME
                 * that the culture is always passed as the first parameters in URL.
                 * This assumption is based on the ProductUrlProvider and StoreUrlProvider classes, which are responsbile for building these URLs.
                 * */
                PagesConfiguration = SiteConfiguration.GetPagesConfiguration();
                int pathPatternIndex = -1;
                if ((pathPatternIndex = GetUrlPathIndexForSpecificPagePattern(urlPathSegments, ProductUrlPathIndicatorRegex)) > -1)
                {
                    CultureInfo urlCulture = GetCultureFromUrlPath(urlPathSegments);
                    var productPageUrl = _pageService.GetPageUrl(PagesConfiguration.ProductPageId, urlCulture);

                    string productId = urlPathSegments.ElementAtOrDefault(pathPatternIndex + 1); //product Id is always in the path after the product path indicator
                    string variantId = urlPathSegments.ElementAtOrDefault(pathPatternIndex + 2); //variant Id is always in the path after the product id

                    newUrl = UrlFormatter.AppendQueryString(productPageUrl, new NameValueCollection
                    {
                        {"id", productId},
                        {"variantId", variantId}
                    });
                }
                else if ((pathPatternIndex = GetUrlPathIndexForSpecificPagePattern(urlPathSegments, StoreUrlPathIndicatorRegex)) > -1)
                {
                    CultureInfo urlCulture = GetCultureFromUrlPath(urlPathSegments);
                    var storePageUrl = _pageService.GetPageUrl(PagesConfiguration.StorePageId, urlCulture);

                    string storeNumber = urlPathSegments.ElementAtOrDefault(pathPatternIndex + 1);

                    newUrl = UrlFormatter.AppendQueryString(storePageUrl, new NameValueCollection
                    {
                        {"storeNumber", storeNumber}
                    });
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Composer Url Rewrite Module", ex);

                throw new HttpException(404, "Could not redirect page");
            }

            if (!string.IsNullOrEmpty(newUrl))
                context.RewritePath(newUrl);
        }

        /// <summary>
        /// Finds and returns the index of the path segment matching the provided regex from a list of path segments.
        /// Returns -1 if no match is found.
        /// </summary>
        /// <param name="urlPathSegments">List of path segments for a URL path</param>
        /// <param name="pathIdentifierRegex">The regex to use for to find a match for the path segment</param>
        /// <param name="minLengthAfterPathId">The minimum amount of subsequent path segments the identified path should have. This ensures that the identified path
        /// is not the last one in the URL</param>
        /// <returns></returns>
        private int GetUrlPathIndexForSpecificPagePattern(List<string> urlPathSegments, string pathIdentifierRegex, int minLengthAfterPathId = 1)
        {
            if (urlPathSegments == null || !urlPathSegments.Any() 
                || string.IsNullOrWhiteSpace(pathIdentifierRegex))
                return -1;
            
            int pathIdIndex = urlPathSegments.FindIndex(s => Regex.IsMatch(s, pathIdentifierRegex));
            if (pathIdIndex == -1 //Make sure the path identifier is found
                || (pathIdIndex + minLengthAfterPathId) + 1 > urlPathSegments.Count //Make sure the length after the path identifier is respected
                )
                return -1;

            return pathIdIndex;
        }

        /// <summary>
        /// Returns the culture retrieved from the provided URL path segments.
        /// Null is returned if a valid culture could not be retrieved.
        /// </summary>
        /// <param name="urlPathSegments">The segments to retrieve the URL form</param>
        /// <param name="cultureIndexInPath">The index of the culture in the URL path</param>
        /// <returns></returns>
        private CultureInfo GetCultureFromUrlPath(List<string> urlPathSegments, int cultureIndexInPath = 0)
        {
            CultureInfo urlCulture = null;

            try
            {
                urlCulture = CultureInfo.CreateSpecificCulture(urlPathSegments?.ElementAtOrDefault(cultureIndexInPath));
            }
            catch (Exception)
            {
                //Silently ignore.
            }
            
            return urlCulture;
        }

        public void Dispose()
        {
            // Do not try to unregister OnBeginRequest here, otherwise it will throw an exception.
        }
    }
}

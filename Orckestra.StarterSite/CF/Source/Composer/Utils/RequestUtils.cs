using System;
using System.Configuration;
using System.Net.Http;
using System.Web;

namespace Orckestra.Composer.Utils
{
    public static class RequestUtils
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["oc-baseUrl"];

        /// <summary>
        /// Get Base Url with Controller Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Uri GetBaseUrl(HttpRequestBase request)
        {
            var uri = GetBaseUrl(request.Url, request.ApplicationPath);

            return uri;
        }

        /// <summary>
        /// Get Base Url with ApiController Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Uri GetBaseUrl(HttpRequestMessage request)
        {
            var uri = GetBaseUrl(request.RequestUri, string.Empty);

            return uri;
        }

        /// <summary>
        /// Get the base url based on the Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public static Uri GetBaseUrl(Uri uri, string applicationPath = "")
        {
            var result = BuildBaseUrl(uri, applicationPath, BaseUrl);

            return result;
        }

        private static Uri BuildBaseUrl(Uri requestUrl, string applicationPath, string baseUrl)
        {
            var baseUri = string.IsNullOrEmpty(baseUrl)
                ? new Uri(requestUrl.GetLeftPart(UriPartial.Authority), UriKind.Absolute)
                : new Uri(baseUrl, UriKind.Absolute);

            //in case the site is in load balance with several https bindings using different ports (i.e.: 4431)
            //by default the :80 and :443 are not displayed but different ports are explicity displayed in the "Authority" property
            var builder = new UriBuilder(baseUri) { Port = -1 };

            return new Uri(
                builder.Uri,
                new Uri(applicationPath, UriKind.Relative));
        }
    }
}

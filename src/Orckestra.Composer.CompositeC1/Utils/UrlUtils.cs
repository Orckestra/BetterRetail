
using Orckestra.Composer.Utils;
using System;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Utils
{
    public static class UrlUtils
    {
        public static string ParseUrlMappingName(string requestedPath)
        {
            var startIndex = requestedPath.IndexOf('/', Composite.Core.WebClient.UrlUtils.PublicRootPath.Length) + 1;

            if (startIndex > 0 && requestedPath.Length > startIndex)
            {
                var endIndex = requestedPath.IndexOf('/', startIndex + 1) - 1;
                if (endIndex < 0)
                {
                    endIndex = requestedPath.Length - 1;
                }

                if (endIndex > startIndex)
                {
                    return requestedPath.Substring(startIndex, endIndex - startIndex + 1);
                }
            }

            return null;
        }

        private static bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri _);
        }

        public static string ToAbsolute(string url)
        {
            if (IsAbsoluteUrl(url)) { return url; }

            Uri absoluteUrl = RequestUtils.GetBaseUrl(HttpContext.Current.Request.Url);
            string relativeUrl = VirtualPathUtility.ToAbsolute(url);

            var fullUrl = new Uri(absoluteUrl, relativeUrl);

            return fullUrl.ToString();
        }
    }
}

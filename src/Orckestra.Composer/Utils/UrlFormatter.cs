using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Utils
{
    public static class UrlFormatter
    {
        private static readonly Regex InvalidCharactersExclusion = new Regex(@"[^A-Za-z 0-9\-]*", RegexOptions.Compiled);
        private static readonly Regex ManyDashExclusion = new Regex(@"\-{2,}", RegexOptions.Compiled);
        private static readonly Regex IsAbsoluteUrlRegex = new Regex(":\\/\\/", RegexOptions.Compiled|RegexOptions.IgnoreCase);
        private static readonly Regex InvalidSchemaRegex = new Regex("^(javascript|data):.*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase); 
        private static readonly Uri FakeDomainUri = new Uri("http://foo.com/");

        /// <summary>
        /// Formats the passed string to show it as URL-Friendly.
        /// </summary>
        /// <param name="data">Data to format.</param>
        /// <returns></returns>
        public static string Format(string data)
        {
            var urlData = data.Trim().ToLowerInvariant();
            urlData = urlData.Replace(' ', '-');
            urlData = urlData.Normalize(NormalizationForm.FormD);
            urlData = InvalidCharactersExclusion.Replace(urlData, string.Empty);
            urlData = ManyDashExclusion.Replace(urlData, "-");

            return urlData;
        }

        public static string FormatProductName(string data)
        {
            var urlData = Format(data);
            urlData = urlData.Replace("p-", "p");
            urlData = urlData.Replace("P-", "P");

            return urlData;
        }

        public static Uri GetUriFromString(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(url)); }

            var isAbsoluteUrl = IsAbsoluteUrl(url);

            var uri = isAbsoluteUrl
                ? new Uri(url, UriKind.Absolute)
                : new Uri(url, UriKind.Relative);

            return uri;
        }

        public static bool IsAbsoluteUrl(string url)
        {
            return IsAbsoluteUrlRegex.IsMatch(url);
        }

        /// <summary>
        /// Appends query string values to the specified url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string AppendQueryString(string url, NameValueCollection query)
        {
            if (string.IsNullOrWhiteSpace(url)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(url)); }
            if (query == null) { throw new ArgumentNullException(nameof(query)); }

            var isAbsoluteUrl = IsAbsoluteUrlRegex.IsMatch(url);

            var uri = isAbsoluteUrl
                ? new Uri(url, UriKind.Absolute)
                : new Uri(FakeDomainUri, new Uri(url, UriKind.Relative));

            var queryStrings = MergeQueryStrings(uri.ParseQueryString(), query);

            var finalUrl = BuildUrl(uri, queryStrings, isAbsoluteUrl);
            return finalUrl;
        }

        public static string ToUrlString(NameValueCollection queryString)
        {
            if (queryString == null) { throw new ArgumentNullException(nameof(queryString)); }

            var urlBuilder = new StringBuilder();
            for (int i = 0; i < queryString.Count; i++)
            {
                var symbol = i == 0 ? "?" : "&";
                var key = queryString.Keys[i];
                var value = queryString.Get(i);

                urlBuilder.AppendFormat("{0}{1}={2}", symbol, key, HttpUtility.UrlEncode(value));
            }

            return urlBuilder.ToString();
        }

        private static NameValueCollection MergeQueryStrings(NameValueCollection originalQs, NameValueCollection qsToAdd)
        {
            var qs = new NameValueCollection(originalQs);

            for (int i = 0; i < qsToAdd.Count; i++)
            {
                var key = qsToAdd.Keys[i];
                var value = qsToAdd.Get(i);

                qs.Set(key, value);
            }

            return qs;
        }

        private static string BuildUrl(Uri uri, NameValueCollection queryString, bool isAbsoluteUrl)
        {
            var urlBuilder = new StringBuilder();

            if (isAbsoluteUrl)
            {
                urlBuilder.Append(uri.GetLeftPart(UriPartial.Path));
            }
            else
            {
                var absolutePath = uri.AbsolutePath;

                // If path is virtual remove the first slash to ensure that url can be resolved
                if (absolutePath.StartsWith("/~"))
                {
                    absolutePath = absolutePath.TrimStart('/');
                }

                urlBuilder.Append(absolutePath);
            }

            for (int i = 0; i < queryString.Count; i++)
            {
                var symbol = i == 0 ? "?" : "&";
                var key = queryString.Keys[i];
                var value = queryString.Get(i);

                urlBuilder.AppendFormat("{0}{1}={2}", symbol, key, HttpUtility.UrlEncode(value));
            }

            return urlBuilder.ToString();
        }

        public static bool IsUrlLocal(string baseUrl, string url)
        {
            if (!IsAbsoluteUrl(url)) { return true; }

            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            }

            var isLocal = url.StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase);
            return isLocal;
        }

        public static bool IsValidScheme(string returnUrl)
        {
            var isValid = !InvalidSchemaRegex.IsMatch(returnUrl);
            return isValid;
        }

        public static bool IsReturnUrlValid(string baseUrl, string returnUrl)
        {
            if (!IsAbsoluteUrl(returnUrl))
            {
                var isSchemeValid = IsValidScheme(returnUrl);
                return isSchemeValid;
            }

            return IsUrlLocal(baseUrl, returnUrl);
        }
    }
}

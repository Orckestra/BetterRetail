using System;
using System.Collections.Specialized;
using System.Linq;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Cache;
using Orckestra.Composer.CompositeC1.DataTypes;

namespace Orckestra.Composer.CompositeC1.Utils
{
    public static class C1Helper
    {
        internal static QueryCache<UrlTarget, Guid> _urlTargetCache = new QueryCache<UrlTarget, Guid>(_ => _.Id);
        internal static QueryCache<CssStyle, Guid> _cssStyleCache = new QueryCache<CssStyle, Guid>(_ => _.Id);

        public static bool IsUrlPagePublished(string url)
        {
            if (IsExternalLink(url)) return true;

            var guidStr = GetPageGuidFromUrl(url);
            if (string.IsNullOrWhiteSpace(guidStr) || !Guid.TryParse(guidStr, out var pageId))
            {
                return false;
            }

            using (new DataConnection(PublicationScope.Published))
            {
                return PageManager.GetPageById(pageId) != null;
            }
        }

        public static string GetPageGuidFromUrl(string url)
        {
            var urlPageGuid = url.Split(new[] {"~/page(", ")"}, StringSplitOptions.RemoveEmptyEntries);
            return urlPageGuid.Any() ? urlPageGuid[0] : string.Empty;
        }

        public static bool IsExternalLink(string url)
        {
            return !url.StartsWith("~/page");
        }

        public static string GetMediaUrl(string mediaPath)
        {
            if (mediaPath == null) return string.Empty;

            string[] parts = mediaPath.Split(':');

            string mediaStore = parts[0];
            Guid mediaId = new Guid(parts[1]);

            string mediaUrl = MediaUrls.BuildUrl(new MediaUrlData { MediaStore = mediaStore, MediaId = mediaId, QueryParameters = new NameValueCollection() },
                UrlKind.Public);

            if (mediaUrl == null)
                throw new InvalidOperationException($"Failed to find media file: '{mediaPath}'");

            return mediaUrl.Replace("_jpg", ".jpg").Replace("_mov", ".mov").Replace("_m4v", ".m4v").Replace("_swf", ".swf");
        }

        public static string GetUrlTargetValue(Guid? targetId)
        {
            if (targetId == null) return string.Empty;

            return _urlTargetCache[targetId.Value]?.Value ?? string.Empty;
        }

        public static string GetCssStyleValue(Guid? cssStyleId)
        {
            if (cssStyleId == null) return string.Empty;

            return _cssStyleCache[cssStyleId.Value]?.CssCode;
        }

        public static PageUrlData GetPageUrlDataFromUrl(string urlStr)
        {
            PageUrlData pageUrlData = null;
            while (pageUrlData == null && urlStr.LastIndexOf('/') > 0)
            {
                urlStr = urlStr.Substring(0, urlStr.LastIndexOf('/'));
                pageUrlData = PageUrls.ParseUrl(urlStr);
            }

            return pageUrlData;
        }

        public static Guid GetWebsiteIdFromPageUrlData(PageUrlData pageUrlData)
        {
            if (pageUrlData == null) return Guid.Empty;

            return PageStructureInfo.GetAssociatedPageIds(pageUrlData.PageId, SitemapScope.AncestorsAndCurrent).LastOrDefault();
        }
    }
}
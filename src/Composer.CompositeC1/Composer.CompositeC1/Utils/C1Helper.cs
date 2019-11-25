using System;
using System.Collections.Specialized;
using System.Linq;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes;

namespace Orckestra.Composer.CompositeC1.Utils
{
    public class C1Helper
    {
        public static bool IsUrlPagePublished(string url)
        {
            if (IsExternalLink(url)) return true;

            using (DataConnection data = new DataConnection())
            {
                return Queryable.Any(data.Get<IPage>(), x => x.Id.ToString() == GetPageGuidFromUrl(url) && x.PublicationStatus == "published");
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

            string[] parts = mediaPath.Split(new[] { ':' });

            string mediaStore = parts[0];
            Guid mediaId = new Guid(parts[1]);

            string mediaUrl = MediaUrls.BuildUrl(new MediaUrlData { MediaStore = mediaStore, MediaId = mediaId, QueryParameters = new NameValueCollection() },
                UrlKind.Public);
            
            return mediaUrl.Replace("_jpg", ".jpg").Replace("_mov", ".mov").Replace("_m4v", ".m4v").Replace("_swf", ".swf");
        }

        public static string GetUrlTargetValue(Guid? targetId)
        {
            if (targetId == null) return string.Empty;

            using (DataConnection data = new DataConnection())
            {
                var targetValue = data.Get<UrlTarget>().FirstOrDefault(x => x.Id == targetId);
                return targetValue != null ? targetValue.Value : string.Empty;
            }
        }

        public static string GetCssStyleValue(Guid? cssStyleId)
        {
            if (cssStyleId == null) return string.Empty;

            using (DataConnection data = new DataConnection())
            {
                var cssStyleValue = data.Get<CssStyle>().FirstOrDefault(x => x.Id == cssStyleId);
                return cssStyleValue != null ? cssStyleValue.CssCode : string.Empty;
            }
        }
    }
}
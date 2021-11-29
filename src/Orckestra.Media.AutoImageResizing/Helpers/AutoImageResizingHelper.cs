using Composite.Core;
using Composite.Core.Routing;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Media;
using Composite.Data;
using Composite.Data.Types;

namespace Orckestra.Media.AutoImageResizing.Helpers
{
    public static class AutoImageResizingHelper
    {
        private static readonly string MediaUrlUnprocessedInternalPrefix = UrlUtils.PublicRootPath + "~/media(";
        private static readonly string MediaUrlInternalPrefix = UrlUtils.PublicRootPath + "/media(";
        private static readonly string MediaUrlPublicPrefix = UrlUtils.PublicRootPath + "/media/";

        public static bool IsLocalC1MediaWithoutResizingOptions(string relativeUrl)
        {
            if (!relativeUrl.StartsWith(MediaUrlUnprocessedInternalPrefix)
                && !relativeUrl.StartsWith(MediaUrlInternalPrefix)
                && !relativeUrl.StartsWith(MediaUrlPublicPrefix))
            {
                return false;
            }

            var parsedUrl = new UrlBuilder(relativeUrl);
            var queryParameters = parsedUrl.GetQueryParameters();
            return ResizingOptions.Parse(queryParameters).IsEmpty;
        }

        public static string GetResizedImageUrl(DataReference<IImageFile> image, int? maxWidth = null, string mediaType = null)
        {
            var baseImageUrl = $"/media({image.KeyValue})";
            return GetResizedImageUrl(baseImageUrl, maxWidth, mediaType);
        }

        public static string GetResizedImageUrl(string baseImageUrl, int? maxWidth = null, string mediaType = null)
        {
            if (maxWidth != null)
            {
                baseImageUrl = AddQueryParameter(baseImageUrl, "mw=" + maxWidth.Value);
            }
            if (mediaType != null)
            {
                baseImageUrl = AddQueryParameter(baseImageUrl, "mt=" + mediaType);
            }
            return ConvertToPublicUrl(baseImageUrl);
        }

        private static string AddQueryParameter(string baseUrl, string param)
        {
            return baseUrl + (baseUrl.Contains("?") ? "&" : "?") + param;
        }

        private static string ConvertToPublicUrl(string imageUrl)
        {
            var mediaData = MediaUrls.ParseUrl(imageUrl, out _);
            if (mediaData != null)
            {
                return MediaUrls.BuildUrl(mediaData, UrlKind.Public) ?? imageUrl;
            }
            return imageUrl;
        }
    }
}

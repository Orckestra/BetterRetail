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
        public static bool IsLocalC1MediaWithoutResizingOptions(string relativeUrl)
        {
            var mediaUrlUnprocessedInternalPrefix = "~/media(";
            var mediaUrlInternalPrefix = UrlUtils.PublicRootPath + "/media(";
            var mediaUrlPublicPrefix = UrlUtils.PublicRootPath + "/media/";

            var parsedUrl = new UrlBuilder(relativeUrl);
            var queryParameters = parsedUrl.GetQueryParameters();
            var isEmptyResizingQueryParameters = ResizingOptions.Parse(queryParameters).IsEmpty;

            return (relativeUrl.Contains(mediaUrlUnprocessedInternalPrefix)
                    || relativeUrl.Contains(mediaUrlInternalPrefix)
                    || relativeUrl.Contains(mediaUrlPublicPrefix))
                   && isEmptyResizingQueryParameters;
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

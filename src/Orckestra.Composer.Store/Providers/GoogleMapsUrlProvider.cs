using Orckestra.Composer.Configuration;
using System.Web;

namespace Orckestra.Composer.Store.Providers
{
    public class GoogleMapsUrlProvider: IGoogleMapsUrlProvider
    {

        public IGoogleSettings GoogleMapsSettings { get; set; }
        public GoogleMapsUrlProvider(IGoogleSettings googleMapsSettings)
        {
            GoogleMapsSettings = googleMapsSettings;
        }

        public static string DirectionWithSetStartingPointUrlTemplate => "https://maps.google.com?saddr={0}&daddr={1}";
        public static string DirectionWithEmptyStartPointUrlTemplate => "https://maps.google.com?daddr={0}";
        public static string StaticMapImgUrlTemplate => "https://maps.googleapis.com/maps/api/staticmap?center={0}&maptype={1}&zoom=13";

        public static string GetDirectionWithSetStartingPointLink(string[] fromAddressParams, string[] toAddressParams)
        {
            return string.Format(DirectionWithSetStartingPointUrlTemplate,
                HttpUtility.UrlEncode(string.Join(" ", fromAddressParams)),
                HttpUtility.UrlEncode(string.Join(" ", toAddressParams)));
        }

        public static string GetDirectionWithEmptyStartPointLink(string[] toAddressParams)
        {
            return string.Format(DirectionWithEmptyStartPointUrlTemplate,
                HttpUtility.UrlEncode(string.Join(" ", toAddressParams)));
        }

        public string GetStaticMapImgUrl(string[] addressParams, string mapType = "roadmap")
        {
            var result = string.Format(StaticMapImgUrlTemplate, HttpUtility.UrlEncode(string.Join(",", addressParams)), mapType);
            if (!string.IsNullOrWhiteSpace(GoogleMapsSettings.MapsApiKey))
            {
                result = result + "&key=" + GoogleMapsSettings.MapsApiKey;
            }
            return result;
        }
    }
}

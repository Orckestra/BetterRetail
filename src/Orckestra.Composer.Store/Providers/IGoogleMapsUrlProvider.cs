using Orckestra.Composer.Configuration;

namespace Orckestra.Composer.Store.Providers
{
    public interface IGoogleMapsUrlProvider
    {
        IGoogleSettings GoogleMapsSettings { get; set; }
        string GetStaticMapImgUrl(string[] addressParams, string mapType = "roadmap");
    }
}
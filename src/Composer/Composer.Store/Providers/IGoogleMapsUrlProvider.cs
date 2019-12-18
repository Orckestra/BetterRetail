namespace Orckestra.Composer.Store.Providers
{
    public interface IGoogleMapsUrlProvider
    {
       string GetStaticMapImgUrl(string[] addressParams, string mapType = "roadmap");
    }
}
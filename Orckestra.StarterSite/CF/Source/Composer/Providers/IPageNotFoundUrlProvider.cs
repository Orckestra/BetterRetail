namespace Orckestra.Composer.Providers
{
    public interface IPageNotFoundUrlProvider
    {
        string Get404PageUrl(string requestedUrl);
    }
}

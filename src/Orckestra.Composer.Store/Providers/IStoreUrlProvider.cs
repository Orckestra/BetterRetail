using Orckestra.Composer.Store.Parameters;

namespace Orckestra.Composer.Store.Providers
{
    public interface IStoreUrlProvider
    {
        string GetStoreUrl(GetStoreUrlParam parameters);
        string GetStoreLocatorUrl(GetStoreLocatorUrlParam parameters);
        string GetStoresDirectoryUrl(GetStoresDirectoryUrlParam parameters);
    }
}
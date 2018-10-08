using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.ViewModels;


namespace Orckestra.Composer.Store.Factory
{
    public interface IStoreViewModelFactory
    {
        StoreViewModel CreateStoreViewModel(CreateStoreViewModelParam param);
        StoreStructuredDataViewModel CreateStoreStructuredDataViewModel(CreateStoreViewModelParam param);
        StorePageViewModel BuildNextPage(GetStorePageViewModelParam param);
    }
}

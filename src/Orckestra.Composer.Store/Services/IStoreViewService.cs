using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.Services
{
    public interface IStoreViewService
    {
        Task<StoreViewModel> GetStoreViewModelAsync(GetStoreByNumberParam viewModelParam);
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetStorePageHeaderViewModelParam param);
        Task<List<StoreViewModel>> GetStoresForInStorePickupViewModelAsync(GetStoresForInStorePickupViewModelParam param);
    }
}
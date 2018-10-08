using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Store.Services
{
    public interface IStoreLocatorViewService
    {
        /// <summary>
        /// Retrieve the Locations List ViewModel
        /// </summary>
        /// <returns></returns>
        Task<StoreLocatorViewModel> GetStoreLocatorViewModelAsync(GetStoreLocatorViewModelParam viewModelParam);

        StoreLocatorViewModel GetEmptyStoreLocatorViewModel(GetEmptyStoreLocatorViewModelParam viewModelParam);
    }
}
using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Store.Services
{
    public interface  IStoreDirectoryViewService
    {
        Task<StoreDirectoryViewModel> GetStoreDirectoryViewModelAsync(GetStoresParam viewModelParam);
    }
}

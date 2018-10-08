using System.Threading.Tasks;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.Store.Parameters;

namespace Orckestra.Composer.Store.Services
{
    public interface IStoreScheduleViewService
    {
        Task<StoreScheduleViewModel> GetStoreScheduleViewModelAsync(GetStoreScheduleParam param);
    }
}
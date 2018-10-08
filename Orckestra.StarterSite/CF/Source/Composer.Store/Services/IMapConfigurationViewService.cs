using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Store.Services
{
    public interface IMapConfigurationViewService
    {
        Task<MapConfigurationViewModel> GetMapConfigurationViewModelAsync(GetMapConfigurationViewModelParam param);
    }
}

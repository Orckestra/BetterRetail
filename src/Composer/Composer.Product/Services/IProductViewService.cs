using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.ViewModels;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Services
{
    public interface IProductViewService
    {
         Task<ProductViewModel> GetProductViewModelAsync(GetProductParam param);
    }
}
using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Services
{
    public interface IProductViewService
    {
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param);

        Task<ProductViewModel> GetProductViewModelAsync(GetProductParam param);
    }
}
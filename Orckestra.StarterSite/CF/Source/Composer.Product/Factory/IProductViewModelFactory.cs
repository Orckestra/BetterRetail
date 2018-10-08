using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Factory
{
    public interface IProductViewModelFactory
    {
        Task<ProductViewModel> GetProductViewModel(GetProductParam param);
    }
}
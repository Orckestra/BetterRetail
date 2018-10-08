using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Services
{
    public interface IRelatedProductViewService
    {
        Task<RelatedProductsViewModel> GetRelatedProductsAsync(GetRelatedProductsParam param);
        Task<RelatedProductsViewModel> GetProductIdsAsync(GetProductIdentifiersParam param);
    }
}
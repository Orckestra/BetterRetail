using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Product.Repositories
{
    public interface IRelationshipRepository
    {
        Task<ProductSearchResult> GetProductInSameCategoryAsync(GetProductsInSameCategoryParam getProductsInSameCategoryParam);
    }
}
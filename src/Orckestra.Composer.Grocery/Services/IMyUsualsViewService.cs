using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Parameters;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public interface IMyUsualsViewService
    {
        Task<string[]> GetMyUsualsProductSkusAsync(GetCustomerOrderedProductsParam param);
        Task<SearchBySkusCriteria> BuildProductsSearchCriteria(string[] listSkus, string QueryString, bool IncludeFactes = true);
    }
}

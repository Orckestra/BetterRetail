using Orckestra.Composer.Cart.Parameters.Order;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public interface IMyUsualsViewService
    {
        Task<string[]> GetMyUsualsProductSkusAsync(GetCustomerOrderedProductsParam param);
    }
}

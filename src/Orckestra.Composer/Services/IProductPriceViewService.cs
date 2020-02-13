using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public interface IProductPriceViewService
    {
        /// <summary>
        /// Gets the products prices view model asynchronous.
        /// </summary>
        /// <param name="getProductPriceParam">The get product price parameter.</param>
        /// <returns></returns>
        Task<ProductsPricesViewModel> CalculatePricesAsync(GetProductsPriceParam getProductPriceParam);
    }
}

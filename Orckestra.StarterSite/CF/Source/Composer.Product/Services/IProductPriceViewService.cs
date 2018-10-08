using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Services
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

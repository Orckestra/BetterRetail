using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Repositories
{
    public interface IProductSettingsRepository
    {
        /// <summary>
        /// Retrieve the Product Settings from Overture
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<ProductSettings> GetProductSettings(string scope);
    }
}
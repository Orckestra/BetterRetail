using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Repositories
{
    public interface IProductMediaSettingsRepository
    {
        /// <summary>
        /// Retrieve the Product Media Settings from Overture
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<MediaSettings> GetProductMediaSettings();
    }
}
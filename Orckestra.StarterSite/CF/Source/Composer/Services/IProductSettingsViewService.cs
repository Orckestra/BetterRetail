using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services
{
    public interface IProductSettingsViewService
    {
        /// <summary>
        /// Retrieve the Product Settings ViewModel
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        Task<ProductSettingsViewModel> GetProductSettings(string scope, CultureInfo cultureInfo);
    }
}
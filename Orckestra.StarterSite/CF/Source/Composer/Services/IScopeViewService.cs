using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services
{
    public interface IScopeViewService
    {
        /// <summary>
        /// Obtains the currency of the scope.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CurrencyViewModel> GetScopeCurrencyAsync(GetScopeCurrencyParam param);
    }
}

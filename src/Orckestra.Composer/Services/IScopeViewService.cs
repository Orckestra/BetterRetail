using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;
using System.Threading.Tasks;

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
        /// <summary>
        /// Get sale scope
        /// </summary>
        /// <param name="scope">Scope, from which to get a sale scope</param>
        /// <returns>Sale scope from dependent scope. If not exists, returns passed scope</returns>
        Task<string> GetSaleScopeAsync(string scope);
    }
}

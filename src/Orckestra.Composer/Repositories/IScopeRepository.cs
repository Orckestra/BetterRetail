using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Repositories
{
    public interface IScopeRepository
    {
        /// <summary>
        /// Obtains a scope description from Overture including the currency of the scope, but not its children.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Scope> GetScopeAsync(GetScopeParam param);


        /// <summary>
        /// Returns the Global scope, with all the descendant scopes populated.
        /// </summary>
        /// <returns>Global scope object with children elements</returns>
        Task<Scope> GetAllScopesAsync();


        /// <summary>
        /// Get sale scope
        /// </summary>
        /// <param name="scope">Scope, from which to get a sale scope</param>
        /// <returns>Sale scope from dependent scope. If not exists, returns passed scope</returns>
        Task<string> GetSaleScopeAsync(string scope);
    }
}

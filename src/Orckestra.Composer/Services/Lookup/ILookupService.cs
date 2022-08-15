using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services.Lookup
{
    /// <summary>
    /// Interface for retrieving Lookup values
    /// </summary>
    /// <remarks>
    /// This interface will grow to include helper methods over time,
    /// and using a specific interface allows easier dependency injection.
    /// </remarks>
    public interface ILookupService
    {
        /// <summary>
        /// Gets all the available product lookups.
        /// </summary>
        /// <remarks>Deprecated, to be removed once <see> <cref>ProductDetailViewService</cref></see>
        ///  uses <see cref="GetLookupDisplayNameAsync"/> and <see cref="LookupAttribute"/></remarks>
        /// <returns></returns>
        Task<List<Overture.ServiceModel.Metadata.Lookup>> GetLookupsAsync(LookupType lookupType);

        /// <summary>
        /// Gets lookup by specific lookup type and name.
        /// </summary>
        Task<Overture.ServiceModel.Metadata.Lookup> GetLookupAsync(LookupType lookupType, string LookupName);

        /// <summary>
        /// Gets a specific lookup value
        /// </summary>
        Task<string> GetLookupDisplayNameAsync(GetLookupDisplayNameParam param);


        /// <summary>
        /// Gets all values display name for a specific lookup.
        /// </summary>
        Task<Dictionary<string, string>> GetLookupDisplayNamesAsync(GetLookupDisplayNamesParam param);
    }
}

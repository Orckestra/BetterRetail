using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Repositories
{
    public interface ILookupRepository
    {
        /// <summary>
        /// Gets all the available product lookups.
        /// </summary>
        /// <returns></returns>
        //TODO: Delete
		Task<List<Lookup>> GetLookupsAsync();

        /// <summary>
        /// Gets a single <see cref="Lookup"/> value from Overture.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Lookup> GetLookupAsync(string name);
    }
}
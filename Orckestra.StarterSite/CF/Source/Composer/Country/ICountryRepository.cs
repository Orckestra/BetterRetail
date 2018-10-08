using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Country
{
    public interface ICountryRepository
    {
        /// <summary>
        /// Retrieve a country using its ISO code
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Country> RetrieveCountry(RetrieveCountryParam param);

        /// <summary>
        /// Retrieve a list of regions for a specified country using its ISO code
        /// </summary>
        /// <param name="param"></param>
        /// <returns>A list of Region</returns>
        Task<IEnumerable<Region>> RetrieveRegions(RetrieveCountryParam param);
    }
}
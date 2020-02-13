using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Country
{
    public interface ICountryService
    {
        /// <summary>
        /// Retrieve the CountryViewModel
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CountryViewModel> RetrieveCountryAsync(RetrieveCountryParam param);

        /// <summary>
        /// Retrieve the list of RegionViewModel for a specified Country
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<RegionViewModel>> RetrieveRegionsAsync(RetrieveCountryParam param);

        /// <summary>
        /// Retrieve the display name for a specified region.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<string> RetrieveRegionDisplayNameAsync(RetrieveRegionDisplayNameParam param);
        Task<string> RetrieveCountryDisplayNameAsync(RetrieveCountryParam param);
    }
}
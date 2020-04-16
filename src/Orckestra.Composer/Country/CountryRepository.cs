using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;

namespace Orckestra.Composer.Country
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IOvertureClient _overtureClient;
        private readonly ICacheProvider _cacheProvider;

        public CountryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            _overtureClient = overtureClient;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Retrieve a country using its ISO code
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Overture.ServiceModel.Country> RetrieveCountry(RetrieveCountryParam param)
        {
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("IsoCode"), "param"); }

            var countryCacheKey = new CacheKey(CacheConfigurationCategoryNames.Country);
            countryCacheKey.AppendKeyParts(param.IsoCode);

            var result = await _cacheProvider.GetOrAddAsync(countryCacheKey, () =>
            {
                var request = new GetCountryRequest
                {
                    CountryIsoCode = param.IsoCode,
                    IncludeRegions = true
                };

                return _overtureClient.SendAsync(request);
            }).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Retrieve a list of regions for a specified country using its ISO code
        /// </summary>
        /// <param name="param"></param>
        /// <returns>A list of Region</returns>
        public async Task<IEnumerable<Region>> RetrieveRegions(RetrieveCountryParam param)
        {
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("IsoCode"), "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }

            var regionsCacheKey = new CacheKey(CacheConfigurationCategoryNames.Regions)
            {
                CultureInfo = param.CultureInfo
            };
            regionsCacheKey.AppendKeyParts(param.IsoCode);

            var result = await _cacheProvider.GetOrAddAsync(regionsCacheKey, () =>
            {
                var request = new GetRegionsRequest
                {
                    CountryIsoCode = param.IsoCode,
                    CultureName = param.CultureInfo.Name,
                    IncludeUnsupported = false
                };

                return _overtureClient.SendAsync(request);
            }).ConfigureAwait(false);

            return result;
        }
    }
}

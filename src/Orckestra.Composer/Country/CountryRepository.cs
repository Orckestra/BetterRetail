using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Country
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IOvertureClient _overtureClient;
        private readonly ICacheProvider _cacheProvider;

        public CountryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            _overtureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Retrieve a country using its ISO code
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Overture.ServiceModel.Country> RetrieveCountry(RetrieveCountryParam param)
        {
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.IsoCode)), nameof(param)); }

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
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.IsoCode)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

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
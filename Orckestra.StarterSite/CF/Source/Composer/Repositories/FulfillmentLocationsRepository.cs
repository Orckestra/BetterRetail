using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;

namespace Orckestra.Composer.Repositories
{
    public class FulfillmentLocationsRepository: IFulfillmentLocationsRepository
    {
        public IOvertureClient OvertureClient { get; set; }
        public ICacheProvider CacheProvider { get; set; }

        public FulfillmentLocationsRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        public Task<List<FulfillmentLocation>> GetFulfillmentLocationsByScopeAsync(GetFulfillmentLocationsByScopeParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (String.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }

            var cacheKey = GetCacheKeyForFulfillmentLocationsByScope(param);

            var results = CacheProvider.GetOrAddAsync(cacheKey, () =>
            {
                var request = GetFulfillmentLocationsByScopeRequest(param);

                var r = OvertureClient.SendAsync(request);
                return r;
            });

            return results;
        }

        private static GetFulfillmentLocationsByScopeRequest GetFulfillmentLocationsByScopeRequest(
            GetFulfillmentLocationsByScopeParam param)
        {
            var request = new GetFulfillmentLocationsByScopeRequest
            {
                IncludeChildScopes = param.IncludeChildScopes,
                IncludeSchedules = param.IncludeSchedules,
                ScopeId = param.Scope
            };
            return request;
        }

        private static CacheKey GetCacheKeyForFulfillmentLocationsByScope(GetFulfillmentLocationsByScopeParam param)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.FulfillmentLocationsByScope, param.Scope);
            cacheKey.AppendKeyParts(param.IncludeChildScopes, param.IncludeSchedules);

            return cacheKey;
        }
    }
}

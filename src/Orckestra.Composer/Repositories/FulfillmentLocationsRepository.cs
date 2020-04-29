using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Repositories
{
    public class FulfillmentLocationsRepository: IFulfillmentLocationsRepository
    {
        public IOvertureClient OvertureClient { get; set; }
        public ICacheProvider CacheProvider { get; set; }

        public FulfillmentLocationsRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public Task<List<FulfillmentLocation>> GetFulfillmentLocationsByScopeAsync(GetFulfillmentLocationsByScopeParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

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

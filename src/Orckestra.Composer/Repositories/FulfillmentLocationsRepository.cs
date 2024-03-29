﻿using Orckestra.Composer.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Repositories
{
    public class FulfillmentLocationsRepository: IFulfillmentLocationsRepository
    {
        public IComposerOvertureClient OvertureClient { get; set; }
        public ICacheProvider CacheProvider { get; set; }

        public FulfillmentLocationsRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public virtual Task<FulfillmentLocation> GetFulfillmentLocationByIdAsync(Guid id, string scope)
        {
            if (string.IsNullOrWhiteSpace(scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(scope))); }
            if (id == null) { throw new ArgumentNullException(nameof(id)); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.FulfillmentLocationsByScope, id.ToString());

            var request = new GetFulfillmentLocationByIdRequest
            {
                FulfillmentLocationId = id,
                ScopeId = scope
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        public virtual Task<List<FulfillmentLocation>> GetFulfillmentLocationsByScopeAsync(GetFulfillmentLocationsByScopeParam param)
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

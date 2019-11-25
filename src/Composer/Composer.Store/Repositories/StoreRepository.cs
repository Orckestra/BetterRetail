using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Overture.ServiceModel.Customers.Stores;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Customers.CustomProfiles;
using Orckestra.Overture.ServiceModel.Requests.Customers.Stores;

namespace Orckestra.Composer.Store.Repositories
{
    public class StoreRepository : IStoreRepository
    {

        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public const string GETSTORES_CACHE_KEYPART = "allstores";
        public const string GETEXSTRASTORESINFO_CACHE_KEYPART = "storesextrainfo";
        public const string GETSTOREBYNUMBER_CACHE_KEYPART = "storebynumber";

        public static readonly string StoreTypePropertyName = ((MemberExpression)((Expression<Func<Overture.ServiceModel.Customers.Stores.Store, StoreType>>)(s => s.StoreType)).Body).Member.Name;
        public static readonly string StoreActivePropertyName = ((MemberExpression)((Expression<Func<Overture.ServiceModel.Customers.Stores.Store, bool>>)(s => s.IsActive)).Body).Member.Name;

        public StoreRepository(
            IOvertureClient overtureClient,
            ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        public virtual async Task<FindStoresQueryResult> GetStoresAsync(GetStoresParam getStoresParam)
        {
            if (string.IsNullOrWhiteSpace(getStoresParam.Scope)) { throw new ArgumentException("scope"); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Store)
            {
                Scope = getStoresParam.Scope
            };
            cacheKey.AppendKeyParts(GETSTORES_CACHE_KEYPART);

            var request = new FindStoresRequest
            {
                ScopeId = getStoresParam.Scope,
                Query = new Query
                {
                    StartingIndex = 0,
                    MaximumItems = int.MaxValue,
                    IncludeTotalCount = false,
                    Filter = new FilterGroup
                    {
                        Filters = GetStoreFilters()
                    }
                }
            };

            var stores = await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request))
                    .ConfigureAwait(false);

            // TODO: Remove this as soon as the FindStoresRequest returns localized display names.
            if (stores.Results.Any(s => s.DisplayName != null))
            {
                return stores;
            }
            // Try get DisplayNames
            if (getStoresParam.IncludeExtraInfo)
            {
                var ids = stores.Results.Select(x => x.Id).ToList();
                var extraStoresInfo = await GetExtraStoresInfoAsync(ids, getStoresParam).ConfigureAwait(false);

                for (var index = 0; index < stores.Results.Count; index++)
                {
                    var store = stores.Results[index];
                    var extraInfo = extraStoresInfo[index];
                    object displayName = null;
                    extraInfo?.PropertyBag.TryGetValue("DisplayName", out displayName);
                    if (displayName != null)
                    {
                        store.DisplayName = displayName as Overture.ServiceModel.LocalizedString;
                    }
                }
            }

            return stores;
        }

        protected virtual async Task<List<CustomProfile>> GetExtraStoresInfoAsync(List<Guid> storesIds, GetStoresParam getStoresParam)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Store)
            {
                Scope = getStoresParam.Scope
            };
            cacheKey.AppendKeyParts(GETEXSTRASTORESINFO_CACHE_KEYPART);

            var request = new GetProfileInstancesRequest
                {
                    Ids = storesIds,
                    EntityTypeName = "Store",
                    ScopeId = getStoresParam.Scope
                };

            return await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        public virtual async Task<Overture.ServiceModel.Customers.Stores.Store> GetStoreByNumberAsync(GetStoreParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("scope"); }
            if (string.IsNullOrWhiteSpace(param.StoreNumber)) { throw new ArgumentException("storeNumber"); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Store)
            {
                Scope = param.Scope
            };
            cacheKey.AppendKeyParts(GETSTOREBYNUMBER_CACHE_KEYPART, param.StoreNumber);

            var request = new GetStoreByNumberRequest()
            {
                ScopeId = param.Scope,
                Number = param.StoreNumber,
                IncludeAddresses = param.IncludeAddresses,
                IncludeSchedules = param.IncludeSchedules
            };

            return await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        public virtual async Task<FulfillmentSchedule> GetStoreScheduleAsync(GetStoreScheduleParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("scope"); }
            if (param.FulfillmentLocationId == null) { throw new ArgumentException("fulfillmentLocationId"); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.StoreSchedule)
            {
                Scope = param.Scope
            };
            cacheKey.AppendKeyParts(param.FulfillmentLocationId.ToString());

            var request = new GetScheduleRequest
            {
                ScopeId = param.Scope,
                FulfillmentLocationId = param.FulfillmentLocationId,
                ScheduleType = ScheduleType.OpeningHours
            };

            return
                await
                    CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        protected virtual List<Filter> GetStoreFilters()
        {
            var physicalStoreFilter = new Filter
            {
                Member = StoreTypePropertyName,
                Operator = Operator.Equals,
                Value = StoreType.Physical.ToString()
            };

            var activeStoreFilter = new Filter
            {
                Member = StoreActivePropertyName,
                Operator = Operator.Equals,
                Value = true
            };

            return new List<Filter> { physicalStoreFilter, activeStoreFilter };
        }
    }
}

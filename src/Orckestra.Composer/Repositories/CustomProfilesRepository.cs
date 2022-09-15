using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers.CustomProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orckestra.Composer.Caching;
using Orckestra.Composer.Configuration;

namespace Orckestra.Composer.Repositories
{
    public class CustomProfilesRepository : ICustomProfilesRepository
    {
        protected IComposerOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public CustomProfilesRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public virtual async Task<List<CustomProfile>> GetProfileInstances(GetCustomProfilesParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            if (!param.CustomProfileIds.Any()) throw new ArgumentNullException(nameof(param.CustomProfileIds));

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.CustomProfiles)
            {
                Scope = param.Scope,
                Key = JsonConvert.SerializeObject(param.CustomProfileIds)
            };
            var result = await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(new GetProfileInstancesRequest()
            {
                EntityTypeName = param.EntityTypeName,
                Ids = param.CustomProfileIds,
                ScopeId = param.Scope
            })).ConfigureAwait(false);

            return result;
        }

        public virtual Task BulkUpdateProfiles(BulkUpdateProfilesRequest request)
        {
            return OvertureClient.SendAsync(request);
        }
    }
}

using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Requests.Metadata;

namespace Orckestra.Composer.CustomerConditionProvider.Repositories
{
    public class CustomerDefinitionsRepository : ICustomerDefinitionsRepository
    {
        protected IComposerOvertureClient OvertureClient { get; }
        protected ICacheProvider CacheProvider { get; }

        public CustomerDefinitionsRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public virtual async Task<EntityDefinition> GetCustomerDefinitionAsync()
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.ProductSettings, nameof(CustomerDefinitionsRepository));

            return await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(new GetCustomerDefinitionRequest()
            {
                Name = "CUSTOMER"
            })).ConfigureAwait(false);
        }
    }
}

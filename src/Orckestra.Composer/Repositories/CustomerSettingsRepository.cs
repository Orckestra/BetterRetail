using Orckestra.Composer.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using System;
using System.Threading.Tasks;
using ProfileSettings = Orckestra.Overture.ServiceModel.Customers.ProfileSettings;

namespace Orckestra.Composer.Repositories
{
    public class CustomerSettingsRepository : ICustomerSettingsRepository
    {
        protected IComposerOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public CustomerSettingsRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Retrieve the Customer Profile Settings from Overture
        /// </summary>
        /// <returns><see cref="ProfileSettings"/></returns>
        public virtual async Task<ProfileSettings> GetCustomerSettings()
        {
            var settingsCacheKey = new CacheKey(CacheConfigurationCategoryNames.CustomerSettings);

            var result = await CacheProvider.GetOrAddAsync(settingsCacheKey, () =>
            {
                return OvertureClient.SendAsync(new GetProfileSettingsRequest());
            }).ConfigureAwait(false);

            return result;
        }
    }
}

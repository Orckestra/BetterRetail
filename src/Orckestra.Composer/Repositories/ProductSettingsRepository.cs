using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Repositories
{
    public class ProductSettingsRepository : IProductSettingsRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public ProductSettingsRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Retrieve the Product Settings from Overture
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual async Task<ProductSettings> GetProductSettings(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(scope)); }

            var productSettingsCacheKey = new CacheKey(CacheConfigurationCategoryNames.ProductSettings);
            productSettingsCacheKey.AppendKeyParts(scope);

            var result = await CacheProvider.GetOrAddAsync(productSettingsCacheKey, () =>
            {
                var request = new GetProductSettingsRequest
                {
                    ScopeId = scope
                };

                return OvertureClient.SendAsync(request);
            }).ConfigureAwait(false);

            return result;
        }
    }
}

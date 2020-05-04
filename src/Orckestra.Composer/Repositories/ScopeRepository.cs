using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Repositories
{
    public class ScopeRepository: IScopeRepository
    {
        protected IOvertureClient OvertureClient { get; }

        protected ICacheProvider CacheProvider { get; }

        public ScopeRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Obtains a scope description from Overture including the currency of the scope, but not its children.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task<Scope> GetScopeAsync(GetScopeParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var key = new CacheKey(CacheConfigurationCategoryNames.Scopes, param.Scope);

            var scope = CacheProvider.GetOrAddAsync(key, async () =>
            {
                var req = new GetScopeRequest
                {
                    ScopeId = param.Scope,
                    IncludeCurrency = true,
                    IncludeChildren = false,
                    CultureName = null
                };

                var response = await OvertureClient.SendAsync(req).ConfigureAwait(false);
                return response;
            });

            return scope;
        }
    }
}
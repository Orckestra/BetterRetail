using System;
using System.Collections.Generic;
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

        public virtual Task<Scope> GetAllScopesAsync()
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.Scopes, nameof(GetAllScopesAsync));

            return CacheProvider.GetOrAddAsync(key, () =>
            {
                var getScopeRequest = new GetScopeRequest
                {
                    ScopeId = "Global",
                    IncludeCurrency = false,
                    IncludeChildren = true,
                    CultureName = null
                };

                return OvertureClient.SendAsync(getScopeRequest);
            });
        }

        public virtual async Task<string> GetSaleScopeAsync(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope)) throw new System.ArgumentException(GetMessageOfNullWhiteSpace(nameof(scope)));

            var dependentScopes = await GetDependentScopesWithParents().ConfigureAwait(false);
            return dependentScopes.ContainsKey(scope) ? dependentScopes[scope] : scope;
        }

        protected virtual async Task<Dictionary<string, string>> GetDependentScopesWithParents()
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.Scopes, nameof(GetDependentScopesWithParents));

            var dependentsScopes = await CacheProvider.GetOrAddAsync(key, async () =>
            {
                var result = new Dictionary<string, string>();
                var scope = await GetAllScopesAsync().ConfigureAwait(false);
                foreach (var saleScope in GetSaleScopes(scope))
                {
                    foreach (var dependentScope in GetDependantsScopes(saleScope))
                    {
                        result[dependentScope.Id] = saleScope.Id;
                    }
                }

                return result;
            }).ConfigureAwait(false);
            return dependentsScopes;
        }

        private IEnumerable<Scope> GetSaleScopes(Scope scope)
        {
            if (scope == null)
                yield break;
            foreach (var child in scope.Children)
            {
                switch (child.Type)
                {
                    case ScopeType.Sale:
                        yield return child;
                        break;
                    case ScopeType.Virtual:
                        {
                            foreach (var virtualChild in GetSaleScopes(child))
                            {
                                yield return virtualChild;
                            }

                            break;
                        }
                }
            }
        }

        private IEnumerable<Scope> GetDependantsScopes(Scope scope)
        {
            if (scope == null)
                yield break;
            foreach (var child in scope.Children)
            {
                switch (child.Type)
                {
                    case ScopeType.Dependant:
                        yield return child;
                        break;
                    case ScopeType.Virtual:
                        {
                            foreach (var virtualChild in GetDependantsScopes(child))
                            {
                                yield return virtualChild;
                            }

                            break;
                        }
                }
            }
        }
    }
}
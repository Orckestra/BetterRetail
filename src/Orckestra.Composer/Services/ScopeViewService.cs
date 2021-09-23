using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Services
{
    public class ScopeViewService: IScopeViewService
    {
        protected IScopeRepository ScopeRepository { get; }

        protected IViewModelMapper ViewModelMapper { get; }

        protected ICacheProvider CacheProvider { get; }

        protected IComposerContext ComposerContext { get; }

        public ScopeViewService(IScopeRepository scopeRepository,
            IViewModelMapper viewModelMapper,
            ICacheProvider cacheProvider,
            IComposerContext composerContext)
        {
            ScopeRepository = scopeRepository;
            ViewModelMapper = viewModelMapper;
            CacheProvider = cacheProvider;
            ComposerContext = composerContext;
        }

        public virtual async Task<CurrencyViewModel> GetScopeCurrencyAsync(GetScopeCurrencyParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var p = new GetScopeParam
            {
                Scope = param.Scope
            };

            var scope = await ScopeRepository.GetScopeAsync(p).ConfigureAwait(false);

            CurrencyViewModel vm = null;
            if (scope?.Currency != null)
            {
                vm = ViewModelMapper.MapTo<CurrencyViewModel>(scope.Currency, param.CultureInfo, ComposerContext.CurrencyIso);
            }

            return vm;
        }

        public async Task<Scope> GetScopeAsync(string scopeId)
        {
            var p = new GetScopeParam
            {
                Scope = scopeId
            };
            var scope = await ScopeRepository.GetScopeAsync(p).ConfigureAwait(false);
            return scope;
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
                var scope = await ScopeRepository.GetAllScopesAsync().ConfigureAwait(false);
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

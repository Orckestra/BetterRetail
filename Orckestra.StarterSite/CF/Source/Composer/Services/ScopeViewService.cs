using System;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services
{
    public class ScopeViewService: IScopeViewService
    {
        protected IScopeRepository ScopeRepository { get; }

        protected IViewModelMapper ViewModelMapper { get; }

        public ScopeViewService(IScopeRepository scopeRepository, IViewModelMapper viewModelMapper)
        {
            ScopeRepository = scopeRepository;
            ViewModelMapper = viewModelMapper;
        }

        public virtual async Task<CurrencyViewModel> GetScopeCurrencyAsync(GetScopeCurrencyParam param)
        {
            if(param == null) { throw new ArgumentNullException(nameof(param)); }
            if(string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("Scope is a required field", nameof(param.Scope)); }
            if(param.CultureInfo == null) { throw new ArgumentException("CultureInfo is a required field.", nameof(param.CultureInfo)); }

            var p = new GetScopeParam
            {
                Scope = param.Scope
            };

            var scope = await ScopeRepository.GetScopeAsync(p).ConfigureAwait(false);

            CurrencyViewModel vm = null;
            if (scope?.Currency != null)
            {
                vm = ViewModelMapper.MapTo<CurrencyViewModel>(scope.Currency, param.CultureInfo);
            }

            return vm;
        }
    }
}

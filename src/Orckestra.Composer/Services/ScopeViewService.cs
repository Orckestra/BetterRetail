﻿using Orckestra.Composer.Caching;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using System;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Services
{
    public class ScopeViewService: IScopeViewService
    {
        protected IScopeRepository ScopeRepository { get; }

        protected IViewModelMapper ViewModelMapper { get; }

        protected ICacheProvider CacheProvider { get; }

        public ScopeViewService(IScopeRepository scopeRepository, IViewModelMapper viewModelMapper, ICacheProvider cacheProvider)
        {
            ScopeRepository = scopeRepository;
            ViewModelMapper = viewModelMapper;
            CacheProvider = cacheProvider;
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
                vm = ViewModelMapper.MapTo<CurrencyViewModel>(scope.Currency, param.CultureInfo);
            }

            return vm;
        }
    }
}

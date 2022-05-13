using Composite.Data;
using Orckestra.Composer.CompositeC1.Providers.Breadcrumb;
using Orckestra.Composer.CompositeC1.Providers.LanguageSwitch;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.Composer.ViewModels.LanguageSwitch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class LanguageSwitchContext : ILanguageSwitchContext
    {
        private readonly Lazy<LanguageSwitchViewModel> _viewModel;
        public virtual LanguageSwitchViewModel ViewModel => _viewModel.Value;

        protected readonly IEnumerable<ILanguageSwitchProvider> Providers;

        public LanguageSwitchContext(IEnumerable<ILanguageSwitchProvider> providers)
        {
            _viewModel = new Lazy<LanguageSwitchViewModel>(() => GeViewModel(), true);
            Providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        private LanguageSwitchViewModel GeViewModel()
        {
            var provider = Providers.FirstOrDefault(e => e.isActiveForCurrentPage(SitemapNavigator.CurrentPageId, SitemapNavigator.CurrentHomePageId));
            if (provider == null)
            {
                throw new ArgumentNullException("There are no LanguageSwitch providers registered");
            }

            return provider.GetViewModel(SitemapNavigator.CurrentPageId, SitemapNavigator.CurrentHomePageId);
        }
    }
}

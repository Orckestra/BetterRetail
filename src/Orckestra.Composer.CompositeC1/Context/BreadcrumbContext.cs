using Composite.Data;
using Orckestra.Composer.CompositeC1.Providers.Breadcrumb;
using Orckestra.Composer.ViewModels.Breadcrumb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class BreadcrumbContext: IBreadcrumbContext
    {
        private readonly Lazy<BreadcrumbViewModel> _breadcrumbViewModel;
        public virtual BreadcrumbViewModel ViewModel => _breadcrumbViewModel.Value;

        protected readonly IEnumerable<IBreadcrumbProvider> BreadcrumbProviders;

        public BreadcrumbContext(IEnumerable<IBreadcrumbProvider> breadcrumbProviders)
        {
            _breadcrumbViewModel = new Lazy<BreadcrumbViewModel>(() => GetBreadcrumbViewModel(), true);
            BreadcrumbProviders = breadcrumbProviders ?? throw new ArgumentNullException(nameof(breadcrumbProviders));
        }

        private BreadcrumbViewModel GetBreadcrumbViewModel()
        {
            var provider = BreadcrumbProviders.FirstOrDefault(e => e.isActiveForCurrentPage(SitemapNavigator.CurrentPageId, SitemapNavigator.CurrentHomePageId));
            if (provider == null)
            {
                throw new ArgumentNullException("There are no Breadcrumb providers registered");
            }

            return provider.GetViewModel(SitemapNavigator.CurrentPageId, SitemapNavigator.CurrentHomePageId);
        }
    }
}

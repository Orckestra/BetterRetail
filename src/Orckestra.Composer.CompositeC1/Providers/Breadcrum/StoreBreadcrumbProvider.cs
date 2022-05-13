using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public class StoreBreadcrumbProvider : IBreadcrumbProvider
    {
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IStoreContext StoreContext { get; private set; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public StoreBreadcrumbProvider(IBreadcrumbViewService breadcrumbViewService, 
            IComposerContext composerContext,
            IStoreContext storeContext,
             ISiteConfiguration siteConfiguration)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            StoreContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.StorePageId == currentPageId;
        }

        public BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var breadcrumbViewModel = BreadcrumbViewService.CreateBreadcrumbViewModel(new GetBreadcrumbParam
            {
                CurrentPageId = currentPageId.ToString(),
                CultureInfo = ComposerContext.CultureInfo
            });

            if (!string.IsNullOrEmpty(StoreContext.ViewModel.LocalizedDisplayName))
            {
                breadcrumbViewModel.ActivePageName = HttpUtility.HtmlEncode(StoreContext.ViewModel.LocalizedDisplayName);
            }

            return breadcrumbViewModel;
        }
    }
}

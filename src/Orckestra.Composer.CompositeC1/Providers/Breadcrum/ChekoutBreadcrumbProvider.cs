using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public class ChekoutBreadcrumbProvider : IBreadcrumbProvider
    {
        protected ICheckoutBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IPageService PageService { get; private set; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public ChekoutBreadcrumbProvider(ICheckoutBreadcrumbViewService breadcrumbViewService,
            IComposerContext composerContext,
            IPageService pageService,
             ISiteConfiguration siteConfiguration)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.CheckoutConfirmationPageId == currentPageId;
        }

        public BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var breadcrumbViewModel = BreadcrumbViewService.CreateBreadcrumbViewModel(new GetCheckoutBreadcrumbParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                HomeUrl = PageService.GetRendererPageUrl(currentHomePageId, ComposerContext.CultureInfo),
            });

            return breadcrumbViewModel;
        }
    }
}

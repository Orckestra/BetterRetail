using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public class SearchBreadcrumbProvider : IBreadcrumbProvider
    {
        protected ISearchBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IPageService PageService { get; private set; }
        public ISiteConfiguration SiteConfiguration { get; set; }
        protected HttpRequestBase Request { get; }

        public SearchBreadcrumbProvider(ISearchBreadcrumbViewService breadcrumbViewService,
            IComposerContext composerContext,
             HttpRequestBase request,
            IPageService pageService,
             ISiteConfiguration siteConfiguration)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.SearchPageId == currentPageId;
        }

        public BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var breadcrumbViewModel = BreadcrumbViewService.CreateBreadcrumbViewModel(new GetSearchBreadcrumbParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                HomeUrl = PageService.GetRendererPageUrl(currentHomePageId, ComposerContext.CultureInfo),
                Keywords = Request.QueryString["keywords"]
            });

            return breadcrumbViewModel;
        }
    }
}

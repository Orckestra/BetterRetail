using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public class ProductBreadcrumbProvider : IBreadcrumbProvider
    {
        protected IProductBreadcrumbService BreadcrumbService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IProductContext ProductContext { get; }
        protected IPageService PageService { get; private set; }
        protected HttpRequestBase Request { get; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public ProductBreadcrumbProvider(IProductBreadcrumbService breadcrumbService,
            IComposerContext composerContext,
            IProductContext productContext,
            IPageService pageService,
            HttpRequestBase request,
            ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            BreadcrumbService = breadcrumbService ?? throw new ArgumentNullException(nameof(breadcrumbService));
            ProductContext = productContext ?? throw new ArgumentNullException(nameof(productContext));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.ProductPageId == currentPageId;
        }

        public BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var parameters = new GetProductBreadcrumbParam
            {
                CategoryId = ProductContext.ViewModel.CategoryId,
                CultureInfo = ComposerContext.CultureInfo,
                HomeUrl = PageService.GetRendererPageUrl(currentHomePageId, ComposerContext.CultureInfo),
                ProductName = ProductContext.ViewModel.DisplayName,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            };

            return BreadcrumbService.CreateBreadcrumbAsync(parameters).Result;
        }
    }
}

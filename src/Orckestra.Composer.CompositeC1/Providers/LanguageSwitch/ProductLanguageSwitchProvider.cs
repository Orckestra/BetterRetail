using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.LanguageSwitch;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Globalization;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.LanguageSwitch
{
    public class ProductLanguageSwitchProvider : ILanguageSwitchProvider
    {
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IProductContext ProductContext { get; }
        protected IProductUrlProvider ProductUrlProvider { get; }
        protected IPageService PageService { get; private set; }
        protected HttpRequestBase Request { get; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public ProductLanguageSwitchProvider(ILanguageSwitchService languageSwitchViewService,
            IComposerContext composerContext,
            IProductContext productContext,
            IProductUrlProvider productUrlProvider,
            IPageService pageService,
            HttpRequestBase request,
            ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = languageSwitchViewService ?? throw new ArgumentNullException(nameof(languageSwitchViewService));
            ProductContext = productContext ?? throw new ArgumentNullException(nameof(productContext));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.ProductPageId == currentPageId;
        }

        public LanguageSwitchViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(cultureInfo => BuildUrl(
              cultureInfo,
              ProductContext.ViewModel.LocalizedDisplayNames[cultureInfo.Name],
              ProductContext.ViewModel.ProductId,
              ProductContext.ViewModel.SelectedVariantId, ProductContext.ViewModel.Sku),
              ComposerContext.CultureInfo);

            return languageSwitchViewModel;
        }

        private string BuildUrl(CultureInfo cultureInfo, string productName, string productId, string variantId, string sku)
        {
            var productUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = cultureInfo,
                ProductId = productId,
                ProductName = productName,
                VariantId = variantId,
                SKU = sku
            });

            return productUrl;
        }
    }
}

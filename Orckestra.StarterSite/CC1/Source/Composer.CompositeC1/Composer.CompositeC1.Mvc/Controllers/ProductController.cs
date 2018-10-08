using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class ProductController : ProductBaseController
    {
        protected override string PreviewModeProductId => "3834593";

        public ProductController(
            IPageService pageService, 
            IComposerContext composerContext, 
            IProductViewService productService, 
            IProductSpecificationsViewService productSpecificationsViewService, 
            IProductBreadcrumbService productBreadcrumbService, 
            ILanguageSwitchService languageSwitchService, 
            IProductUrlProvider productUrlProvider, 
            IRelatedProductViewService relatedProductViewService) 
            
            : base(
            pageService, 
            composerContext, 
            productService, 
            productSpecificationsViewService, 
            productBreadcrumbService, 
            languageSwitchService, 
            productUrlProvider, 
            relatedProductViewService)
        {
        }
    }
}
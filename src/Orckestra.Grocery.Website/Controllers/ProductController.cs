using System;
using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Grocery.Website.Controllers
{
    public class ProductController : ProductBaseController
    {
        public ProductController(
            IPageService pageService, 
            IComposerContext composerContext, 
            IProductBreadcrumbService productBreadcrumbService, 
            ILanguageSwitchService languageSwitchService, 
            IProductUrlProvider productUrlProvider, 
            IRelatedProductViewService relatedProductViewService,
            Lazy<IPreviewModeService> previewModeService,
            IProductContext productContext) 
            
            : base(
            pageService, 
            composerContext, 
            productBreadcrumbService, 
            languageSwitchService, 
            productUrlProvider, 
            relatedProductViewService,
            previewModeService,
            productContext)
        {
        }
    }
}
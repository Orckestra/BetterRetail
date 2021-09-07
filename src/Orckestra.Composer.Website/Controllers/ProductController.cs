using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using System;

namespace Orckestra.Composer.Website.Controllers
{
    public class ProductController : ProductBaseController
    {
        public ProductController(
            IComposerContext composerContext,
            ILanguageSwitchService languageSwitchService, 
            IProductUrlProvider productUrlProvider, 
            IRelatedProductViewService relatedProductViewService,
            Lazy<IPreviewModeService> previewModeService,
            IProductContext productContext) 
            
            : base(
            composerContext,
            languageSwitchService, 
            productUrlProvider, 
            relatedProductViewService,
            previewModeService,
            productContext)
        {
        }
    }
}
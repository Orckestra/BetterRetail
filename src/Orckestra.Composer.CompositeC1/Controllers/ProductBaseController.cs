using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using System;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class ProductBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IProductUrlProvider ProductUrlProvider { get; private set; }
        protected IRelatedProductViewService RelatedProductViewService { get; private set; }
        protected Lazy<IPreviewModeService> PreviewModeService { get; }
        protected IProductContext ProductContext { get; private set; }

        protected ProductBaseController(
            IComposerContext composerContext,
            ILanguageSwitchService languageSwitchService,
            IProductUrlProvider productUrlProvider,
            IRelatedProductViewService relatedProductViewService,
            Lazy<IPreviewModeService> previewModeService,
            IProductContext productContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = languageSwitchService ?? throw new ArgumentNullException(nameof(languageSwitchService));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            RelatedProductViewService = relatedProductViewService ?? throw new ArgumentNullException(nameof(relatedProductViewService));
            PreviewModeService = previewModeService ?? throw new ArgumentNullException(nameof(previewModeService));
            ProductContext = productContext ?? throw new ArgumentNullException(nameof(productContext)); ;
        }
    }
}

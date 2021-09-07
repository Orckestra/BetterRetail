using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Extensions;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

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

        public virtual ActionResult LanguageSwitch()
        {
            var productViewModel = ProductContext.ViewModel;

            if (productViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(cultureInfo => BuildUrl(
                cultureInfo,
                productViewModel.LocalizedDisplayNames[cultureInfo.Name],
                productViewModel.ProductId,
                productViewModel.SelectedVariantId, productViewModel.Sku),
                ComposerContext.CultureInfo);

            return View("LanguageSwitch", languageSwitchViewModel);
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

        public virtual ActionResult RelatedProducts(string id, string merchandiseTypes, string headingText, int maxItems, bool displaySameCategoryProducts, bool displayPrices, bool displayAddToCart, DataReference<CssStyle> backgroundStyle = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.HandlePreviewMode( () =>
                            RelatedProducts(PreviewModeService.Value.GetProductId(), merchandiseTypes, headingText, maxItems,
                                displaySameCategoryProducts, displayPrices, displayAddToCart));
            }
            
            if (string.IsNullOrWhiteSpace(merchandiseTypes))
            {
                throw new HttpException(400, "merchandiseTypes parameter is required.");
            }

            var cssStyle = backgroundStyle?.Data?.CssCode;

            var relatedProductsViewModel = CreateRelatedProductsViewModel(id, 
                merchandiseTypes, 
                headingText, 
                maxItems, 
                displaySameCategoryProducts, 
                displayPrices, 
                displayAddToCart,
                cssStyle);

            return View("RelatedProductsBlade", relatedProductsViewModel);
        }

        private RelatedProductsViewModel CreateRelatedProductsViewModel(string id, string merchandiseTypes,
            string headingText, int maxItems, bool displaySameCategoryProducts, bool displayPrices,
            bool displayAddToCart, string backgroundStyle)
        {
            var param = new GetProductIdentifiersParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                ProductId = id,
                Scope = ComposerContext.Scope,
                MerchandiseTypes = merchandiseTypes.Split(','),
                FallbackOnSameCategoriesProduct = displaySameCategoryProducts,
                MaxItems = maxItems
            };

            var relatedProductsViewModel = RelatedProductViewService.GetProductIdsAsync(param).Result;
            relatedProductsViewModel.Context["DisplayAddToCart"] = displayAddToCart;
            relatedProductsViewModel.Context["DisplayPrices"] = displayPrices;
            relatedProductsViewModel.Context["HeadingComponentText"] = headingText;

            if (!string.IsNullOrWhiteSpace(backgroundStyle))
            {
                relatedProductsViewModel.Context["BackgroundStyle"] = backgroundStyle;
            }

            return relatedProductsViewModel; ;
        }
    }
}

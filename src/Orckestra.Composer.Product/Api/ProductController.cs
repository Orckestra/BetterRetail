using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Requests;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Product.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class ProductController : ApiController
    {
        protected IRelatedProductViewService RelatedProductViewService { get; private set; }
        protected IProductPriceViewService ProductPriceViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IProductViewService ProductViewService { get; private set; }

        public ProductController(
            IProductPriceViewService productPriceViewService, 
            IComposerContext composerContext,
            IProductViewService productViewService,
            IRelatedProductViewService relatedProductViewService)
        {
            ProductPriceViewService = productPriceViewService ?? throw new ArgumentNullException(nameof(productPriceViewService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ProductViewService = productViewService ?? throw new ArgumentNullException(nameof(productViewService));
            RelatedProductViewService = relatedProductViewService ?? throw new ArgumentNullException(nameof(relatedProductViewService));
        }

        [ActionName("calculatePrices")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CalculatePrices(CalculatePricesRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var vm = await ProductPriceViewService.CalculatePricesAsync(
                new GetProductsPriceParam
                {
                    ProductIds = request.Products.ToList(),
                    CultureInfo = ComposerContext.CultureInfo,
                    Scope = ComposerContext.Scope,
                    CurrencyIso = ComposerContext.CurrencyIso
                }
            );

            return Ok(vm);
        }

        [ActionName("variantSelection")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> VariantSelection(GetProductRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var vm = await ProductViewService.GetProductViewModelAsync(new GetProductParam
                {
                    ProductId = request.ProductId,
                    VariantId = request.VariantId,
                    CultureInfo = ComposerContext.CultureInfo,
                    Scope = ComposerContext.Scope,
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
                }
            );

            //Some additionnal Context Required by JS - used by Product Quick View on search page
            vm.Context["Images"] = vm.Images;
            vm.Context["SelectedImage"] = vm.SelectedImage;
            vm.Context["FallbackImageUrl"] = vm.FallbackImageUrl;
            vm.Context["allVariants"] = vm.Variants;
            return Ok(vm);
        }

        [ActionName("relatedProducts")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> RelatedProducts(IEnumerable<ProductIdentifier> relatedProductIdentifiers)  //TODO: Make a ViewModel for this.
        {
            var param = new GetRelatedProductsParam
            {
                ProductIds = relatedProductIdentifiers,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request),
                CurrencyIso = ComposerContext.CurrencyIso
            };

            var vm = await RelatedProductViewService.GetRelatedProductsAsync(param);

            return Ok(vm);
        }
    }
}
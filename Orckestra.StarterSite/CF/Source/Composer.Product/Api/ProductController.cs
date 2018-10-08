using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
        protected IProductSpecificationsViewService ProductSpecificationsViewService { get; private set; }

        public ProductController(
            IProductPriceViewService productPriceViewService, 
            IComposerContext composerContext,
            IProductViewService productViewService,
            IProductSpecificationsViewService productSpecificationsViewService,
            IRelatedProductViewService relatedProductViewService)
        {
            if (productPriceViewService == null) { throw new ArgumentNullException("productPriceViewService"); }
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (productViewService == null) { throw new ArgumentNullException("productViewService"); }
            if (productSpecificationsViewService == null) { throw new ArgumentNullException("productSpecificationsViewService"); }
            if (relatedProductViewService == null) { throw new ArgumentNullException("relatedProductViewService"); }

            ProductPriceViewService = productPriceViewService;
            ComposerContext = composerContext;
            ProductViewService = productViewService;
            ProductSpecificationsViewService = productSpecificationsViewService;
            RelatedProductViewService = relatedProductViewService;
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
                BaseUrl = RequestUtils.GetBaseUrl(Request)
            };

            var vm = await RelatedProductViewService.GetRelatedProductsAsync(param);

            return Ok(vm);
        }

        [ActionName("specifications")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> Specifications(GetSpecificationsRequest request)
        {
            var vm = await ProductSpecificationsViewService.GetProductSpecificationsViewModelAsync(new GetProductSpecificationsParam
            {
                ProductId = request.ProductId,
                VariantId = request.VariantId
            });

            return Ok(vm);
        }
    }
}
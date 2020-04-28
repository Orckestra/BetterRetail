using System;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class ProductContext : IProductContext
    {
        private readonly Lazy<ProductViewModel> _viewModel;
        protected IComposerContext ComposerContext { get; }
        protected IProductViewService ProductService { get; }
        protected HttpRequestBase Request { get; }
        protected Lazy<IPreviewModeService> PreviewModeService { get; }

        public ProductContext(
            IComposerContext composerContext,
            IProductViewService productService,
            HttpRequestBase request,
            Lazy<IPreviewModeService> previewModeService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ProductService = productService ?? throw new ArgumentNullException(nameof(productService));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            PreviewModeService = previewModeService ?? throw new ArgumentNullException(nameof(previewModeService));

            _viewModel = new Lazy<ProductViewModel>(() => GetProductViewModelAsync().Result, true);
        }

        public virtual ProductViewModel ViewModel => _viewModel.Value;


        protected virtual Task<ProductViewModel> GetProductViewModelAsync()
        {
            string id = Request[nameof(id)];
            string variantId = Request[nameof(variantId)];
            return GetProductViewModelAsync(id, variantId);
        }

        protected virtual async Task<ProductViewModel> GetProductViewModelAsync(string id, string variantId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return ContextHelper.HandlePreviewMode(() => GetProductViewModelAsync(PreviewModeService.Value.GetProductId()).Result);
            }

            var productViewModel = await ProductService.GetProductViewModelAsync(new GetProductParam
            {
                ProductId = id,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                VariantId = variantId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return productViewModel;
        }
     
    }
}

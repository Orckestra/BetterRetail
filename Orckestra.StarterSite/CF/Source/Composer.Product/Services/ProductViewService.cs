using System;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.Utils;
using System.Web;
using Orckestra.Composer.Logging;

namespace Orckestra.Composer.Product.Services
{
    public class ProductViewService : IProductViewService
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        protected IProductViewModelFactory ProductViewModelFactory { get; private set; }
        protected IProductUrlProvider ProductUrlProvider { get; private set; }

        public ProductViewService(IProductViewModelFactory productViewModelFactory, IProductUrlProvider productUrlProvider)
        {
            if (productViewModelFactory == null) { throw new ArgumentNullException("productViewModelFactory"); }
            if (productUrlProvider == null) { throw new ArgumentNullException("productUrlProvider"); }

            ProductViewModelFactory = productViewModelFactory;
            ProductUrlProvider = productUrlProvider;
        }

        public virtual async Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var product = await GetProductViewModelAsync(new GetProductParam
            {
                ProductId = param.ProductId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope,
                BaseUrl = param.BaseUrl

            }).ConfigureAwait(false);

            if (product == null)
            {
                return null;
            }

            var vm = new PageHeaderViewModel
            {
                PageTitle = GetPageTitle(product),
                CanonicalUrl = GetCanonicalUrl(param, product)
            };

            return vm;
        }

        protected virtual string GetPageTitle(ProductViewModel product)
        {
            return product.DisplayName;
        }

        protected virtual string GetCanonicalUrl(GetPageHeaderParam param, ProductViewModel product)
        {
            var relativeUri = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,
                ProductId = param.ProductId,
                ProductName = product.DisplayName
            });

            if (HttpContext.Current == null)
            {
                Log.Error("HttpContext.Current is null");

                return relativeUri;
            }

            try
            {
                var baseUri = RequestUtils.GetBaseUrl(HttpContext.Current.Request.Url);

                var url = new Uri(baseUri, relativeUri);

                return url.ToString();
            }
            catch (Exception ex)
            {
                string fullStackTrace = ex.StackTrace + Environment.StackTrace;
                Log.Error($"Error retrieving product canonical url. Exception : {ex}, {fullStackTrace}");

                return relativeUri;
            }
        }

        public virtual async Task<ProductViewModel> GetProductViewModelAsync(GetProductParam param)
        {
            return await ProductViewModelFactory.GetProductViewModel(param).ConfigureAwait(false);
        }
    }
}
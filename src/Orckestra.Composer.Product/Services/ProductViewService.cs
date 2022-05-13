using System;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Product.Services
{
    public class ProductViewService : IProductViewService
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        protected IProductViewModelFactory ProductViewModelFactory { get; private set; }
        protected IProductUrlProvider ProductUrlProvider { get; private set; }

        public ProductViewService(IProductViewModelFactory productViewModelFactory, IProductUrlProvider productUrlProvider)
        {
            ProductViewModelFactory = productViewModelFactory ?? throw new ArgumentNullException(nameof(productViewModelFactory));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
        }

        protected virtual string GetPageTitle(ProductViewModel product)
        {
            return product.DisplayName;
        }

        protected virtual string GetProductDescription(ProductViewModel product)
        {
            return product.Description;
        }

        protected virtual string GetCanonicalUrl(GetPageHeaderParam param, ProductViewModel product)
        {
            var relativeUri = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,
                ProductId = param.ProductId,
                ProductName = product.DisplayName,
                SKU = product.Sku
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

        public virtual Task<ProductViewModel> GetProductViewModelAsync(GetProductParam param)
        {
            return ProductViewModelFactory.GetProductViewModel(param);
        }
    }
}
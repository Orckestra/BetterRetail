using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Services
{
    public class ProductPriceViewService : IProductPriceViewService
    {
        protected IProductRepository ProductRepository { get; }
        protected IScopeViewService ScopeViewService { get; }
        protected IFulfillmentContext FulfillmentContext { get; }

        protected IProductPricesViewModelFactory ProductPricesViewModelFactory { get; set; }

        public ProductPriceViewService(IProductRepository productRepository, 
            IViewModelMapper viewModelMapper,
            IScopeViewService scopeViewService, 
            IFulfillmentContext fulfillmentContext,
            ICurrencyProvider currencyProvider,
            IProductPricesViewModelFactory productPricesViewModelFactory)
        {
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            ScopeViewService = scopeViewService ?? throw new ArgumentNullException(nameof(scopeViewService));
            FulfillmentContext = fulfillmentContext ?? throw new ArgumentNullException(nameof(fulfillmentContext));
            ProductPricesViewModelFactory= productPricesViewModelFactory ?? throw new ArgumentNullException(nameof(productPricesViewModelFactory));
        }

        /// <summary>
        /// Gets the products prices view model asynchronous.
        /// </summary>
        /// <param name="param">The get product price parameter.</param>
        /// <returns></returns>
        public virtual async Task<ProductsPricesViewModel> CalculatePricesAsync(GetProductsPriceParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.ProductIds == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductIds)), nameof(param)); }
            if (string.IsNullOrEmpty(param.Scope)) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var productsPriceTask = ProductRepository.CalculatePricesAsync(param.ProductIds, param.Scope, FulfillmentContext.AvailabilityAndPriceDate);
            var currencyTask = ScopeViewService.GetScopeCurrencyAsync(new GetScopeCurrencyParam
            {
                Scope = param.Scope,
                CultureInfo = param.CultureInfo
            });

            await Task.WhenAll(productsPriceTask, currencyTask).ConfigureAwait(false);

            //var productsPrice = await ProductRepository.CalculatePricesAsync(getProductPriceParam.ProductIds, getProductPriceParam.Scope).ConfigureAwait(false);
            var vm = CreateProductsPricesViewModel(new CreateProductPriceViewModelParam
            {
                CultureInfo = param.CultureInfo,
                ProductPrices = await productsPriceTask,
                CurrencyViewModel = await currencyTask
            });

            return vm;
        }

        /// <summary>
        /// Creates a list of <see cref="ProductPriceViewModel"/> based on an instance of List of <see cref="ProductPrice"/>.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual ProductsPricesViewModel CreateProductsPricesViewModel(CreateProductPriceViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductPrices == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductPrices)), nameof(param)); }

            var viewModel = new ProductsPricesViewModel()
            {
                ProductPrices = new List<ProductPriceViewModel>(),
                Currency = param.CurrencyViewModel
            };

            foreach (var productPrice in param.ProductPrices)
            {
                var productPriceVm = ProductPricesViewModelFactory.CreateProductPriceViewModel(param.CultureInfo, productPrice);
                viewModel.ProductPrices.Add(productPriceVm);
            }

            return viewModel;
        }
    }
}

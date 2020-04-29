using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
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
        protected ILocalizationProvider LocalizationProvider { get; }
        protected IViewModelMapper ViewModelMapper { get; }
        protected IScopeViewService ScopeViewService { get; }

        public ProductPriceViewService(IProductRepository productRepository, ILocalizationProvider localizationProvider, IViewModelMapper viewModelMapper,
            IScopeViewService scopeViewService)
        {
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ScopeViewService = scopeViewService ?? throw new ArgumentNullException(nameof(scopeViewService));
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

            var productsPriceTask = ProductRepository.CalculatePricesAsync(param.ProductIds, param.Scope);
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
                var productPriceVm = CreateProductPriceVm(param.CultureInfo, productPrice);
                viewModel.ProductPrices.Add(productPriceVm);
            }

            return viewModel;
        }

        protected virtual ProductPriceViewModel CreateProductPriceVm(CultureInfo cultureInfo, ProductPrice productPrice)
        {
            var productPriceVm = GenerateProductPriceVm(cultureInfo, productPrice);

            foreach (var variantPriceEntry in productPrice.VariantPrices)
            {
                var variantPriceVm = GenerateVariantPriceVm(cultureInfo, variantPriceEntry);
                productPriceVm.VariantPrices.Add(variantPriceVm);
            }

            return productPriceVm;
        }

        /// <summary>
        /// Creates the model view product price.
        /// </summary>
        /// <param name="cultureInfo">The get product price parameter.</param>
        /// <param name="productPrice">The product price.</param>
        /// <returns></returns>
        protected virtual ProductPriceViewModel GenerateProductPriceVm(CultureInfo cultureInfo, ProductPrice productPrice)
        {
            var vm = ViewModelMapper.MapTo<ProductPriceViewModel>(productPrice, cultureInfo);

            vm.IsPriceDiscounted = IsPriceDiscounted(productPrice.Pricing.Price, productPrice.DefaultPrice);
            vm.ListPrice = LocalizationProvider.FormatPrice(productPrice.Pricing.Price, cultureInfo);
            vm.VariantPrices = new List<VariantPriceViewModel>();

            return vm;
        }

        /// <summary>
        /// Generates the variant price model view item.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="variantPriceEntry">The variant price entry.</param>
        /// <returns></returns>
        protected virtual VariantPriceViewModel GenerateVariantPriceVm(CultureInfo cultureInfo, VariantPrice variantPriceEntry)
        {
            var vm = ViewModelMapper.MapTo<VariantPriceViewModel>(variantPriceEntry, cultureInfo);

            vm.IsPriceDiscounted = IsPriceDiscounted(variantPriceEntry.Pricing.Price, variantPriceEntry.DefaultPrice);
            vm.ListPrice = LocalizationProvider.FormatPrice(variantPriceEntry.Pricing.Price, cultureInfo);

            return vm;
        }

        /// <summary>
        /// Determines whether there is a discount on the product price (or its variants).
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="defaultPrice">The default price.</param>
        /// <returns></returns>
        protected bool IsPriceDiscounted(decimal price, decimal defaultPrice)
        {
            return price < defaultPrice;
        }
    }
}

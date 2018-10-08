using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Services
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
            if (productRepository == null) { throw new ArgumentNullException(nameof(productRepository)); }
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (viewModelMapper == null) { throw new ArgumentNullException(nameof(viewModelMapper)); }
            if (scopeViewService == null) { throw new ArgumentNullException(nameof(scopeViewService)); }

            ProductRepository = productRepository;
            LocalizationProvider = localizationProvider;
            ViewModelMapper = viewModelMapper;
            ScopeViewService = scopeViewService;
        }

        /// <summary>
        /// Gets the products prices view model asynchronous.
        /// </summary>
        /// <param name="getProductPriceParam">The get product price parameter.</param>
        /// <returns></returns>
        public virtual async Task<ProductsPricesViewModel> CalculatePricesAsync(GetProductsPriceParam getProductPriceParam)
        {
            ValidateProductsPriceInputParam(getProductPriceParam);

            var productsPriceTask = ProductRepository.CalculatePricesAsync(getProductPriceParam.ProductIds,
                getProductPriceParam.Scope);
            var currencyTask = ScopeViewService.GetScopeCurrencyAsync(new GetScopeCurrencyParam
            {
                Scope = getProductPriceParam.Scope,
                CultureInfo = getProductPriceParam.CultureInfo
            });

            await Task.WhenAll(productsPriceTask, currencyTask).ConfigureAwait(false);

            //var productsPrice = await ProductRepository.CalculatePricesAsync(getProductPriceParam.ProductIds, getProductPriceParam.Scope).ConfigureAwait(false);
            var vm = CreateProductsPricesViewModel(new CreateProductPriceViewModelParam
            {
                CultureInfo = getProductPriceParam.CultureInfo,
                ProductPrices = productsPriceTask.Result,
                CurrencyViewModel = currencyTask.Result
            });

            return vm;
        }

        /// <summary>
        /// Validates the products price input parameter.  Throws exceptions for each param invalud value.
        /// </summary>
        /// <param name="getProductPriceParam">The get product price parameter.</param>
        protected virtual void ValidateProductsPriceInputParam(GetProductsPriceParam getProductPriceParam)
        {
            if (getProductPriceParam == null)
            {
                throw new ArgumentNullException(nameof(getProductPriceParam));
            }

            if (getProductPriceParam.ProductIds == null)
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProductIds"), nameof(getProductPriceParam));
            }

            if (string.IsNullOrEmpty(getProductPriceParam.Scope))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(getProductPriceParam));
            }

            if (getProductPriceParam.CultureInfo == null)
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(getProductPriceParam));
            }
        }

        /// <summary>
        /// Creates a list of <see cref="ProductPriceViewModel"/> based on an instance of List of <see cref="ProductPrice"/>.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual ProductsPricesViewModel CreateProductsPricesViewModel(CreateProductPriceViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.ProductPrices == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProductPrices"), nameof(param)); }

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

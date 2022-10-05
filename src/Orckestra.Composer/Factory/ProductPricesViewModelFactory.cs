using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Factory
{
    public class ProductPricesViewModelFactory : IProductPricesViewModelFactory
    {
        protected ILocalizationProvider LocalizationProvider { get; }
        protected ICurrencyProvider CurrencyProvider { get; private set; }

        protected IViewModelMapper ViewModelMapper { get; }

        public ProductPricesViewModelFactory(
           ILocalizationProvider localizationProvider,
           IViewModelMapper viewModelMapper,
           ICurrencyProvider currencyProvider)
        {
           
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CurrencyProvider = currencyProvider ?? throw new ArgumentNullException(nameof(currencyProvider));
        }
        public virtual ProductPriceViewModel CreateProductPriceViewModel(CultureInfo cultureInfo, ProductPrice productPrice)
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
            vm.ListPrice = LocalizationProvider.FormatPrice(productPrice.Pricing.Price, CurrencyProvider.GetCurrency());
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
            vm.ListPrice = LocalizationProvider.FormatPrice(variantPriceEntry.Pricing.Price, CurrencyProvider.GetCurrency());

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

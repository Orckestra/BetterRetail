using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using System;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryProductViewModelFactory : ProductViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IGroceryProductInformationFactory GroceryProductInformationFactory { get; }

        public GroceryProductViewModelFactory(IViewModelMapper viewModelMapper,
            IProductRepository productRepository,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            ILookupService lookupService,
            IProductUrlProvider productUrlProvider,
            IScopeViewService scopeViewService,
            IRecurringOrdersRepository recurringOrdersRepository,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersSettings recurringOrdersSettings,
            IProductSpecificationsViewService productSpecificationsViewService,
            IMyAccountUrlProvider myAccountUrlProvider,
            IConverterProvider converterProvider,
            IGroceryProductInformationFactory groceryProductInformationFactory)
            : base(viewModelMapper,
                productRepository,
                damProvider,
                localizationProvider,
                lookupService,
                productUrlProvider,
                scopeViewService,
                recurringOrdersRepository,
                recurringOrderProgramViewModelFactory,
                recurringOrdersSettings,
                productSpecificationsViewService,
                myAccountUrlProvider)
        {
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            GroceryProductInformationFactory = groceryProductInformationFactory ?? throw new ArgumentNullException(nameof(groceryProductInformationFactory));
        }

        protected override ProductViewModel CreateViewModel(CreateProductDetailViewModelParam param)
        {
            var productViewModel = base.CreateViewModel(param);
            var extendedVM = productViewModel.AsExtensionModel<IGroceryProductViewModel>();

            extendedVM.ProductBadgeValues = GroceryProductInformationFactory.BuildProductBadgeValues(extendedVM.ProductBadgesKeys, extendedVM.ProductBadgesLookup);
            extendedVM.Format = GroceryProductInformationFactory.BuildProductFormat(extendedVM.ProductUnitQuantity,
                extendedVM.ProductUnitSize,
                extendedVM.ProductUnitMeasure,
                extendedVM.IsWeightedProduct,
                param.CultureInfo);

            var convertedVolumeMeasurment = BuildConvertedVolumeMeasurement(param.Product);
            if (convertedVolumeMeasurment.HasValue)
            {
                extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment.Value;
                productViewModel.Context["ConvertedVolumeMeasurement"] = convertedVolumeMeasurment.Value;
                productViewModel.Context["BaseProductMeasure"] = extendedVM.BaseProductMeasure;
            }

            return productViewModel;
        }

        public virtual decimal? BuildConvertedVolumeMeasurement(Overture.ServiceModel.Products.Product product)
        {
            var baseProductSize = product.PropertyBag.TryGetValue("BaseProductSize", out var sizeValue) ? double.Parse(sizeValue.ToString()) : 0;
            var baseProductMeasure = product.PropertyBag.TryGetValue("BaseProductMeasure", out var measureValue) ? measureValue.ToString() : string.Empty;
            var productUnitMeasure = product.PropertyBag.TryGetValue("ProductUnitMeasure", out var unitMeasureValue) ? unitMeasureValue.ToString() : string.Empty;
            if (string.IsNullOrEmpty(baseProductMeasure) || string.IsNullOrEmpty(productUnitMeasure))
            {
                return null;
            }

            return (decimal)ConverterProvider.ConvertMeasurements(baseProductSize, baseProductMeasure, productUnitMeasure);
        }
    }
}

using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Grocery.Providers;
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
using Orckestra.Composer.Grocery.ViewModels;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryProductViewModelFactory: ProductViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
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
            IConverterProvider converterProvider)
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
        }

        protected override ProductViewModel CreateViewModel(CreateProductDetailViewModelParam param)
        {
            var productViewModel = base.CreateViewModel(param);
            var extendedVM = productViewModel.AsExtensionModel<IGroceryProductViewModel>();
            var baseProductSize = param.Product.PropertyBag.ContainsKey("BaseProductSize") ? double.Parse(param.Product.PropertyBag["BaseProductSize"].ToString()) : 0;
            var baseProductMeasure = param.Product.PropertyBag.ContainsKey("BaseProductMeasure") ?  param.Product.PropertyBag["BaseProductMeasure"].ToString() : string.Empty;
            var weightVolumeQuantityMeasure = param.Product.PropertyBag.ContainsKey("ProductUnitMeasure") ?  param.Product.PropertyBag["ProductUnitMeasure"].ToString(): string.Empty;

            if (string.IsNullOrEmpty(baseProductMeasure) || string.IsNullOrEmpty(weightVolumeQuantityMeasure))
                return productViewModel;

            var convertedVolumeMeasurment = (decimal)ConverterProvider.ConvertMeasurements(baseProductSize, baseProductMeasure, weightVolumeQuantityMeasure);

            extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment;
            productViewModel.Context["ConvertedVolumeMeasurement"] = convertedVolumeMeasurment;
            productViewModel.Context["BaseProductMeasure"] = extendedVM.BaseProductMeasure;
            return productViewModel;
        }
    }
}

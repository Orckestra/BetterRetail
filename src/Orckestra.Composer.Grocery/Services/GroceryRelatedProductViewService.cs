﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryRelatedProductViewService : RelatedProductViewService
    {
        public GroceryProductViewModelFactory GroceryProductViewModelFactory { get; }

        public GroceryRelatedProductViewService(
            IProductRepository productRepository,
            IRelationshipRepository relationshipRepository,
            IDamProvider damProvider,
            IProductUrlProvider productUrlProvider,
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IInventoryLocationProvider inventoryLocationProvider,
            IRecurringOrdersSettings recurringOrdersSettings,
            IFulfillmentContext fulfillmentContext,
            IProductViewModelFactory groceryProductViewModelFactory)
            : base(productRepository, relationshipRepository, damProvider, productUrlProvider, viewModelMapper, localizationProvider, inventoryLocationProvider, recurringOrdersSettings, fulfillmentContext)
        {
            GroceryProductViewModelFactory = groceryProductViewModelFactory as GroceryProductViewModelFactory ?? throw new ArgumentNullException(nameof(groceryProductViewModelFactory));
        }

        protected override RelatedProductViewModel CreateRelatedProductsViewModel(
            Uri baseUrl,
            CultureInfo cultureInfo,
            ProductWithVariant productVariant,
            List<ProductPrice> prices,
            IEnumerable<ProductMainImage> images,
            string currencyIso)
        {
            var relatedProductViewModel = base.CreateRelatedProductsViewModel(baseUrl, cultureInfo, productVariant, prices, images, currencyIso);

            var extendedVM = relatedProductViewModel.AsExtensionModel<IGroceryRelatedProductViewModel>();
            extendedVM.ProductBadgeValues = GroceryProductViewModelFactory.BuildProductBadgeValues(productVariant.Product, relatedProductViewModel);

            return relatedProductViewModel;
        }
    }
}
using System;
using System.Collections.Generic;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Factory
{
    public interface IProductSearchViewModelFactory
    {
        ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria, IDictionary<(string, string), ProductMainImage> imgDictionary);
        void MapProductSearchViewModelPricing(ProductSearchViewModel productSearchVm, ProductPriceSearchViewModel pricing);
        void MapProductSearchViewModelAvailableForSell(ProductSearchViewModel productSearchViewModel, ProductDocument productDocument, bool IsInventoryEnabled);
    }
}
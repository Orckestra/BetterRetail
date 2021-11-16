using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Factory
{
    public interface IProductSearchViewModelFactory
    {
        ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria, IDictionary<(string ProductId, string VariantId), ProductMainImage> imgDictionary);
        Task<IList<ProductSearchViewModel>> EnrichAppendProductSearchViewModels(IList<(ProductSearchViewModel, ProductDocument)> productSearchResultList);
    }
}
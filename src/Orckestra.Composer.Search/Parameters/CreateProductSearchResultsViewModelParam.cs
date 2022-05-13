using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Search;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.Parameters
{
    public class CreateProductSearchResultsViewModelParam<TParam>
    {
        public ProductSearchResult SearchResult { get; set; }
        public ProductSearchResult CategoryFacetCountsResult { get; set; }
        public TParam SearchParam { get; set; }
        public List<ProductMainImage> ImageUrls { get; set; }

        public CreateProductSearchResultsViewModelParam()
        {
            ImageUrls = new List<ProductMainImage>();
        }
    }
}

using System.Collections.Generic;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Parameters
{
    public class CreateProductSearchResultsViewModelParam<TParam>
    {
        public ProductSearchResult SearchResult { get; set; }
        public TParam SearchParam { get; set; }
        public List<ProductMainImage> ImageUrls { get; set; }

        public CreateProductSearchResultsViewModelParam()
        {
            ImageUrls = new List<ProductMainImage>();
        }
    }
}

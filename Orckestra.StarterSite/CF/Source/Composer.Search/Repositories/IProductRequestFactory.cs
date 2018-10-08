using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Requests.Search;

namespace Orckestra.Composer.Search.Repositories
{
    public interface IProductRequestFactory
    {
        SearchAvailableProductsRequest CreateProductRequest(string scopeId);

        SearchAvailableProductsBaseRequest CreateProductRequest(SearchCriteria criteria);
    }
}